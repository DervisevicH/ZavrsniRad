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
using Microsoft.EntityFrameworkCore;
using PagedList;
using TDSalon.Data;
using TDSalon.Web.Helper;
using TDSalon.Web.Models;

namespace TDSalon.Web.Controllers
{
    public class ProizvodiController : Controller
    {
        TDSalondbContext _db;
        IMapper _mapper;
        public ProizvodiController(TDSalondbContext db, IMapper mapper) { _db = db; _mapper = mapper; }
        [Authorize(Roles = "Zaposlenik")]
        [HttpGet]
        public async Task<ActionResult> IndexAdmin(int? kategorijaId, string pretraga, bool isAktivan = true)
        {
            ProizvodiIndexVM model = new ProizvodiIndexVM();
            model.IsAktivan = isAktivan;
            model.ListaKategorija = await _db.Kategorije.Select(x => new SelectListItem { Value = x.KategorijaId.ToString(), Text = x.Naziv }).ToListAsync();
            var listaProizvoda = _db.Proizvodi.Include(x => x.ProizvodDetalji.Kategorija).AsQueryable();
            if (kategorijaId.HasValue)
            {
                listaProizvoda = listaProizvoda.Where(x => x.ProizvodDetalji.KategorijaId == kategorijaId);
                model.KategorijaId = kategorijaId.Value;
            }
            if (!String.IsNullOrEmpty(pretraga))
            {
                listaProizvoda = listaProizvoda.Where(x => x.ProizvodDetalji.Naziv.Contains(pretraga));
            }

            listaProizvoda = listaProizvoda.Where(x => x.ProizvodDetalji.IsAktivan == isAktivan);
            var tempList = listaProizvoda.ToList();

            model.ListaProizvodi = tempList.Select(x => new ProizvodiIndexVM.ProizvodiVM
            {
                ProizvodId = x.ProizvodId,
                Sifra = x.Sifra,
                Naziv = x.ProizvodDetalji.Naziv,
                Cijena = x.Cijena.ToString(),
                Kolicina = x.Stanje.ToString(),
                Kategorija = x.ProizvodDetalji.Kategorija.Naziv,
                IsAktivan = x.ProizvodDetalji.IsAktivan.Value,
                IsAkcija = x.IsAkcija
            }).ToList();
           
            return View("IndexAdmin", model);
        }
        [HttpGet]
        public async Task<ActionResult> Shop(string pretraga, string preporucenoZa, string dimenzijaId, string boja, string cijenaOd, string cijenaDo, int kategorijaId = 1)
        {
            ProizvodiVM model = new ProizvodiVM();
            model.Rows = new List<ProizvodRow>();
            
            var listaProizvoda = _db.Proizvodi.Include(x => x.ProizvodDetalji).Where(x => x.ProizvodDetalji.KategorijaId == kategorijaId && x.ProizvodDetalji.IsAktivan == true).AsQueryable();
            
            if (!String.IsNullOrEmpty(pretraga))
            {
                listaProizvoda = listaProizvoda.Where(x => x.ProizvodDetalji.Naziv.StartsWith(pretraga));
            }
            if (!String.IsNullOrEmpty(preporucenoZa))
            {
                listaProizvoda = listaProizvoda.Where(x => x.ProizvodDetalji.PreporucenoZa.StartsWith(preporucenoZa));
            }
            if (!String.IsNullOrEmpty(boja))
            {
                listaProizvoda = listaProizvoda.Where(x => x.ProizvodDetalji.Boja.StartsWith(boja));
            }
            if (!String.IsNullOrEmpty(cijenaOd) && !String.IsNullOrEmpty(cijenaDo))
            {
                decimal Od = Convert.ToDecimal(cijenaOd);
                decimal Do = Convert.ToDecimal(cijenaDo);
                listaProizvoda = listaProizvoda.Where(x => x.Cijena >= Od && x.Cijena <= Do);
            }
            if (!String.IsNullOrEmpty(dimenzijaId))
            {
                int dimenzija = Convert.ToInt32(dimenzijaId);
                listaProizvoda = listaProizvoda.Where(x => x.DimenzijaId == dimenzija);
            }
            if (!String.IsNullOrEmpty(cijenaOd) && String.IsNullOrEmpty(cijenaDo))
            {
                decimal Od = Convert.ToDecimal(cijenaOd);
                listaProizvoda = listaProizvoda.Where(x => x.Cijena >= Od);
            }
            if (String.IsNullOrEmpty(cijenaOd) && !String.IsNullOrEmpty(cijenaDo))
            {
                decimal Do = Convert.ToDecimal(cijenaDo);
                listaProizvoda = listaProizvoda.Where(x => x.Cijena <= Do);

            }
            var lista = listaProizvoda.ToList();
            foreach (var item in lista)
            {
                ProizvodRow row = new ProizvodRow()
                {
                    Cijena = item.Cijena.ToString(),
                    ProizvodId = item.ProizvodId,
                    Naziv = item.ProizvodDetalji.Naziv,
                    SlikaUrl = _db.Slike.Where(x => x.ProizvodDetaljiId == item.ProizvodDetaljiId).Select(x => x.SlikaUrl).FirstOrDefault(),                    
                    IsAkcija = item.IsAkcija.Value
                };
                model.Rows.Add(row);
            }
            var proizvodi = model.Rows.Where(x => x.IsAkcija == true).ToList();
            var trenutnaAkcija = await _db.Akcije.Where(x => x.IsAktivna == true).SingleOrDefaultAsync();
            if (trenutnaAkcija.DatumDo < System.DateTime.Now)
            {
                trenutnaAkcija.IsAktivna = false;
                var proizvodiNaAkciji = await _db.AkcijeProizvodi.Where(x=>x.AkcijaId== trenutnaAkcija.AkcijaId).ToListAsync();
                foreach (var proizvod in proizvodiNaAkciji)
                {
                    var proizvodDb = await _db.Proizvodi.Where(x=>x.ProizvodId == proizvod.ProizvodId).SingleOrDefaultAsync();
                    proizvodDb.IsAkcija = false;
                    _db.Proizvodi.Update(proizvodDb);
                }
                await _db.SaveChangesAsync();
            }
            if (trenutnaAkcija != null)
            {
                var proizvodiNaAkciji = await _db.AkcijeProizvodi.Where(x=>x.AkcijaId== trenutnaAkcija.AkcijaId).ToListAsync();

                foreach (var item in proizvodi)
                {
                    var akcijskiProizvod = proizvodiNaAkciji.Where(x => x.ProizvodId == item.ProizvodId).SingleOrDefault();
                    var proizvod = model.Rows.Where(x => x.ProizvodId == item.ProizvodId).SingleOrDefault();
                    if (akcijskiProizvod != null)
                    {
                        proizvod.Akcija = (int)trenutnaAkcija.Postotak;
                        proizvod.AkcijskaCijena = akcijskiProizvod.AkcijskaCijena.ToString();
                    }
                }

            }
            model.ListaDimenzija = ListaZaDropDown(kategorijaId, 0);
            return View(model);
       
        }
        [HttpGet]
        public async Task<ActionResult> ShopAkcije(string pretraga, string preporucenoZa, string dimenzijaId, string boja, string cijenaOd, string cijenaDo, int kategorijaId = 1)
        {
            ProizvodiVM model = new ProizvodiVM();
            model.Rows = new List<ProizvodRow>();
            var listaProizvoda = _db.Proizvodi.Include(x => x.ProizvodDetalji).Where(x => x.ProizvodDetalji.KategorijaId == kategorijaId && x.ProizvodDetalji.IsAktivan ==true  && x.IsAkcija ==true).AsQueryable();
            foreach (var item in listaProizvoda)
            {
                ProizvodRow row = new ProizvodRow()
                {
                    Cijena = item.Cijena.ToString(),
                    ProizvodId = item.ProizvodId,
                    Naziv = item.ProizvodDetalji.Naziv,
                    SlikaUrl = await _db.Slike.Where(x => x.ProizvodDetaljiId == item.ProizvodDetaljiId).Select(x => x.SlikaUrl).FirstOrDefaultAsync(),
                    IsAkcija = item.IsAkcija
                };
                model.Rows.Add(row);
            }

            var proizvodi = model.Rows.Where(x => x.IsAkcija == true).ToList();
            var trenutnaAkcija = await _db.Akcije.Where(x => x.IsAktivna == true).SingleOrDefaultAsync();
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
                        proizvod.AkcijskaCijena = akcijskiProizvod.AkcijskaCijena.ToString();
                    }
                }

            }
            model.ListaKategorija = await _db.Kategorije.Select(x => new SelectListItem { Value = x.KategorijaId.ToString(), Text = x.Naziv }).ToListAsync();
            model.ListaDimenzija = ListaZaDropDown(kategorijaId, 0);
            return View("ShopAkcije", model);
        }
        [HttpGet]
        public ActionResult ProizvodDetalji(int proizvodId)
        {
            Proizvodi proizvod = _db.Proizvodi.Include(x=>x.ProizvodDetalji).Include(x=>x.ProizvodDetalji.Kategorija).Where(x => x.ProizvodId == proizvodId).SingleOrDefault();
            ProizvodDetaljiVM model = new ProizvodDetaljiVM() 
            {
                ProizvodId=proizvod.ProizvodId,
                ProizvodDetaljiId=proizvod.ProizvodDetalji.ProizvodDetaljiId,
                Naziv=proizvod.ProizvodDetalji.Naziv,
                Kategorija=proizvod.ProizvodDetalji.Kategorija.Naziv,
                Cijena=proizvod.Cijena.ToString(),
                Boja=proizvod.ProizvodDetalji.Boja,
                Slike=_db.Slike.Where(x=>x.ProizvodDetaljiId==proizvod.ProizvodDetaljiId).Select(x=>x.SlikaUrl).ToList(),
                ListaDimenzija=ListaZaDropDown(0, proizvod.ProizvodDetaljiId),
                BrojKomentara=_db.Komentari.Where(x=>x.ProizvodDetaljiId==proizvod.ProizvodDetaljiId).Count(),
                Opis=proizvod.ProizvodDetalji.Opis,
                PreporucenoZa=proizvod.ProizvodDetalji.PreporucenoZa,
                ZemljaPorijekla=proizvod.ProizvodDetalji.NapravljenoU
            };
            return View(model);
        }
        [HttpGet]
        public async Task<ActionResult> Uredi(int id)
        {
            Proizvodi proizvod =await _db.Proizvodi.Include(x => x.ProizvodDetalji).Where(x => x.ProizvodId == id).SingleOrDefaultAsync();
            if (proizvod != null)
            {
                ProizvodiUrediVM model = new ProizvodiUrediVM()
                {
                    ProizvodId = proizvod.ProizvodId,
                    Boja = proizvod.ProizvodDetalji.Boja,
                    Naziv = proizvod.ProizvodDetalji.Naziv,
                    Opis = proizvod.ProizvodDetalji.Opis,
                    Kolicina = proizvod.Stanje.Value,
                    Sifra = proizvod.Sifra,
                    Cijena = proizvod.Cijena.Value,
                    DimenzijaId = proizvod.DimenzijaId,
                    KategorijaId = proizvod.ProizvodDetalji.KategorijaId.Value,
                    DimenzijeLista = ListaZaDropDown(proizvod.ProizvodDetalji.KategorijaId,0),                  
                    NapravljenoU=proizvod.ProizvodDetalji.NapravljenoU,
                    slike=_db.Slike.Where(x=>x.ProizvodDetaljiId==proizvod.ProizvodDetaljiId).ToList()
                };
                return View(model);
            }
            return NotFound();
        }
        [HttpPost]
        public ActionResult Sacuvaj(ProizvodiUrediVM model)
        {
            if (ModelState.IsValid)
            {
                Proizvodi proizvod = _db.Proizvodi.Where(x => x.ProizvodId == model.ProizvodId).SingleOrDefault();
                ProizvodiDetalji detalji = _db.ProizvodiDetalji.Where(x => x.ProizvodDetaljiId == proizvod.ProizvodDetaljiId).SingleOrDefault(); 
                if (proizvod != null)
                {
                    proizvod.Sifra = model.Sifra;
                    proizvod.DimenzijaId = model.DimenzijaId;
                    proizvod.Cijena = model.Cijena;
                    proizvod.Stanje = model.Kolicina;

                    _db.Proizvodi.Update(proizvod);

                    detalji.Naziv = model.Naziv;
                    detalji.Opis = model.Opis;
                    detalji.NapravljenoU = model.NapravljenoU;
                    detalji.Boja = model.Boja;
                    detalji.DatumIzmjene = System.DateTime.Now;

                    _db.ProizvodiDetalji.Update(detalji);

                    _db.SaveChanges();
                    return RedirectToAction("IndexAdmin");
                }
            }
           
                model.DimenzijeLista = ListaZaDropDown(model.KategorijaId,0);
                return View("Uredi", model);
            
        }
        [HttpGet]
        public async Task<ActionResult> DodajInfo()
        {
            ProizvodiDodajVM model=new ProizvodiDodajVM();            
            ProizvodiDodajVM temp = HttpContext.Session.Get<ProizvodiDodajVM>("NoviProizvod");
            if (temp != null)
            {
                model = temp;
            }           
           
            PopuniListe(model);
            return View("DodajInfo", model);
        }
        [HttpPost]
        public async Task<ActionResult> SacuvajInfo(ProizvodiDodajVM model)
        {
            if (ModelState.IsValid)
            {
                ProizvodiDodajVM temp = HttpContext.Session.Get<ProizvodiDodajVM>("NoviProizvod");
                if (temp == null)
                {
                    temp = new ProizvodiDodajVM();
                }
                temp = model;
                HttpContext.Session.Set("NoviProizvod", temp);

                return RedirectToAction("DodajCijene", new { kategorijaId = model.KategorijaId });
            }
            else
            {
                PopuniListe(model);
                return View("DodajInfo", model);
            }
        }
        [HttpGet]
        public async Task<ActionResult> DodajCijene(int? kategorijaId)
        {
            ProizvodiCijeneVM model;
            ProizvodiDodajVM obj = HttpContext.Session.Get<ProizvodiDodajVM>("NoviProizvod");
            if (obj.ProizvodiCijene != null)
            {
                model = obj.ProizvodiCijene;
                model.RowsDetalji = new List<RowsDetalji>();
                model.RowsDetalji = obj.ProizvodiCijene.RowsDetalji;
                model.KategorijaId = obj.KategorijaId.Value;
            }
            else
            {
                model = new ProizvodiCijeneVM();
                model.KategorijaId = kategorijaId.Value;
                PopuniDimenzije(model);
            }

            return View("DodajCijene", model);
        }
        [HttpPost]
        public async Task<ActionResult> SacuvajCijene(ProizvodiCijeneVM model)
        {
            if (model.RowsDetalji.Any(x => x.Cijena > 0))
            {
                foreach (var item in model.RowsDetalji)
                {
                    decimal cijena;
                    if (!decimal.TryParse(item.Cijena.ToString(), out cijena))
                    {
                        PopuniDimenzije(model);
                        TempData["greska_poruka"] = "Niste unijeli validnu cijenu";
                        return View("DodajCijene", model);
                    }
                    if ((cijena == 0 || item.Cijena == null) && String.IsNullOrWhiteSpace(item.Sifra) && (item.Stanje == 0 || item.Stanje == null))
                    {
                        continue;
                    }
                    if (cijena > 0)
                    {
                        if (item.Stanje == 0 || item.Stanje == null)
                        {
                            PopuniDimenzije(model);
                            TempData["greska_poruka"] = "Stanje proizvoda ne moze biti 0";
                            return View("DodajCijene", model);
                        }
                        if (String.IsNullOrWhiteSpace(item.Sifra))
                        {
                            PopuniDimenzije(model);
                            TempData["greska_poruka"] = "Niste unijeli sifru";
                            return View("DodajCijene", model);
                        }
                    }
                }
                ProizvodiDodajVM obj = HttpContext.Session.Get<ProizvodiDodajVM>("NoviProizvod");
                obj.ProizvodiCijene = new ProizvodiCijeneVM();
                obj.ProizvodiCijene.KategorijaId = model.KategorijaId;
                obj.ProizvodiCijene.RowsDetalji = new List<RowsDetalji>();

                foreach (var item in model.RowsDetalji)
                {
                    if ((item.Cijena != 0 || item.Cijena != null) && !String.IsNullOrWhiteSpace(item.Sifra) && (item.Stanje != 0 || item.Stanje != null))
                    {
                        RowsDetalji row = new RowsDetalji
                        {
                            DimenzijaId = item.DimenzijaId,
                            Cijena = item.Cijena,
                            Sifra = item.Sifra,
                            Stanje = item.Stanje
                        };
                        obj.ProizvodiCijene.RowsDetalji.Add(row);
                    }
                }           
                HttpContext.Session.Set("NoviProizvod", obj);
                return RedirectToAction("DodajSlike");
            }
            else
            {
                TempData["greska_poruka"] = "Niste unijeli validnu cijenu";
                PopuniDimenzije(model);
                return View("DodajCijene", model);
            }
        }        
        [HttpGet]
        public async Task<ActionResult> DodajSlike()
        {
            ProizvodiSlikeVM model = new ProizvodiSlikeVM();
           
            return View("DodajSlike", model);
        }
        [HttpPost]
        public async Task<ActionResult> SacuvajProizvod(ProizvodiSlikeVM model)
        {
            if (model.slike == null || model.slike.Length == 0)
            {
                ModelState.AddModelError("slike", "Odaberite sliku");
                return View("SlikeProizvodiDodaj", model);
            }
            else
            {
                ProizvodiDodajVM proizvod = HttpContext.Session.Get<ProizvodiDodajVM>("NoviProizvod");

                //Osnovne informacije o proizvodu                
                ProizvodiDetalji info = new ProizvodiDetalji();
                info.Naziv = proizvod.Naziv;
                info.Opis = proizvod.Opis;
                info.Boja = proizvod.Boja;
                info.DatumIzmjene = DateTime.Now;
                info.NapravljenoU = proizvod.NapravljenoU;
                info.KategorijaId = proizvod.KategorijaId;
                info.DobavljacId = proizvod.DobavljacId;
                info.PreporucenoZa = proizvod.PreporucenoZa;

                _db.ProizvodiDetalji.Add(info);
                _db.SaveChanges();

                //Proizvodi

                foreach (var item in proizvod.ProizvodiCijene.RowsDetalji)
                {
                    Proizvodi noviProizvod = new Proizvodi();
                    noviProizvod.DimenzijaId = item.DimenzijaId;
                    noviProizvod.Cijena = item.Cijena;
                    noviProizvod.ProizvodDetaljiId = info.ProizvodDetaljiId;
                    noviProizvod.Sifra = item.Sifra;
                    noviProizvod.Stanje = item.Stanje;
                    noviProizvod.IsAkcija = false;
                    _db.Proizvodi.Add(noviProizvod);
                    _db.SaveChanges();
                }

                //Slike proizvoda
                foreach (IFormFile item in model.slike)
                {
                    var fileName = Path.GetFileName(item.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\uploads\\products", fileName);
                    using (var fileSteam = new FileStream(filePath, FileMode.Create))
                    {
                        item.CopyTo(fileSteam);
                    }
                    Slike novaSlika = new Slike();
                    novaSlika.ProizvodDetaljiId = info.ProizvodDetaljiId;
                    novaSlika.SlikaUrl = fileName;

                    await _db.Slike.AddAsync(novaSlika);
                    await _db.SaveChangesAsync();
                }
                HttpContext.Session.Remove("NoviProizvod");
                return RedirectToAction("IndexAdmin");
            }
        }

        [HttpPost]
        public async Task<ActionResult> SacuvajIzmjenu(ProizvodiUrediVM model)
        {
            if (ModelState.IsValid)
            {
                Proizvodi proizvodDb =await _db.Proizvodi.FindAsync(model.ProizvodId);
                ProizvodiDetalji detaljiDb =await _db.ProizvodiDetalji.FindAsync(proizvodDb.ProizvodDetaljiId);

                _db.Proizvodi.Attach(proizvodDb);
                _db.ProizvodiDetalji.Attach(detaljiDb);
                _db.Proizvodi.Update(proizvodDb);
                _db.ProizvodiDetalji.Update(detaljiDb);

                _mapper.Map(model,proizvodDb);
                _mapper.Map(model,detaljiDb);
                await _db.SaveChangesAsync();

                TempData["poruka"] = "Uspješno ste izmijenili podatke!";
                return RedirectToAction("Uredi",new { id=proizvodDb.ProizvodId });
            }
            
            return Ok();
        }
        public void PopuniListe(ProizvodiDodajVM model)
            {
                model.DobavljaciLista = _db.Dobavljaci.Select(x => new SelectListItem
                {
                    Value = x.DobavljacId.ToString(),
                    Text = x.NazivFirme
                }).ToList();
                model.JedinicaMjereLista = _db.JediniceMjere.Select(x => new SelectListItem
                {
                    Value = x.JedinicaMjereId.ToString(),
                    Text = x.Naziv
                }).ToList();
                model.KategorijeLista = _db.Kategorije.Select(x => new SelectListItem
                {
                    Value = x.KategorijaId.ToString(),
                    Text = x.Naziv
                }).ToList();

            }
        public void PopuniDimenzije(ProizvodiCijeneVM model)
            {
                var dimenzije = _db.Dimenzije.Where(x => x.KategorijaId.Value == model.KategorijaId).ToList();

                model.RowsDetalji = new List<RowsDetalji>();
                if (dimenzije.Count() != 0)
                {
                    foreach (var item in dimenzije)
                    {
                        RowsDetalji row = new RowsDetalji();
                        row.DimenzijaId = item.DimenzijaId;
                        if (item.Debljina != null)
                            row.Dimenzija = item.Duzina + "X" + item.Sirina + "X" + item.Debljina + "cm";
                        else
                            row.Dimenzija = item.Duzina + "X" + item.Sirina + "cm";
                        row.Cijena = 0;
                        model.RowsDetalji.Add(row);
                    }
                }

            }
        public List<SelectListItem> ListaZaDropDown(int? kategorijaId,int ?detaljiId)
        {
            List<Dimenzije> dimenzije=new List<Dimenzije>();
            if (kategorijaId != 0)
            {
                dimenzije=_db.Dimenzije.Where(x=>x.KategorijaId==kategorijaId).ToList();
            }
            if(detaljiId != 0)
            {
                dimenzije = _db.Proizvodi.Where(x => x.ProizvodDetaljiId == detaljiId).Include(x=>x.Dimenzija).Select(x => x.Dimenzija).ToList();
            }
           
            List<SelectListItem> list = new List<SelectListItem>();
            foreach (var item in dimenzije)
            {
                SelectListItem row = new SelectListItem();
                row.Value = item.DimenzijaId.ToString();
                if (item.Debljina != null)
                    row.Text = item.Duzina + "X" + item.Sirina + "X" + item.Debljina + "cm";
                else
                    row.Text = item.Duzina + "X" + item.Sirina + "cm";
                list.Add(row);
            }
            return list;
        }
        public DimenzijaCijenaVM PromijeniCijenu(int dimenzijaId, int proizvodDetaljiId)
        {
            var proizvod = _db.Proizvodi.Where(x => x.DimenzijaId == dimenzijaId && x.ProizvodDetaljiId == proizvodDetaljiId).SingleOrDefault();
            if (proizvod != null)
            {
                DimenzijaCijenaVM model = new DimenzijaCijenaVM()
                {
                    Cijena = proizvod.Cijena.ToString(),
                    ProizvodId = proizvod.ProizvodId
                };
                return model;
            }
            return null;
        }
        public JsonResult GetProizvod(int proizvodId,decimal postotak)
        {            
            var proizvod = _db.Proizvodi.Include(x => x.ProizvodDetalji).Where(x => x.ProizvodId == proizvodId).SingleOrDefault();
            if (proizvod != null)
            {
                AkcijeProizvodiVM model = new AkcijeProizvodiVM()
                {
                    Naziv = proizvod.ProizvodDetalji.Naziv,
                    Sifra = proizvod.Sifra,
                    Cijena = proizvod.Cijena.Value,
                    AkcijskaCijena = proizvod.Cijena.Value - (proizvod.Cijena.Value * (postotak / 100))
            };
                return Json(model);
            }
            else
                return null;
        }
        public ActionResult DeaktivirajProizvod(int proizvodId)
        {
            Proizvodi proizvodDb = _db.Proizvodi.Find(proizvodId);
            ProizvodiDetalji detaljiDb = _db.ProizvodiDetalji.Find(proizvodDb.ProizvodDetaljiId);

            detaljiDb.IsAktivan = false;
            _db.ProizvodiDetalji.Update(detaljiDb);
            _db.SaveChanges();

            TempData["SuccessMessage"] = "Uspješno ste obrisali proizvod";
            return RedirectToAction("IndexAdmin");
        }
        
    }
}
