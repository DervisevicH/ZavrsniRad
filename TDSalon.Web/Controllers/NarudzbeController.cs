using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rotativa;
using Rotativa.AspNetCore;
using TDSalon.Data;
using TDSalon.Web.Helper;
using TDSalon.Web.Models;

namespace TDSalon.Web.Controllers
{
    public class NarudzbeController : Controller
    {
        private readonly TDSalondbContext _db;
        IMapper _mapper;
        public NarudzbeController(TDSalondbContext db, IMapper mapper) { _db = db; _mapper = mapper; }

        [HttpGet]
        [Authorize(Roles = "Zaposlenik")]
        public async Task<ActionResult> Index(string brojNarudzbe, DateTime? datumOd, DateTime? datumDo, string status, string statusDostave, int rok, string kupac)
        {
            
            var listaNarudzbi = await _db.Narudzbe.Include("Kupac").AsNoTracking().ToListAsync();
            
            if(String.Equals(status,"zavrsene"))
               listaNarudzbi = listaNarudzbi.Where(x => x.Procesirana == true).ToList();           

            if (!String.IsNullOrEmpty(brojNarudzbe))
            {
                listaNarudzbi = listaNarudzbi.Where(x => x.BrojNarudzbe.ToString().Contains(brojNarudzbe)).ToList();
            }
            if (datumOd != null)
            {
                listaNarudzbi = listaNarudzbi.Where(x => x.Datum > datumOd).ToList();

            }
            if(datumDo != null)
            {
                listaNarudzbi = listaNarudzbi.Where(x => x.Datum < datumDo).ToList();
            }
            if (!String.IsNullOrEmpty(statusDostave))
            {
                listaNarudzbi = listaNarudzbi.Where(x => x.StatusNarudzbe == statusDostave).ToList();

            }
            if (!String.IsNullOrEmpty(kupac))
            {
                listaNarudzbi = listaNarudzbi.Where(x => x.Kupac.Ime.ToLower().Contains(kupac) || x.Kupac.Prezime.ToLower().Contains(kupac)).ToList();
            }
            if (rok != 0)
            {
                listaNarudzbi = listaNarudzbi.Where(x => x.RokZaDostavu==rok).ToList();
            }
            List<NarudzbeIndexVM> model = _mapper.Map<List<NarudzbeIndexVM>>(listaNarudzbi);

            return View(model);
        }
        [Authorize(Roles = "Kupac")]
        public async Task<ActionResult> DodajNarudzbu()
        {
            var model = await NapraviNarudzbu();
            return View("DodajNarudzbu", model);
        }
        [HttpGet]
        [Authorize(Roles = "Kupac")]
        public ActionResult GetNarudzbeByKorisnik()
        {
            NarudzbeKorisnikVM model = new NarudzbeKorisnikVM();
            int korisnikId = HttpContext.GetUserId();
            model.rows = _db.Narudzbe.Where(x => x.KupacId == korisnikId).Select(y => new NarudzbeKorisnikVM.Row
            {
                BrojNarudzbe = y.BrojNarudzbe.Value,
                Datum = y.Datum.Value,
                NarudzbaId = y.NarudzbaId,
                Status = y.StatusNarudzbe,
                Ukupno = y.Ukupno.Value
            }).ToList();
            return View("NarudzbaByKorisnik", model);
        }
        [HttpPost]
        public async Task<ActionResult> SacuvajNarudzbu(string napomena, int brojDana)
        {
            int logiraniKupac = HttpContext.GetUserId();
            var model = await NapraviNarudzbu();
            Narudzbe novaNarudzba = new Narudzbe()
            {
                KupacId = logiraniKupac,
                BrojNarudzbe = 123,
                Datum = System.DateTime.Now,
                Napomena = napomena,
                Procesirana = false,
                RokZaDostavu = brojDana,
                Ukupno=model.Ukupno,                
                TroskoviDostave=model.TroskoviDostave,
                Otkazano = false
            };
            await _db.Narudzbe.AddAsync(novaNarudzba);
            await _db.SaveChangesAsync();

            foreach (var item in model.rows)
            {
                NarudzbaStavke novaStavka = new NarudzbaStavke
                {
                    NarudzbaId = novaNarudzba.NarudzbaId,
                    ProizvodId = item.ProizvodId,
                    Kolicina = item.Kolicina,
                    Cijena = item.Cijena
                };
                await _db.NarudzbaStavke.AddAsync(novaStavka);
            }
            await _db.SaveChangesAsync();
            TempData["SuccessMsg"] = "Vaša narudžba je poslana";

            return RedirectToAction("GetNarudzbeByKorisnik");
        }
        [HttpGet]
        [Authorize(Roles = "Zaposlenik")]
        public async Task<ActionResult> Uredi(int narudzbaId)
        {
            Narudzbe narudzba = await  _db.Narudzbe.Include("Kupac").Where(x => x.NarudzbaId == narudzbaId).SingleAsync();
            List<NarudzbaStavke> stavke = await _db.NarudzbaStavke.Include("Proizvod").Include("Proizvod.ProizvodDetalji").Where(x => x.NarudzbaId == narudzba.NarudzbaId).AsNoTracking().ToListAsync();
            NarudzbaDetaljiVM model = new NarudzbaDetaljiVM();            
            model = _mapper.Map<NarudzbaDetaljiVM>(narudzba);
            model.listaStavki = new List<NarudzbaDetaljiVM.Stavke>();
            model.UkupnoZaPlatit = narudzba.Ukupno.Value;
            if (narudzba.TroskoviDostave.HasValue)
                model.Medusuma = narudzba.Ukupno.Value - narudzba.TroskoviDostave.Value;
            else
                model.Medusuma = narudzba.Ukupno.Value;
            foreach (var item in stavke)
            {
                NarudzbaDetaljiVM.Stavke novaStavka = new NarudzbaDetaljiVM.Stavke();
                novaStavka.StavkaId = item.StavkaId;
                novaStavka.Sifra = item.Proizvod.Sifra;
                novaStavka.Proizvod = item.Proizvod.ProizvodDetalji.Naziv;
                novaStavka.Kolicina = item.Kolicina.Value;
                if (item.Popust.HasValue)
                    novaStavka.Popust = item.Popust.Value;
                novaStavka.Cijena = item.Cijena.Value;
                novaStavka.Ukupno = item.Cijena.Value * Convert.ToDecimal(item.Kolicina);

                int dimenzijaId = item.Proizvod.DimenzijaId.Value;
                Dimenzije dim = _db.Dimenzije.Where(x => x.DimenzijaId == dimenzijaId).Single();
                if (dim.Debljina != null)
                    novaStavka.Dimenzija = dim.Sirina + "cmX" + dim.Duzina + "cmX" + dim.Debljina + "cm";
                else
                    novaStavka.Dimenzija = dim.Sirina + "cmX" + dim.Duzina + "cm";

                model.listaStavki.Add(novaStavka);
            }       
                
            return View("Detalji", model);
        }
        [HttpPost]
        public ActionResult ProcesirajNarudzbu(NarudzbaDetaljiVM model)
        {
            Narudzbe narudzba = _db.Narudzbe.Find(model.NarudzbaId);
            narudzba.StatusNarudzbe = model.Status;
            narudzba.Komentar = model.Komentar;

            _db.Narudzbe.Update(narudzba);
            _db.SaveChanges();

            return RedirectToAction("Uredi", new { narudzbaId = narudzba.NarudzbaId });
        }
        public string GetDimenziju(int proizvodId)
        {
            Proizvodi proizvod = _db.Proizvodi.Include(x => x.Dimenzija).Where(x => x.ProizvodId == proizvodId).SingleOrDefault();

            string dimenzija = "";
            if (proizvod != null)
            {
                if (proizvod.Dimenzija.Debljina != null)
                    dimenzija = proizvod.Dimenzija.Duzina + "X" + proizvod.Dimenzija.Sirina + "X" + proizvod.Dimenzija.Debljina + "cm";
                else
                    dimenzija = proizvod.Dimenzija.Duzina + "X" + proizvod.Dimenzija.Sirina + "cm";
            }
            return dimenzija;
        }
        private async Task<NarudzbaDodajVM> NapraviNarudzbu() {
            int korisnikId = HttpContext.GetUserId(); ;
            Kupci kupac = await _db.Kupci.Include(x => x.Grad).Where(x => x.KupacId == korisnikId).SingleOrDefaultAsync();
            Korpe korpa = await _db.Korpe.Where(x => x.KupacId == korisnikId).SingleOrDefaultAsync();
            var dostava = 0;
            var kanton = await _db.Kantoni.Where(x => x.KantonId == kupac.Grad.KantonId).SingleOrDefaultAsync();
            if (kanton.KantonId != 3)
            {
                dostava = 10;
            }
            NarudzbaDodajVM model = new NarudzbaDodajVM()
            {
                KorpaId = korpa.KorpaId,
                TroskoviDostave = dostava,
                Ukupno = korpa.Ukupno.Value,
                rows = new List<NarudzbaDodajVM.Row>(),
                Kupac = kupac
            };
            List<KorpaProizvodi> listaProizvoda = _db.KorpaProizvodi.Include(x => x.Proizvod).Where(x => x.KorpaId == korpa.KorpaId).ToList();
            foreach (var item in listaProizvoda)
            {
                NarudzbaDodajVM.Row row = new NarudzbaDodajVM.Row()
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
            var trenutnaAkcija = await _db.Akcije.Where(x => x.IsAktivna == true).SingleOrDefaultAsync();
            if (trenutnaAkcija != null)
            {
                var proizvodiNaAkciji = await _db.AkcijeProizvodi.Where(x => x.AkcijaId == trenutnaAkcija.AkcijaId).ToListAsync();
                var proizvodiIzKorpe = model.rows.Where(x => x.IsAkcija == true).ToList();
                foreach (var item in proizvodiIzKorpe)
                {
                    var akcijskiProizvod = proizvodiNaAkciji.Where(x => x.ProizvodId == item.ProizvodId).FirstOrDefault();
                    var proizvod = model.rows.Where(x => x.ProizvodId == item.ProizvodId).FirstOrDefault();
                    if (akcijskiProizvod != null)
                    {
                        proizvod.Cijena = akcijskiProizvod.AkcijskaCijena.Value;
                    }
                }

            }
            decimal ukupno = 0;
            foreach (var item in model.rows)
            {
                ukupno += item.Cijena;
            }
            if (ukupno != model.Ukupno)
                model.Ukupno = ukupno;
            return model;
        }

    }
}
