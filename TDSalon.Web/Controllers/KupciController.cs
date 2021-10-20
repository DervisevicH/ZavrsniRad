using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TDSalon.Data;
using TDSalon.Web.Helper;
using TDSalon.Web.Models;

namespace TDSalon.Web.Controllers
{
    public class KupciController : Controller
    {
        private readonly TDSalondbContext _db;
        IMapper _mapper;

        public KupciController(TDSalondbContext db, IMapper mapper) { _db = db; _mapper = mapper; }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Zaposlenik")]
        [HttpGet]
        public async Task<ActionResult> GetAllKupce(string imePrezime) {

            List<Kupci> listaKupaca = await _db.Kupci.Include(x => x.Grad).Include("KorisnickiNalog").ToListAsync();
            if (!String.IsNullOrEmpty(imePrezime))
            {
                listaKupaca = listaKupaca.Where(
                    x => (x.Ime != null && x.Ime.ToLower().Contains(imePrezime.ToLower()))
                    || (x.Prezime != null && x.Prezime.ToLower().Contains(imePrezime.ToLower()))).ToList();
            }
            List<KupciVM> model = new List<KupciVM>();
            model = _mapper.Map<List<KupciVM>>(listaKupaca);
            return View("KupciInfo",model);
        
        }
        [Authorize(Roles = "Kupac")]
        [HttpGet]
        public async Task<ActionResult> DodajInfo()
        {
            int kupacId = HttpContext.GetUserId();
            Kupci kupacDb = await _db.Kupci.Include(x => x.Grad).Where(x => x.KupacId == kupacId).SingleOrDefaultAsync();
            KupciDodajInfoVM model = new KupciDodajInfoVM();
            model = _mapper.Map<KupciDodajInfoVM>(kupacDb);

            if (model.GradId != 0)
                model.KantonId = kupacDb.Grad.KantonId.GetValueOrDefault();
            if (kupacDb.GradId != null)
            {
                model.GradId = kupacDb.GradId.Value;
                model.KantonId = _db.Gradovi.Where(x => x.GradId == kupacDb.GradId.Value).Select(x => x.KantonId.Value).Single();
                var listaGradova = _db.Gradovi.Where(x => x.KantonId == model.KantonId).Select(x => new SelectListItem { Value = x.GradId.ToString(), Text = x.Naziv }).ToList();
                model.listaGradova = listaGradova;
            }
            model.listaKantona = _db.Kantoni.Select(x => new SelectListItem { Value = x.KantonId.ToString(), Text = x.Naziv }).ToList();
            return View("DodajInfo", model);
        }
        [Authorize(Roles = "Kupac")]
        [HttpPost]
        public async Task<ActionResult> SacuvajInfo(KupciDodajInfoVM model)
        {
            if (ModelState.IsValid)
            {
                int logiraniKupacId = HttpContext.GetUserId();
                Kupci kupac = await _db.Kupci.Where(x => x.KupacId == logiraniKupacId).SingleOrDefaultAsync();
                if (kupac != null)
                {
                    kupac.Ime = model.Ime;
                    kupac.Prezime = model.Prezime;
                    kupac.Email = model.Email;
                    kupac.Telefon = model.Telefon;
                    kupac.GradId = model.GradId;
                    kupac.Adresa = model.Adresa;

                    _db.Kupci.Update(kupac);
                    await _db.SaveChangesAsync();
                    return RedirectToAction("DodajNarudzbu", "Narudzbe");
                }
                else
                    return RedirectToAction("Logout", "Autentifikacija");
            }
            else
            {
                model.listaKantona = _db.Kantoni.Select(x => new SelectListItem { Value = x.KantonId.ToString(), Text = x.Naziv }).ToList();
                if (model.KantonId != 0)
                {
                    model.listaGradova = _db.Gradovi.Where(x => x.KantonId == model.KantonId).Select(x => new SelectListItem { Value = x.GradId.ToString(), Text = x.Naziv }).ToList();
                }
                return View("DodajInfo", model);
            }
        }
        [HttpGet]
        [Authorize(Roles = "Kupac")]
        public async Task<ActionResult> Profil()
        {
            int kupacId = HttpContext.GetUserId();
            Kupci kupacDb = await _db.Kupci.Where(x => x.KupacId == kupacId).AsNoTracking().SingleOrDefaultAsync();
            ProfilVM model = _mapper.Map<ProfilVM>(kupacDb);            
            return View(model);
        }
        [HttpPost]
        [Authorize(Roles = "Kupac")]
        public async Task<ActionResult> SacuvajProfil(ProfilVM model)
        {
            if (ModelState.IsValid)
            {
                Kupci kupac = _db.Kupci.Find(model.KupacId);
                kupac.Ime = model.Ime;
                kupac.Prezime = model.Prezime;
                kupac.Email = model.Email;
                kupac.Telefon = model.Telefon;
                kupac.Spol = model.Spol;

                _db.Kupci.Update(kupac);
                await _db.SaveChangesAsync();
                TempData["SuccessMsg"] = "Uspješno ste sačuvali podatke";
                return RedirectToAction("Profil",model.KupacId);
            }
            else
            {
                return View("Profil", model);
            }
        }
        [HttpGet]
        [Authorize(Roles = "Kupac")]
        public async Task<ActionResult> ProfilLozinke()
        {
            ProfilLozinkeVM model = new ProfilLozinkeVM();
            model.KupacId = HttpContext.GetUserId();
            return View("ProfilLozinke", model);
        }
        [HttpPost]
        [Authorize(Roles = "Kupac")]
        public async Task<ActionResult> SacuvajLozinku(ProfilLozinkeVM model)
        {
            Kupci kupac = await  _db.Kupci.Include(x=>x.KorisnickiNalog).Where(x=>x.KupacId==model.KupacId).SingleOrDefaultAsync();
            var hash = Helper.PasswordExtension.GetHashedPassword(model.StaraLozinka, kupac.KorisnickiNalog.PasswordSalt);
            if (hash!=kupac.KorisnickiNalog.PasswordHash)
                ModelState.AddModelError("StaraLozinka", "Niste unijeli tačnu lozinku");
            if (ModelState.IsValid && !String.IsNullOrEmpty(model.NovaLozinka))
            {
                KorisnickiNalozi nalog = await _db.KorisnickiNalozi.Where(x => x.KorisnickiNalogId == kupac.KorisnickiNalog.KorisnickiNalogId).SingleOrDefaultAsync();                
                byte[] salt = Helper.PasswordExtension.GenerateSalt(model.NovaLozinka);
                string hashed = Helper.PasswordExtension.GenerateHash(salt, model.NovaLozinka);
                string sal = Convert.ToBase64String(salt);
                nalog.PasswordSalt = sal;
                nalog.PasswordHash = hashed;

                _db.KorisnickiNalozi.Update(nalog);
                await _db.SaveChangesAsync();
                
                return RedirectToAction("Logout","Autentifikacija");
            }
            else
                return View("ProfilLozinke", model);
            
        }
        [HttpGet]
        [Authorize(Roles = "Kupac")]
        public async Task<ActionResult> ProfilAdrese()
        {
            ProfilAdreseVM model = new ProfilAdreseVM();
            int kupacId = HttpContext.GetUserId();
            Kupci kupac = await _db.Kupci.Include(x => x.Grad).Where(x => x.KupacId == kupacId).SingleOrDefaultAsync();
            model.Adresa = kupac.Adresa;
            if (model.GradId != 0)
                model.KantonId = kupac.Grad.KantonId.GetValueOrDefault();
            if (kupac.GradId != null)
            {
                model.GradId = kupac.GradId.Value;
                model.KantonId = _db.Gradovi.Where(x => x.GradId == kupac.GradId.Value).Select(x => x.KantonId.Value).Single();
                var listaGradova = _db.Gradovi.Where(x => x.KantonId == model.KantonId).Select(x => new SelectListItem { Value = x.GradId.ToString(), Text = x.Naziv }).ToList();
                model.listaGradova = listaGradova;
            }
            model.listaKantona = _db.Kantoni.Select(x => new SelectListItem { Value = x.KantonId.ToString(), Text = x.Naziv }).ToList();
            return View("ProfilAdrese", model);
        }
        [HttpPost]
        [Authorize(Roles = "Kupac")]
        public async Task<ActionResult> SacuvajAdrese(ProfilAdreseVM model)
        {
            if (ModelState.IsValid)
            {
                int kupacId = HttpContext.GetUserId();
                Kupci kupac = await _db.Kupci.Where(x => x.KupacId == kupacId).SingleOrDefaultAsync();
                if (kupac != null)
                {                   
                    kupac.GradId = model.GradId;
                    kupac.Adresa = model.Adresa;

                    _db.Kupci.Update(kupac);
                    await _db.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Vaša adresa je promijenjena!";
                    return RedirectToAction("ProfilAdrese");
                }
                else
                    return RedirectToAction("Logout", "Autentifikacija");
            }
            else
            {
                model.listaKantona =await _db.Kantoni.Select(x => new SelectListItem { Value = x.KantonId.ToString(), Text = x.Naziv }).ToListAsync();
                if (model.KantonId != 0)
                {
                    model.listaGradova = await _db.Gradovi.Where(x => x.KantonId == model.KantonId).Select(x => new SelectListItem { Value = x.GradId.ToString(), Text = x.Naziv }).ToListAsync();
                }
                return View("ProfilAdrese", model);
            }            
        }
        [Authorize(Roles = "Zaposlenik")]
        public async Task<ActionResult> PromijeniStatus(int kupacId)
        {
            KorisnickiNalozi korisnickiNalog = await _db.Kupci.Include(x=>x.KorisnickiNalog).Where(x=>x.KupacId==kupacId).Select(x=>x.KorisnickiNalog).SingleOrDefaultAsync();
            if (korisnickiNalog != null)
            {
                korisnickiNalog.IsAktivan = !korisnickiNalog.IsAktivan;
                _db.KorisnickiNalozi.Update(korisnickiNalog);
                await _db.SaveChangesAsync();
                if(!korisnickiNalog.IsAktivan.Value)
                    TempData["SuccessMsg"] = "Uspješno ste deaktivirali kupca!";
                else
                   TempData["SuccessMsg"] = "Uspješno ste aktivirali kupca!";
            }
            return RedirectToAction("GetAllKupce");
        }
    }
}
