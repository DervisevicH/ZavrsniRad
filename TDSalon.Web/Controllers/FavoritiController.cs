using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TDSalon.Data;
using TDSalon.Web.Helper;
using TDSalon.Web.Models;

namespace TDSalon.Web.Controllers
{
    public class FavoritiController : Controller
    {
        private readonly TDSalondbContext _db;
        public FavoritiController(TDSalondbContext db) { _db = db; }
        [Authorize(Roles = "Kupac")]

        public async Task<IActionResult> Index()
        {
            int kupacId = HttpContext.GetUserId();
            FavoritiVM model = new FavoritiVM();
            model.FavoritId = await _db.Favoriti.Where(x => x.KupacId == kupacId).Select(x => x.FavoritId).SingleOrDefaultAsync();
            var listaStavki = await _db.FavoritiStavke.Include(x => x.Proizvod).Include(x=>x.Proizvod.ProizvodDetalji).Where(x => x.FavoritId == model.FavoritId && x.Proizvod.ProizvodDetalji.IsAktivan == true).ToListAsync();
            model.Rows = listaStavki.Select(x => new FavoritiVM.Row
            {
                ProizvodId = x.ProizvodId.Value,
                FavoritProizvodId = x.FavoritStavkaId,
                Proizvod = x.Proizvod.ProizvodDetalji.Naziv,
                Cijena = x.Proizvod.Cijena.Value,
                Slika = _db.Slike.Where(y => y.ProizvodDetaljiId == x.Proizvod.ProizvodDetaljiId).Select(s=>s.SlikaUrl).FirstOrDefault(),
                IsAkcija = x.Proizvod.IsAkcija.Value
            }).ToList();
            var proizvodi = model.Rows.Where(x => x.IsAkcija == true).ToList();
            var trenutnaAkcija = await _db.Akcije.Where(x => x.IsAktivna == true).SingleOrDefaultAsync();
            if (trenutnaAkcija != null)
            {
                if (trenutnaAkcija.DatumDo < System.DateTime.Now)
                {
                    trenutnaAkcija.IsAktivna = false;
                    var proizvodiNaAkciji = await _db.AkcijeProizvodi.Where(x => x.AkcijaId == trenutnaAkcija.AkcijaId).ToListAsync();
                    foreach (var proizvod in proizvodiNaAkciji)
                    {
                        var proizvodDb = await _db.Proizvodi.Where(x => x.ProizvodId == proizvod.ProizvodId).SingleOrDefaultAsync();
                        proizvodDb.IsAkcija = false;
                        _db.Proizvodi.Update(proizvodDb);
                    }
                    await _db.SaveChangesAsync();
                    trenutnaAkcija = null;
                }
            }
            if (trenutnaAkcija != null)
            {
                var proizvodiNaAkciji = await _db.AkcijeProizvodi.Where(x => x.AkcijaId == trenutnaAkcija.AkcijaId).ToListAsync();

                foreach (var item in proizvodi)
                {
                    var akcijskiProizvod = proizvodiNaAkciji.Where(x => x.ProizvodId == item.ProizvodId).SingleOrDefault();
                    var proizvod = model.Rows.Where(x => x.ProizvodId == item.ProizvodId).SingleOrDefault();
                    if (akcijskiProizvod != null)
                    {
                        proizvod.Akcija = (int)trenutnaAkcija.Postotak;
                        proizvod.AkcijskaCijena = akcijskiProizvod.AkcijskaCijena.Value;
                    }
                }

            }
            return View(model);
        }
        [Authorize(Roles = "Kupac")]
        public async Task<ActionResult> Dodaj(int proizvodId)
        {
            int korisnikId = HttpContext.GetUserId();
            Favoriti favorit = await _db.Favoriti.Where(x => x.KupacId == korisnikId).SingleOrDefaultAsync();
            if (favorit == null)
            {
                favorit = new Favoriti() { KupacId = korisnikId };
                await _db.Favoriti.AddAsync(favorit);
                await _db.SaveChangesAsync();
            }
            FavoritiStavke stavka = new FavoritiStavke()
            {
                DatumDodavanja = System.DateTime.Now,
                FavoritId = favorit.FavoritId,
                ProizvodId = proizvodId
            };
            await _db.FavoritiStavke.AddAsync(stavka);
            await _db.SaveChangesAsync();

            return Ok();
        }

        [Authorize(Roles = "Kupac")]
        public async Task<ActionResult> IzbrisiIzFavorita(int favoritId)
        {
            if (favoritId != 0) {
                FavoritiStavke stavkaDb = await _db.FavoritiStavke.FindAsync(favoritId);
                if (stavkaDb != null)
                {
                    _db.FavoritiStavke.Remove(stavkaDb);
                    await _db.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Uspješno ste obrisali proizvod iz favorita!";
                }
            }
          
            return RedirectToAction("Index");
        }
    }
}
