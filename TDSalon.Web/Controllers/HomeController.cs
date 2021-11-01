using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TDSalon.Data;
using TDSalon.Web.Hubs;
using TDSalon.Web.Models;

namespace TDSalon.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHubContext<NotificationHub> _notificationHubContext;
        private readonly TDSalondbContext _db;

        public HomeController(IHubContext<NotificationHub> notificationHubContext, TDSalondbContext db)
        {
            _notificationHubContext = notificationHubContext;
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Zaposlenik")]
        public async Task<ActionResult> IndexZaposlenik()
        {
            HomeZaposlenikVM model = new HomeZaposlenikVM();
            model.UkupnoKupaca = await _db.Kupci.Distinct().AsNoTracking().Select(x => x.KupacId).CountAsync();
            model.UkupnoPitanja = await _db.Pitanja.Distinct().AsNoTracking().Select(x => x.PitanjeId).CountAsync();
            model.UkupnoNarudzbi = await _db.Narudzbe.Distinct().AsNoTracking().Select(x => x.NarudzbaId).CountAsync();
            model.UkupnoZaradjeno =await _db.Narudzbe.Where(x => x.Procesirana == true).SumAsync(x => x.Ukupno.Value);

            model.ListaKupaca = new List<KupacRow>();
            var listaKupacaDb = await _db.Kupci.Include(x=>x.KorisnickiNalog).OrderByDescending(x=>x.DatumRegistracije).AsNoTracking().ToListAsync();
            foreach (var item in listaKupacaDb)
            {
                KupacRow row = new KupacRow()
                {
                    DatumRegistracije = item.DatumRegistracije.Value,       
                    
                    KorisnickoIme = item.KorisnickiNalog.Username
                };
                if(!String.IsNullOrEmpty(item.Ime) && !String.IsNullOrEmpty(item.Prezime))
                {
                    row.ImePrezime = item.Ime + " " + item.Prezime;
                }
                if (!String.IsNullOrEmpty(item.Spol))
                {
                    row.Spol = item.Spol;
                }
                model.ListaKupaca.Add(row);
            }
            model.ListaNarudzbi = new List<NarudzbaRow>();
            var listaNarudzbiDb = await _db.Narudzbe.Include(x=>x.Kupac).Where(x=>x.Procesirana == false).OrderByDescending(x => x.Datum).AsNoTracking().ToListAsync();
            foreach (var item in listaNarudzbiDb)
            {
                NarudzbaRow row = new NarudzbaRow()
                {
                    NarudzbaId = item.NarudzbaId,
                    Kupac = item.Kupac.Ime + " " + item.Kupac.Prezime,
                    BrojNarudzbe = item.BrojNarudzbe.ToString(),
                    Datum = item.Datum.Value,
                    Status = item.StatusNarudzbe,
                    Ukupno = item.Ukupno.Value
                };
                model.ListaNarudzbi.Add(row);
            }
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<ActionResult> ONama()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
