using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TDSalon.Data;

namespace TDSalon.Web.Controllers
{
    public class GradoviController : Controller
    {
        private readonly TDSalondbContext _db;
        public GradoviController(TDSalondbContext db) { _db = db; }
        public IActionResult Index()
        {
            return View();
        }
        public JsonResult PopuniGradove(int kantonId)
        {
            List<SelectListItem> listaGradova = _db.Gradovi.Where(x => x.KantonId == kantonId).Select(x => new SelectListItem
            {
                Value = x.GradId.ToString(),
                Text = x.Naziv
            }).ToList();
            return Json(listaGradova);
        }
    }
}
