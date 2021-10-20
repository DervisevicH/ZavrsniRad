using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TDSalon.Data;
using TDSalon.Web.Helper;
using TDSalon.Web.Models;

namespace TDSalon.Web.Controllers
{
    public class ObavijestiController : Controller
    {

        private readonly TDSalondbContext _db;
        IMapper _mapper;
        public ObavijestiController(TDSalondbContext db, IMapper mapper) { _db = db; _mapper = mapper; }
        [HttpGet]
        public async Task<ActionResult> IndexPartial()
        {
            List<Obavijesti> obavijestiDb = await _db.Obavijesti.OrderByDescending(x => x.DatumObjave).ToListAsync();
            List<ObavijestiVM> model = _mapper.Map<List<ObavijestiVM>>(obavijestiDb);
            return PartialView("_obavijestiPartial",model);
        }

        [Authorize(Roles = "Zaposlenik")]
        [HttpGet]
        public async Task<ActionResult> IndexAdmin(string pretraga) {

            var obavijesti = _db.Obavijesti.AsQueryable();
            if (!String.IsNullOrEmpty(pretraga)) {
                obavijesti = obavijesti.Where(x => x.Naslov.Contains(pretraga));
            }
            var model = obavijesti.ToList();
            var mappedModel = _mapper.Map<List<ObavijestiVM>>(model);
            return View("IndexAdmin", mappedModel);
        }
        [Authorize(Roles = "Zaposlenik")]
        [HttpGet]
        public async Task<ActionResult> Dodaj()
        {
            ObavijestiDodajVM model = new ObavijestiDodajVM();
            return View(model);
        }
        [Authorize(Roles = "Zaposlenik")]
        [HttpGet]
        public async Task<ActionResult> Uredi(int obavijestId)
        {
            var obavijest = await _db.Obavijesti.FindAsync(obavijestId);
            var mappedObavijest = _mapper.Map<ObavijestiDodajVM>(obavijest);
            return View("Dodaj", mappedObavijest);
        }
        [Authorize(Roles = "Zaposlenik")]
        [HttpPost]
        public async Task<ActionResult> Sacuvaj(ObavijestiDodajVM model) 
        {
            if (ModelState.IsValid)
            {
                Obavijesti novaObavijest;
                if (model.ObavijestId == 0) {
                    novaObavijest = new Obavijesti();
                    novaObavijest = _mapper.Map<Obavijesti>(model);
                    novaObavijest.DatumObjave = System.DateTime.Now;
                    novaObavijest.ZaposlenikId = HttpContext.GetUserId();
                    _db.Obavijesti.Add(novaObavijest);
                }
                else
                {
                    var obavijest =await  _db.Obavijesti.FindAsync(model.ObavijestId);
                    obavijest.Naslov = model.Naslov;
                    obavijest.Obavijest = model.Obavijest;
                    _db.Obavijesti.Update(obavijest);
                }
              
                await _db.SaveChangesAsync();
                return RedirectToAction("IndexAdmin");
            }
            else
            {
                return View("Dodaj", model);
            }
        }

        [Authorize(Roles = "Zaposlenik")]
        [HttpGet]
        public async Task<ActionResult> Obrisi(int obavijestId)
        {
            Obavijesti obavijestDb = _db.Obavijesti.Find(obavijestId);
            if (obavijestDb != null)
            {
                _db.Obavijesti.Remove(obavijestDb);
                await _db.SaveChangesAsync();
                TempData["poruka"] = "Uspješno ste obrisali obavijest!";
            }

            return RedirectToAction("IndexAdmin");
        }


    }
}
