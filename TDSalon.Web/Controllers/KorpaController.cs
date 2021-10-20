using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TDSalon.Data;
using TDSalon.Web.Helper;
using TDSalon.Web.Models;

namespace TDSalon.Web.Controllers
{
    [Authorize(Roles = "Kupac")]
    public class KorpaController : Controller
    {
        private readonly TDSalondbContext _db;
        public KorpaController(TDSalondbContext db) { _db = db; }
        public async Task<ActionResult> DodajUKorpu(int proizvodId, decimal kolicina = 1)
        {
            try
            {
                int korisnikId = HttpContext.GetUserId();
                Korpe korpa = await _db.Korpe.Where(x => x.KupacId == korisnikId).AsNoTracking().SingleOrDefaultAsync();

                if (korpa == null)
                {
                    korpa = new Korpe()
                    {
                        KupacId = korisnikId,
                        Ukupno = 0                        
                    };
                    await _db.Korpe.AddAsync(korpa);
                    await _db.SaveChangesAsync();
                }
                Proizvodi proizvod = await _db.Proizvodi.FindAsync(proizvodId);
                decimal cijena = proizvod.Cijena.Value;
                var trenutnaAkcija = await _db.Akcije.Where(x => x.IsAktivna == true).SingleOrDefaultAsync();
                if (proizvod.IsAkcija.HasValue)
                {
                    if (proizvod.IsAkcija.Value)
                        cijena = await _db.AkcijeProizvodi.Where(x => x.ProizvodId == proizvod.ProizvodId && trenutnaAkcija.AkcijaId == x.AkcijaId).Select(x => x.AkcijskaCijena.Value).SingleOrDefaultAsync();

                }
                KorpaProizvodi stavka = await _db.KorpaProizvodi.Where(x => x.KorpaId == korpa.KorpaId && x.ProizvodId == proizvodId).SingleOrDefaultAsync();
                if (stavka == null)
                {
                    stavka = new KorpaProizvodi()
                    {
                        KorpaId = korpa.KorpaId,
                        ProizvodId = proizvodId,
                        Kolicina = kolicina,
                        Cijena = cijena
                    };
                    await _db.KorpaProizvodi.AddAsync(stavka);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    stavka.Kolicina += kolicina;
                    stavka.Cijena = cijena;
                    _db.KorpaProizvodi.Update(stavka);
                    await _db.SaveChangesAsync();
                }
                korpa.Ukupno += kolicina * cijena;
                korpa.DatumModifikacije = System.DateTime.Now;
                _db.Korpe.Update(korpa);
                await _db.SaveChangesAsync();

                return Ok();
            }
            catch (Exception)
            {

                return NoContent();
            }

        }
        public async Task<ActionResult> KorpaIndex()
        {
            int korisnikId = HttpContext.GetUserId();
            Korpe korpa = await _db.Korpe.Where(x => x.KupacId == korisnikId).AsNoTracking().SingleOrDefaultAsync() ;
           
            KorpaIndexVM model = new KorpaIndexVM()
            {
                KorpaId = korpa.KorpaId,
                TroskoviDostave = 0,
                Ukupno = korpa.Ukupno.Value,
                UkupnoZaPlatiti=0+korpa.Ukupno.Value,
                rows = new List<KorpaIndexVM.Row>()
            };
            List<KorpaProizvodi> listaProizvoda =await _db.KorpaProizvodi.Include(x=>x.Proizvod).Where(x => x.KorpaId == korpa.KorpaId).ToListAsync();
            foreach (var item in listaProizvoda)
            {
                KorpaIndexVM.Row row = new KorpaIndexVM.Row()
                {
                    StavkaId = item.KorpaProizvodId,
                    Cijena = item.Cijena.Value,
                    Dimenzija = GetDimenziju(item.ProizvodId.Value),
                    Kolicina = item.Kolicina.Value,
                    Slika = await _db.Slike.Where(x => x.ProizvodDetaljiId == item.Proizvod.ProizvodDetaljiId).Select(x => x.SlikaUrl).FirstOrDefaultAsync(),
                    NazivProizvoda = await _db.ProizvodiDetalji.Where(x => x.ProizvodDetaljiId == item.Proizvod.ProizvodDetaljiId).Select(x => x.Naziv).FirstOrDefaultAsync(),
                    IsAkcija = item.Proizvod.IsAkcija.Value,
                    ProizvodId = item.ProizvodId.Value
                };
                model.rows.Add(row);
            }
            if (model.rows.Any(x => x.IsAkcija==true))
            {
                var listaProizvodaNaAkciji = await _db.AkcijeProizvodi.Include(x => x.Proizvod).Include(x => x.Akcija).Where(x => x.Akcija.IsAktivna == true).ToListAsync();
                var listaProizvodaUKorpi = model.rows.ToList();

                foreach (var item in listaProizvodaUKorpi)
                {
                    var akcijskiProizvod = listaProizvodaNaAkciji.Where(x => x.ProizvodId == item.ProizvodId).SingleOrDefault();
                    var proizvod = model.rows.Where(x => x.ProizvodId == item.ProizvodId).SingleOrDefault();
                    if (akcijskiProizvod != null)
                    {
                        proizvod.Cijena = akcijskiProizvod.AkcijskaCijena.Value;
                    }
                }
            }
            return View("KorpaIndex",model);
        }
        public async Task<ActionResult> IndexPartial()
        {
            int kupacId = HttpContext.GetUserId();
            KorpaPartialVM model = new KorpaPartialVM();
            Korpe korpa =await _db.Korpe.Where(x => x.KupacId == kupacId).AsNoTracking().SingleOrDefaultAsync();
            if (korpa != null)
            {
                model.KorpaId = korpa.KorpaId;
                model.rows = new List<KorpaPartialVM.Row>();
                List<KorpaProizvodi> stavkeKorpe =await _db.KorpaProizvodi.Include(x=>x.Proizvod.ProizvodDetalji).Where(x => x.KorpaId == korpa.KorpaId).ToListAsync();
                foreach (var item in stavkeKorpe)
                {
                    KorpaPartialVM.Row row = new KorpaPartialVM.Row()
                    {
                        StavkaId = item.KorpaProizvodId,
                        NazivProizvoda = item.Proizvod.ProizvodDetalji.Naziv,
                        Cijena = item.Proizvod.Cijena.Value,
                        Kolicina = item.Kolicina.ToString(),
                        Slika = await _db.Slike.Where(x => x.ProizvodDetaljiId == item.Proizvod.ProizvodDetaljiId).Select(x => x.SlikaUrl).FirstOrDefaultAsync(),
                        IsAkcija = item.Proizvod.IsAkcija.Value,
                        ProizvodId = item.ProizvodId.Value
                    };
                    model.rows.Add(row);
                }
                var trenutnaAkcija = await _db.Akcije.Where(x => x.IsAktivna == true).SingleOrDefaultAsync();
                if (trenutnaAkcija != null)
                {
                    var proizvodiNaAkciji = await _db.AkcijeProizvodi.Where(x => x.AkcijaId == trenutnaAkcija.AkcijaId).ToListAsync();
                    var proizvodiIzKorpe = model.rows.Where(x => x.IsAkcija == true).ToList();
                    foreach (var item in proizvodiIzKorpe)
                    {
                        var akcijskiProizvod = proizvodiNaAkciji.Where(x => x.ProizvodId == item.ProizvodId).SingleOrDefault();
                        var proizvod = model.rows.Where(x => x.ProizvodId == item.ProizvodId).SingleOrDefault();
                        if (akcijskiProizvod != null)
                        {
                            proizvod.Cijena = akcijskiProizvod.AkcijskaCijena.Value;
                        }
                    }

                }
                model.Ukupno = korpa.Ukupno.Value;
                return PartialView("_KorpaPartial", model);
            }
            return NotFound();
        }       
        public string GetDimenziju(int proizvodId)
        {
            Proizvodi proizvod = _db.Proizvodi.Include(x => x.Dimenzija).Where(x => x.ProizvodId == proizvodId).SingleOrDefault();
            
            string dimenzija="";
            if (proizvod != null)
            {
                if (proizvod.Dimenzija.Debljina != null)
                    dimenzija = proizvod.Dimenzija.Duzina + "X" + proizvod.Dimenzija.Sirina + "X" + proizvod.Dimenzija.Debljina + "cm";
                else
                    dimenzija = proizvod.Dimenzija.Duzina + "X" + proizvod.Dimenzija.Sirina + "cm";
            }
            return dimenzija;
        }
        public async Task<ActionResult> UkloniProizvod(int stavkaId)
        {
            KorpaProizvodi stavka = await _db.KorpaProizvodi.Where(x => x.KorpaProizvodId == stavkaId).AsNoTracking().SingleOrDefaultAsync();
            if (stavka != null)
            {
                Korpe korpa = await _db.Korpe.FindAsync(stavka.KorpaId);
                var iznos = stavka.Cijena.Value * stavka.Kolicina;
                korpa.Ukupno -= iznos;
                _db.Korpe.Update(korpa);
                _db.KorpaProizvodi.Remove(stavka);
                await _db.SaveChangesAsync();
            }
           
            return RedirectToAction("KorpaIndex");
        }
        [HttpGet]
        public async Task<JsonResult> PromijeniKolicinu(int stavkaId,decimal kolicina)
        {
            KorpaProizvodi stavka = await _db.KorpaProizvodi.Include(x=>x.Proizvod).Where(x => x.KorpaProizvodId == stavkaId).SingleOrDefaultAsync();
            Korpe korpa = await _db.Korpe.FindAsync(stavka.KorpaId);
            var iznos = stavka.Cijena.Value * stavka.Kolicina;
            korpa.Ukupno -= iznos;
                stavka.Kolicina = kolicina;
                stavka.Cijena = stavka.Proizvod.Cijena;
            var noviIznos = stavka.Kolicina * stavka.Cijena;
            korpa.Ukupno += noviIznos;
            _db.Korpe.Update(korpa);
            _db.KorpaProizvodi.Update(stavka);
            await _db.SaveChangesAsync();
            return Json(new { redirectToUrl = Url.Action("KorpaIndex", "Korpa") });
        }
        public async Task<ActionResult> DodajFavoriteUKorpu()
        {
            int kupacId = HttpContext.GetUserId();
            var favorit = await _db.Favoriti.Where(x => x.KupacId == kupacId).Select(x => x.FavoritId).SingleOrDefaultAsync();
            var listaStavki = await _db.FavoritiStavke.Include(x => x.Proizvod).Include(x => x.Proizvod.ProizvodDetalji).Where(x => x.FavoritId == favorit && x.Proizvod.ProizvodDetalji.IsAktivan == true).ToListAsync();
            Korpe korpa = await _db.Korpe.Where(x => x.KupacId == kupacId).AsNoTracking().SingleOrDefaultAsync();
            if (korpa == null)
            {
                korpa = new Korpe()
                {
                    KupacId = kupacId,
                    Ukupno = 0
                };
                await _db.Korpe.AddAsync(korpa);
                await _db.SaveChangesAsync();
            }

            foreach (var item in listaStavki)
            {
                Proizvodi proizvod = await _db.Proizvodi.FindAsync(item.ProizvodId);
                KorpaProizvodi stavka = await _db.KorpaProizvodi.Where(x => x.KorpaId == korpa.KorpaId && x.ProizvodId == item.ProizvodId).SingleOrDefaultAsync();
                if (stavka == null)
                {
                    stavka = new KorpaProizvodi()
                    {
                        KorpaId = korpa.KorpaId,
                        ProizvodId = item.ProizvodId,
                        Kolicina = 1,
                        Cijena = proizvod.Cijena * 1
                    };
                    await _db.KorpaProizvodi.AddAsync(stavka);
                    await _db.SaveChangesAsync();
                }
                else
                {
                    stavka.Kolicina += 1;
                    stavka.Cijena = proizvod.Cijena * stavka.Kolicina;
                    _db.KorpaProizvodi.Update(stavka);
                    await _db.SaveChangesAsync();
                }
                korpa.Ukupno += 1 * proizvod.Cijena;
                korpa.DatumModifikacije = System.DateTime.Now;
                _db.Korpe.Update(korpa);
                await _db.SaveChangesAsync();

            }
            return RedirectToAction("KorpaIndex");
        }
    }
}
