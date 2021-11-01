using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using TDSalon.Data;
using TDSalon.Web.Helper;
using TDSalon.Web.Models;

namespace TDSalon.Web.Controllers
{
    public class AutentifikacijaController : Controller
    {
        private readonly TDSalondbContext _db;
        public AutentifikacijaController(TDSalondbContext db) { _db = db; }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Login()
        {
            return View("Login");
        }
        [HttpPost]
        public ActionResult Login(LoginVM model)
        {
            if (ModelState.IsValid)
            {
                KorisnickiNalozi korisnik = _db.KorisnickiNalozi
                   .SingleOrDefault(x => x.Username == model.KorisnickoIme);
                string hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: model.Lozinka,
                salt: System.Convert.FromBase64String(korisnik.PasswordSalt),///Encoding.ASCII.GetBytes(dbPasswordSalt),
               prf: KeyDerivationPrf.HMACSHA1,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));

                if (korisnik == null || korisnik.PasswordHash != hashedPassword)
                {
                    TempData["error_poruka"] = "Pogrešan username ili password";
                    return View(model);
                }

                //Check the user name and password
                //Here can be implemented checking logic from the database
                ClaimsIdentity identity = null;
                bool isAuthenticated = false;

                Kupci kupac = _db.Kupci.Where(x => x.KorisnickiNalogId == korisnik.KorisnickiNalogId).SingleOrDefault();
                Zaposlenici zaposlenik = _db.Zaposlenici.Where(x => x.KorisnickiNalogId == korisnik.KorisnickiNalogId).SingleOrDefault();
                if (kupac != null)
                {

                    //Create the identity for the user
                    identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, korisnik.Username),
                    new Claim(ClaimTypes.Role, "Kupac"),
                    new Claim("KupacId",kupac.KupacId.ToString())
                }, CookieAuthenticationDefaults.AuthenticationScheme);

                    isAuthenticated = true;
                }

                if (zaposlenik != null)
                {
                    //Create the identity for the user
                    identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, korisnik.Username),
                    new Claim(ClaimTypes.Role, "Zaposlenik"),
                    new Claim("ZaposlenikId",zaposlenik.ZaposlenikId.ToString())
                }, CookieAuthenticationDefaults.AuthenticationScheme);

                    isAuthenticated = true;
                }

                if (isAuthenticated)
                {
                    var principal = new ClaimsPrincipal(identity);

                    var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                    if (zaposlenik != null)
                        return RedirectToAction("IndexZaposlenik", "Home");
                    else
                        return RedirectToAction("Shop", "Proizvodi");
                }

            }
            return View(model);
        }
        public ActionResult Registracija()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registracija(RegistracijaVM model)
        {
            if (ModelState.IsValid)
            {

                KorisnickiNalozi noviKorisnickiNalog = new KorisnickiNalozi();
                noviKorisnickiNalog.Username = model.Username;
                byte[] salt = PasswordExtension.GenerateSalt(model.Password);
                string hashed = PasswordExtension.GenerateHash(salt, model.Password);
                string sal = Convert.ToBase64String(salt);
                noviKorisnickiNalog.PasswordSalt = sal;
                noviKorisnickiNalog.PasswordHash = hashed;
                _db.KorisnickiNalozi.Add(noviKorisnickiNalog);
                _db.SaveChanges();


                Kupci noviKupac = new Kupci();
                noviKupac.Email = model.Email;
                noviKupac.DatumRegistracije = DateTime.Now;
                noviKupac.Spol = model.Spol;
                noviKupac.KorisnickiNalogId = noviKorisnickiNalog.KorisnickiNalogId;
                _db.Kupci.Add(noviKupac);
                _db.SaveChanges();

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }
        public ActionResult Logout()
        {
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}
