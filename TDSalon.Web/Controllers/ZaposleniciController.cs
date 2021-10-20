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
using TDSalon.Web.Helper;
using TDSalon.Web.Hubs;
using TDSalon.Web.Models;

namespace TDSalon.Web.Controllers
{
    public class ZaposleniciController : Controller
    {
        private readonly TDSalondbContext _db;
        IMapper _mapper;
        public ZaposleniciController(TDSalondbContext db, IMapper mapper) { _db = db; _mapper = mapper; }
        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Zaposlenik")]
        [HttpGet]
        public async Task<ActionResult> Profil()
        {
            int zaposlenikId = HttpContext.GetUserId();
            if (zaposlenikId != 0)
            {
                Zaposlenici zaposlenikDb = _db.Zaposlenici.Find(zaposlenikId);
                if (zaposlenikDb != null)
                {
                    ZaposlenikVM model = new ZaposlenikVM();
                    model = _mapper.Map<ZaposlenikVM>(zaposlenikDb);
                    return View(model);
                }
            }
            return NotFound();
        }
        [Authorize(Roles = "Zaposlenik")]
        [HttpPost]
        public async Task<ActionResult> Sacuvaj(ZaposlenikVM model)
        {
            Zaposlenici zaposlenik =await  _db.Zaposlenici.Where(x => x.ZaposlenikId == model.ZaposlenikId).AsNoTracking().SingleOrDefaultAsync();
            KorisnickiNalozi nalog =await  _db.KorisnickiNalozi.Where(x => x.KorisnickiNalogId==zaposlenik.KorisnickiNalogId).AsNoTracking().SingleOrDefaultAsync();
            if (!String.IsNullOrEmpty(model.StaraLozinka))
            {
                var hash = Helper.PasswordExtension.GetHashedPassword(model.StaraLozinka,nalog.PasswordSalt);
                if (hash != nalog.PasswordHash)
                    ModelState.AddModelError("StaraLozinka", "Niste unijeli tačnu lozinku");
            }

            if (ModelState.IsValid)
            {
                bool isPasswordChanged = false;
                if(!String.IsNullOrEmpty(model.NovaLozinka) && !String.IsNullOrEmpty(model.NovaLozinkaPotvrda))
                {
                    byte[] salt = Helper.PasswordExtension.GenerateSalt(model.NovaLozinka);
                    string hashed = Helper.PasswordExtension.GenerateHash(salt, model.NovaLozinka);
                    string sal = Convert.ToBase64String(salt);
                    nalog.PasswordSalt = sal;
                    nalog.PasswordHash = hashed;

                    _db.KorisnickiNalozi.Update(nalog);
                    await _db.SaveChangesAsync();
                    isPasswordChanged = true;

                }

                zaposlenik.Ime = model.Ime;
                zaposlenik.Prezime = model.Prezime;
                zaposlenik.Telefon = model.Telefon;
                zaposlenik.Email = model.Email;
                _db.Zaposlenici.Update(zaposlenik);
                await _db.SaveChangesAsync();
                if (isPasswordChanged)
                    return RedirectToAction("Logout", "Autentifikacija");
                else
                {
                    TempData["SuccessMessage"] = "Vaša podaci su sačuvani!";                    
                    return RedirectToAction("Profil");
                }

            }
            else
            {
                return View("Profil", model);
            }
        }

    }
}

