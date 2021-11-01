using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PagedList;
using TDSalon.Data;
using TDSalon.Web.Helper;
using TDSalon.Web.Hubs;
using TDSalon.Web.Models;
namespace TDSalon.Web.Controllers
{
    public class PitanjaController : Controller
    {
        private TDSalondbContext _db;
        IMapper _mapper;
        private readonly IHubContext<NotificationHub> _notificationHubContext;
        private readonly INotificationManager _notificationManager;
        public PitanjaController(TDSalondbContext db, IMapper mapper, IHubContext<NotificationHub> notificationHubContext, INotificationManager notificationManager) { _db = db; _mapper = mapper;_notificationHubContext = notificationHubContext;_notificationManager = notificationManager; }

        [Authorize(Roles = "Zaposlenik")]
        [HttpGet]
        public async Task<ActionResult> Index(int proizvodId, bool isOdgovorena)
         {            
            List<Pitanja> pitanjaDb = await _db.Pitanja.Include(x=>x.Proizvod.ProizvodDetalji).Include(x=>x.Kupac.KorisnickiNalog).AsNoTracking().ToListAsync();
            PitanjaIndexVM model = new PitanjaIndexVM();
            model.ListaPitanja = _mapper.Map<List<PitanjaVM>>(pitanjaDb);
            if (isOdgovorena) { 
            foreach (var item in model.ListaPitanja)
            {
                var odgovor = _db.Odgovori.Where(x => x.PitanjeId == item.PitanjeId).FirstOrDefault();
                if (odgovor != null)
                {
                    item.Odgovor = odgovor.Odgovor;
                }
            }
                model.ListaPitanja = model.ListaPitanja.Where(x => !String.IsNullOrEmpty(x.Odgovor)).ToList(); 
            }
            if (proizvodId != 0)
            {
                model.ListaPitanja = model.ListaPitanja.Where(x =>x.ProizvodId==proizvodId).ToList();

            }
            var listaProizvoda = await _db.Proizvodi.Include(x => x.ProizvodDetalji).Where(x=>x.ProizvodDetalji.IsAktivan == true).ToListAsync();
            model.ListaProizvoda = _mapper.Map<List<SelectListItem>>(listaProizvoda);
            return View(model);            
        }
        [Authorize(Roles = "Kupac")]
        public ActionResult DodajPitanje(int proizvodId)
        {
            Proizvodi proizvod = _db.Proizvodi.Include(x => x.ProizvodDetalji).Where(x => x.ProizvodId == proizvodId).SingleOrDefault();
            PitanjaDodajVM model = new PitanjaDodajVM
            {
                ProizvodNaziv=proizvod.ProizvodDetalji.Naziv,
                ProizvodId = proizvodId
            };
            return View("Dodaj", model);
        }
        [HttpPost]
        [Authorize(Roles = "Kupac")]
        public async Task<ActionResult> Sacuvaj(PitanjaDodajVM model)
        {
            if (ModelState.IsValid)
            {

                int kupacId = HttpContext.GetUserId();
                string username = HttpContext.GetUsername();
                Pitanja novoPitanje = new Pitanja
                {
                    Datum = System.DateTime.Now,
                    KupacId = kupacId,
                    ProizvodId = model.ProizvodId,
                    Pitanje = model.Pitanje,
                    Procitano = false
                };
                
                await _db.Pitanja.AddAsync(novoPitanje);
                await _db.SaveChangesAsync();
                var proizvodDb = await _db.Proizvodi.Include(x=>x.ProizvodDetalji).Where(x=>x.ProizvodId==model.ProizvodId).SingleOrDefaultAsync();

                Notifikacije novaNotifikacija = new Notifikacije();
                novaNotifikacija.DatumKreiranja = System.DateTime.Now;
                novaNotifikacija.ZaposlenikId = await _db.Zaposlenici.Select(x => x.ZaposlenikId).FirstOrDefaultAsync();
                novaNotifikacija.Sadrzaj = $"{username} je postavio novo pitanje za proizvod {proizvodDb.ProizvodDetalji.Naziv}";
                novaNotifikacija.SadrzajId = novoPitanje.PitanjeId;
                novaNotifikacija.TipNotifikacije = "Pitanje";
                novaNotifikacija.Procitano = false;

                await _db.Notifikacije.AddAsync(novaNotifikacija);
                await _db.SaveChangesAsync();
                List<Notifikacije> listNotifikacija = new List<Notifikacije>() { novaNotifikacija };
                var connections = _notificationManager.GetUserConnections(novaNotifikacija.ZaposlenikId.ToString());
                if (connections != null && connections.Count > 0)
                {
                    foreach (var connectionId in connections)
                    {
                        foreach (var item in listNotifikacija)
                        {                            
                            await _notificationHubContext.Clients.Client(connectionId).SendAsync("novaNotifikacija", novoPitanje.PitanjeId, novaNotifikacija.Sadrzaj, novaNotifikacija.TipNotifikacije, novaNotifikacija.NotifikacijaId);
                        }
                    }
                }
            }

            TempData["SuccessMessage"] = "Uspješno ste poslali pitanje!";
            return RedirectToAction("ProizvodDetalji","Proizvodi", new {proizvodId = model.ProizvodId });
        }
        [HttpGet]
        [Authorize(Roles = "Kupac")]
        public async Task<ActionResult> PitanjaByKorisnik()
        {
            int kupacId = HttpContext.GetUserId();
            var odgovorenaPitanja = await _db.Odgovori.Include(x => x.Pitanje).Where(x => x.Pitanje.KupacId == kupacId).ToListAsync();
            PitanjaKorisnikVM model = new PitanjaKorisnikVM();
            model.Rows = odgovorenaPitanja.Select(x => new PitanjaKorisnikVM.Row
            {
                OdgovorId = x.OdgovorId,
                PitanjeId = x.Pitanje.PitanjeId,
                Pitanje = x.Pitanje.Pitanje,
                Odgovor = x.Odgovor
            }).ToList();
            var neOdgovorenaPitanja = await _db.Pitanja.Where(x => x.KupacId == kupacId).ToListAsync();
            foreach (var item in neOdgovorenaPitanja)
            {
                PitanjaKorisnikVM.Row pitanje = new PitanjaKorisnikVM.Row()
                {
                    Odgovor = "",
                    Pitanje = item.Pitanje,
                    PitanjeId = item.PitanjeId
                };
                model.Rows.Add(pitanje);
            }
            return View("PitanjaByKorisnik",model);
        }
    }
}
