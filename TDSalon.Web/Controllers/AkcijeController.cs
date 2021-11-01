using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TDSalon.Data;
using TDSalon.Web.Hubs;
using TDSalon.Web.Models;

namespace TDSalon.Web.Controllers
{
    public class AkcijeController : Controller
    {
        private readonly TDSalondbContext _db;
        IMapper _mapper;
        private readonly IHubContext<NotificationHub> _notificationHubContext;
        private readonly INotificationManager _notificationManager;
        public AkcijeController(TDSalondbContext db, IMapper mapper, IHubContext<NotificationHub> notificationHubContext, INotificationManager notificationManager) { _db = db; _mapper = mapper; _notificationHubContext = notificationHubContext; _notificationManager = notificationManager; }

        public async Task<ActionResult> AkcijeShop()
        {
            return View();
        }
        public async Task<ActionResult> Index(string sortOrder,string naziv,DateTime? datumOd,DateTime? datumDo)
        {
            ViewData["NazivSortParm"] = String.IsNullOrEmpty(sortOrder) ? "naziv_desc" : "";
            ViewData["DatumOdSortParm"] = sortOrder == "datumOd" ? "datumOd_desc" : "datumOd";
            ViewData["DatumDoSortParm"] = sortOrder == "datumDo" ? "datumDo_desc" : "datumDo";

            AkcijeIndexVM model = new AkcijeIndexVM();
            model.AkcijaOd = DateTime.Now.Date;
            model.AkcijaDo = DateTime.Now.Date;
            model.ListaAkcija = new List<AkcijeVM>();

            var listaAkcija = await _db.Akcije.AsNoTracking().ToListAsync();
            if (!String.IsNullOrEmpty(naziv))
            {
                listaAkcija = listaAkcija.Where(s => s.Naziv.Contains(naziv)).ToList();
            }
            if(datumOd!=null && datumDo != null)
            {
                listaAkcija = listaAkcija.Where(s => s.DatumOd >= datumOd && s.DatumDo <= datumDo).ToList();
            }
            switch (sortOrder)
            {
                case "naziv_desc":
                    listaAkcija = listaAkcija.OrderByDescending(s => s.Naziv).ToList();
                    break;
                case "datumOd":
                    listaAkcija = listaAkcija.OrderBy(s => s.DatumOd).ToList(); 
                    break;
                case "datumOd_desc":
                    listaAkcija = listaAkcija.OrderByDescending(s => s.DatumOd).ToList();
                    break;
                case "datumDo":
                    listaAkcija = listaAkcija.OrderBy(s => s.DatumDo).ToList();
                    break;
                case "datumDo_desc":
                    listaAkcija = listaAkcija.OrderByDescending(s => s.DatumDo).ToList();
                    break;
                default:
                    listaAkcija = listaAkcija.OrderBy(s => s.Naziv).ToList();
                    break;
            }
            model.ListaAkcija=_mapper.Map<List<AkcijeVM>>(listaAkcija);            
            return View(model);
        }
        public async Task<ActionResult> DodajAkciju()
        {
            AkcijeDodajVM model = new AkcijeDodajVM();
            model.ListaProizvoda = await _db.Proizvodi.Include(x => x.ProizvodDetalji).Where(x=>x.ProizvodDetalji.IsAktivan==true).Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = x.ProizvodId.ToString(),
                Text = x.ProizvodDetalji.Naziv                
            }).ToListAsync();
            model.AkcijaOd = DateTime.Now.Date;
            model.AkcijaDo = DateTime.Now.Date;
            return View("Dodaj", model);
        }        
        public async Task<JsonResult> Sacuvaj(decimal postotak, string naziv, DateTime datumOd, DateTime datumDo,bool isObavijesti, int[] listaProizvoda)
        {
            try
            {
                if (_db.Akcije.Any(x => x.IsAktivna.Value))
                {
                    TempData["SuccessMessage"] = "Akcija već postoji!";
                    return Json(new { redirectToUrl = Url.Action("DodajAkciju", "Akcije") });
                }
                Akcije akcija = new Akcije()
                {
                    Postotak = postotak,
                    DatumOd = datumOd,
                    DatumDo = datumDo,
                    Naziv = naziv,
                    IsAktivna = true
                };
                await _db.Akcije.AddAsync(akcija);
                await _db.SaveChangesAsync();

                foreach (var item in listaProizvoda)
                {
                    AkcijeProizvodi proizvod = new AkcijeProizvodi();
                    proizvod.ProizvodId = Convert.ToInt32(item);
                    proizvod.AkcijaId = akcija.AkcijaId;
                    proizvod.Cijena = _db.Proizvodi.Where(x => x.ProizvodId == proizvod.ProizvodId).Select(x => x.Cijena.Value).SingleOrDefault();
                    proizvod.AkcijskaCijena = proizvod.Cijena.Value - (proizvod.Cijena.Value * (postotak / 100));
                    proizvod.Postotak = Convert.ToInt32(postotak);
                    await _db.AkcijeProizvodi.AddAsync(proizvod);
                }
                var proizvodiDb = await _db.Proizvodi.Where(x => listaProizvoda.Contains(x.ProizvodId)).ToListAsync();
                proizvodiDb.ForEach(x => x.IsAkcija = true);
                await _db.SaveChangesAsync();

                if (isObavijesti)
                {
                    var listaAkcijeProizvodi =  _db.AkcijeProizvodi.Where(x => x.AkcijaId == akcija.AkcijaId).ToList();
                    var listaFavoritiStavke = _db.FavoritiStavke.Include(x => x.Favorit).Include(x=>x.Proizvod.ProizvodDetalji).ToList();
                    var listaFavorita = listaFavoritiStavke.Where(x => listaAkcijeProizvodi.Any(y => y.ProizvodId == x.ProizvodId)).ToList();
                    List<Notifikacije> akcijaNotifikacije = new List<Notifikacije>();
                    foreach (var item in listaFavorita)
                    {
                        Notifikacije novaNotifikacija = new Notifikacije()
                        {
                            KupacId = item.Favorit.KupacId,
                            Procitano = false,
                            Sadrzaj = "Proizvod " + item.Proizvod.ProizvodDetalji.Naziv + " je na akciji!",
                            TipNotifikacije = "Akcija",
                            DatumKreiranja = DateTime.Now,
                            SadrzajId = item.ProizvodId
                        };
                        await _db.Notifikacije.AddAsync(novaNotifikacija);
                        akcijaNotifikacije.Add(novaNotifikacija);
                    }
                    await _db.SaveChangesAsync();                   
                    foreach (var notifikacija in akcijaNotifikacije)
                    {
                        var connections = _notificationManager.GetUserConnections(notifikacija.KupacId.ToString());
                        if (connections != null && connections.Count > 0)
                        {
                            foreach (var connectionId in connections)
                            {
                                await _notificationHubContext.Clients.Client(connectionId).SendAsync(
                                    "novaNotifikacija",
                                    notifikacija.SadrzajId,
                                    notifikacija.Sadrzaj,
                                    notifikacija.TipNotifikacije);
                            }
                        }

                    }
                }
                TempData["SuccessMessage"] = "Uspješno ste kreirali akciju!";
                return Json(new { redirectToUrl = Url.Action("DodajAkciju", "Akcije") });
            }
            catch (Exception ex)
            {

                throw;
            }
         
        }
        public async Task<ActionResult> UrediAkciju(int akcijaId)
        {
            Akcije dbAkcija =await _db.Akcije.FindAsync(akcijaId);
            if (dbAkcija != null)
            {
               AkcijeUrediVM model = new AkcijeUrediVM();               
               List<AkcijeProizvodi> akcijeProizvodi = await _db.AkcijeProizvodi.Include(x=>x.Proizvod.ProizvodDetalji).Where(x => x.AkcijaId == akcijaId).ToListAsync();
              
               model = _mapper.Map<AkcijeUrediVM>(dbAkcija);
               model.AkcijeProizvodi = _mapper.Map<List<AkcijeProizvodiVM>>(akcijeProizvodi);
               model.ListaProizvoda = await _db.Proizvodi.Include(x => x.ProizvodDetalji).Where(x=>x.ProizvodDetalji.IsAktivan == true).Select(x => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
                {
                    Value = x.ProizvodId.ToString(),
                    Text = x.ProizvodDetalji.Naziv
                }).ToListAsync();

                return View("Uredi", model);
            }
            else
            {
                return NotFound();
            }
        }
        [HttpPost]
        public async Task<ActionResult> SacuvajAkciju(AkcijeUrediVM model)
        {
            Akcije akcijaDb = await _db.Akcije.FindAsync(model.AkcijaId);
            if (akcijaDb != null)
            {
                akcijaDb.Postotak = model.Postotak;
                akcijaDb.Naziv = model.Naziv;
                akcijaDb.DatumOd = model.DatumOd;
                akcijaDb.DatumDo = model.DatumDo;

                _db.Akcije.Update(akcijaDb);
                var proizvodiAkcije = await _db.AkcijeProizvodi.Where(x => x.AkcijaId == model.AkcijaId).ToListAsync();
               
                await _db.SaveChangesAsync();

                TempData["SuccessMessage"] = "Uspješno ste sačuvali podatke!";
                return RedirectToAction("UrediAkciju",new { akcijaId=model.AkcijaId});

            }
            else
            {
                return NotFound();
            }
        }
        
        [HttpPost]
        public async Task<ActionResult> DodajAkcijskiProizvod(int proizvodId, decimal postotak,int akcijaId)
        {
            var proizvodDb = _db.Proizvodi.Include(x => x.ProizvodDetalji).Where(x => x.ProizvodId == proizvodId).SingleOrDefault();
            if (proizvodDb != null)
            {
                var akcijeProizvodiDb = _db.AkcijeProizvodi.Include(x => x.Proizvod.ProizvodDetalji).Where(x => x.AkcijaId == akcijaId).ToList();
                AkcijeUrediVM model = new AkcijeUrediVM();

                AkcijeProizvodiVM akcijaProizvodVM = new AkcijeProizvodiVM()
                {
                    Naziv = proizvodDb.ProizvodDetalji.Naziv,
                    Sifra = proizvodDb.Sifra,
                    Cijena = proizvodDb.Cijena.Value,
                    Postotak = Convert.ToInt32(postotak),
                    ProizvodId = proizvodId,
                    AkcijaId = akcijaId,
                    
                    AkcijskaCijena = proizvodDb.Cijena.Value - (proizvodDb.Cijena.Value * (postotak / 100))
                };
                var akcijaProizvodDb = _mapper.Map<AkcijeProizvodi>(akcijaProizvodVM);
                proizvodDb.IsAkcija = true;

                _db.Proizvodi.Update(proizvodDb);
                await _db.AkcijeProizvodi.AddAsync(akcijaProizvodDb);
                await _db.SaveChangesAsync();
                   
                akcijeProizvodiDb = _db.AkcijeProizvodi.Include(x => x.Proizvod.ProizvodDetalji).Where(x => x.AkcijaId == akcijaId).ToList();

                model.AkcijeProizvodi = _mapper.Map<List<AkcijeProizvodiVM>>(akcijeProizvodiDb);
                return PartialView("_ProizvodiAkcije",model);
            }
            else
                return null;
        }

        [ActionName("IzbrisiAkcijskiProizvod")]
        [HttpGet]
        public async Task<ActionResult> IzbrisiAkcijskiProizvod(int akcijaProizvodId)
        {
            AkcijeProizvodi akcijeProizvodiDb = await _db.AkcijeProizvodi.FindAsync(akcijaProizvodId);
            if (akcijeProizvodiDb != null)
            {
                Proizvodi proizvodDb = await _db.Proizvodi.FindAsync(akcijeProizvodiDb.ProizvodId);
                proizvodDb.IsAkcija = false;

                _db.Proizvodi.Update(proizvodDb);
                _db.AkcijeProizvodi.Remove(akcijeProizvodiDb);

                await _db.SaveChangesAsync();

                var listaAkcijeProizvodiDb = _db.AkcijeProizvodi.Include(x => x.Proizvod.ProizvodDetalji).Where(x => x.AkcijaId == akcijeProizvodiDb.AkcijaId).ToList();

                AkcijeUrediVM model = new AkcijeUrediVM();
                model.AkcijeProizvodi = _mapper.Map<List<AkcijeProizvodiVM>>(listaAkcijeProizvodiDb);
                return PartialView("_ProizvodiAkcije", model);
            }
            else
            {
                return NotFound();
            }
        }

        public async Task<ActionResult> DeaktivirajAkciju(int akcijaId)
        {
            var akcijaDb = await _db.Akcije.FindAsync(akcijaId);
            if (akcijaDb != null)
            {
                var proizvodiAkcijeDb = _db.AkcijeProizvodi.Where(x => x.AkcijaId == akcijaId).ToList();
                var proizvodiDb = _db.Proizvodi.ToList();
                var akcijskiProizvodiDb = proizvodiDb.Where(x => proizvodiAkcijeDb.Any(y => y.ProizvodId == x.ProizvodId)).ToList();

                akcijskiProizvodiDb.ForEach(x => x.IsAkcija = false);
                akcijaDb.IsAktivna = false;

                _db.Akcije.Update(akcijaDb);
                await _db.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }
        }
    }
}
