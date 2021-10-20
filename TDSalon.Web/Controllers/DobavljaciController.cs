using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using TDSalon.Data;
using TDSalon.Web.Models;
using Microsoft.AspNetCore.Authorization;

namespace TDSalon.Web.Controllers
{
    public class DobavljaciController : Controller
    {
        private TDSalondbContext _db;
        IMapper _mapper;

        public DobavljaciController(TDSalondbContext db, IMapper mapper) { _db = db;  _mapper = mapper; }
        [Authorize(Roles = "Zaposlenik")]
        public async Task<ActionResult> Index(string naziv)
        {
            var dobavljaciDb = _db.Dobavljaci.AsNoTracking().AsQueryable();
            if (!String.IsNullOrEmpty(naziv))
            {
                 dobavljaciDb = dobavljaciDb.Where(x=>x.NazivFirme.Contains(naziv));
            }
            await dobavljaciDb.ToListAsync();
            List<DobavljaciVM> model = _mapper.Map<List<DobavljaciVM>>(dobavljaciDb);
            return View(model);
        }
        [HttpGet]
        [Authorize(Roles = "Zaposlenik")]
        public ActionResult Dodaj()
        {
            DobavljaciVM model = new DobavljaciVM();
            return View(model);
        }
        [HttpGet]
        [Authorize(Roles = "Zaposlenik")]

        public async Task<ActionResult> Uredi(int id)
        {
            DobavljaciVM model = new DobavljaciVM();
            Dobavljaci dobavljacDb = await _db.Dobavljaci.FindAsync(id);
            model = _mapper.Map<DobavljaciVM>(dobavljacDb);
            return View("Dodaj",model);
        }
        [HttpPost]
        [Authorize(Roles = "Zaposlenik")]
        public async Task<ActionResult> Sacuvaj(DobavljaciVM model)
        {
            if (ModelState.IsValid)
            {                
                Dobavljaci dobavljacDb = await _db.Dobavljaci.AsNoTracking().Where(x=>x.DobavljacId==model.DobavljacId).FirstOrDefaultAsync();
                dobavljacDb = _mapper.Map<Dobavljaci>(model);
                if (dobavljacDb != null)
                {
                    _db.Dobavljaci.Update(dobavljacDb);
                }
                else
                {
                    _db.Dobavljaci.Add(dobavljacDb);
                }
                await _db.SaveChangesAsync(); 
                TempData["SuccessMessage"] = "Uspješno ste sačuvali dobavljača!";
                return RedirectToAction("Dodaj");
            }
            else
            {
                return View("Dodaj", model);
            }
        }
        [Authorize(Roles = "Zaposlenik")]
        [HttpGet]
        public async Task<ActionResult> Obrisi(int dobavljacId)
        {
            Dobavljaci dobavljacDb = _db.Dobavljaci.Find(dobavljacId);
            if (dobavljacDb != null)
            {
                _db.Dobavljaci.Remove(dobavljacDb);
                await _db.SaveChangesAsync();
                TempData["poruka"] = "Uspješno ste obrisali dobavljača!";
            }

            return RedirectToAction("Index");
        }

    }
}
