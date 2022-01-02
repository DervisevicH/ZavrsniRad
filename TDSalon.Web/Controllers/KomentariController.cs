using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TDSalon.Data;
using TDSalon.Web.Helper;
using TDSalon.Web.Models;

namespace TDSalon.Web.Controllers
{
    public class KomentariController : Controller
    {
        private readonly TDSalondbContext _db;
        public KomentariController(TDSalondbContext db)
        {
            _db = db;
        }
        public IActionResult Index(int id)
        {
            KomentariVM model = new KomentariVM();
            model.rows = _db.Komentari.Where(x => x.ProizvodDetaljiId == id).Select(x => new KomentariVM.Rows
            {
                Datum = x.Datum.Value.ToShortDateString(),
                Komentar = x.Komentar,
                KomentarId = x.KomentarId,
                Korisnik = _db.KorisnickiNalozi.Where(nalog => nalog.KorisnickiNalogId == x.Kupac.KorisnickiNalogId).Select(nalog => nalog.Username).Single(),
                KorisnikId = x.KupacId.Value,
                ProizvodId = x.ProizvodDetaljiId.Value,              
                Ocjena = x.Ocjena.Value,
                Naslov = x.Naslov
            }).ToList();
            return PartialView("_KomentariPartial", model);
        }
        [HttpGet]
        [Authorize(Roles = "Kupac")]

        public async Task<ActionResult> Dodaj(int ProizvodId)
        {
            int logiraniKupac = HttpContext.GetUserId();
            Kupci kupacDb = await _db.Kupci.FindAsync(logiraniKupac);
            KomentarDodajVM model = new KomentarDodajVM();
            if (kupacDb != null)
            {
                model = new KomentarDodajVM()
                {
                    ProizvodId = ProizvodId,
                    KupacId = logiraniKupac,
                    Ime = kupacDb.Ime,
                    Email = kupacDb.Email
                };
            }         

            return PartialView("_KomentarDodajPartial", model);
        }
        [HttpPost]
        [Authorize(Roles = "Kupac")]

        public async Task<ActionResult> Sacuvaj(KomentarDodajVM model)
        {
            if (ModelState.IsValid)
            {
                Komentari komentar = new Komentari()
                {
                    KupacId = model.KupacId,
                    Komentar = model.Komentar,
                    Datum = System.DateTime.Now,
                    ProizvodDetaljiId = model.ProizvodId,
                    Naslov = model.Naslov,
                    Ocjena = model.Ocjena
                };
                await _db.Komentari.AddAsync(komentar);
                await _db.SaveChangesAsync();
            }

            return RedirectToAction("ProizvodDetalji", "Proizvodi", new { proizvodId = model.ProizvodId });
        }

    }
}
