using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TDSalon.Data;
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

        public ActionResult Dodaj(int ProizvodId)
        {
            //KorisnickiNalozi logiraniKorisnik = HttpContext.GetLogiraniKorisnik();
            //if (logiraniKorisnik == null)
            //{
            //    TempData["returnUrl"] = HttpContext.Request.Path.ToString();
            //    return RedirectToAction("Login", "Autentifikacija");

            //}
            //Kupci kupac = _db.Kupci.Where(x => x.KorisnickiNalogId == logiraniKorisnik.KorisnickiNalogId).Single();
            KomentarDodajVM model = new KomentarDodajVM()
            {
                ProizvodId = ProizvodId,
                KupacId = 1,
                Ime = "amina",
                Email = "amina@email.com"
            };

            return PartialView("_KomentarDodajPartial", model);
        }
        [HttpPost]
        public ActionResult Sacuvaj(KomentarDodajVM model)
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
                _db.Komentari.Add(komentar);
                _db.SaveChanges();
            }
            return RedirectToAction("ProizvodiById", "Proizvodi", new { id = model.ProizvodId });
        }

    }
}
