using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using TDSalon.Data;
using TDSalon.Web.Hubs;
using TDSalon.Web.Models;
using Microsoft.AspNetCore.Http;
using TDSalon.Web.Helper;
using Microsoft.AspNetCore.Authorization;

namespace TDSalon.Web.Controllers
{
    public class NotifikacijeController : Controller
    {
        private TDSalondbContext _db;
        private readonly IHubContext<NotificationHub> _notificationHubContext;
        private readonly INotificationManager _notificationManager;
        public NotifikacijeController( INotificationManager notificationManager,TDSalondbContext db, IHubContext<NotificationHub> notificationHubContext) { _db = db; _notificationManager = notificationManager; _notificationHubContext = notificationHubContext; }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<ActionResult> GetZaposlenikNotifikacije()
        {
            
            int zaposlenikId = HttpContext.GetUserId();
            List<Notifikacije> listaNotifikacija = new List<Notifikacije>();
            listaNotifikacija = _db.Notifikacije.Where(x => x.ZaposlenikId == zaposlenikId).ToList();
            var connections = _notificationManager.GetUserConnections(zaposlenikId.ToString());
            if (connections != null && connections.Count > 0)
            {
                foreach (var connectionId in connections)
                {
                    foreach (var item in listaNotifikacija)
                    {
                        await _notificationHubContext.Clients.Client(connectionId).SendAsync("novaNotifikacija",item.SadrzajId, item.Sadrzaj, item.TipNotifikacije, item.NotifikacijaId);
                    }
                }
            }
            return Ok();
        }
        [Authorize(Roles = "Kupac")]
        public async Task<ActionResult> GetKupacNotifikacije()
        {

            int kupacId = HttpContext.GetUserId();
            List<Notifikacije> listaNotifikacija = new List<Notifikacije>();
            listaNotifikacija = _db.Notifikacije.Where(x => x.KupacId == kupacId && x.Procitano == false).ToList();
            var connections = _notificationManager.GetUserConnections(kupacId.ToString());
            if (connections != null && connections.Count > 0)
            {
                foreach (var connectionId in connections)
                {
                    foreach (var item in listaNotifikacija)
                    {

                        await _notificationHubContext.Clients.Client(connectionId).SendAsync("novaNotifikacija", item.SadrzajId, item.Sadrzaj, item.TipNotifikacije, item.NotifikacijaId);
                    }
                }
            }
            return Ok();
        }
        
        public async Task<ActionResult> IzbrisiNotifikaciju(int notifikacijaId)
        {
            Notifikacije notifikacijaDb = await _db.Notifikacije.FindAsync(notifikacijaId);
            notifikacijaDb.Procitano = true;
            _db.Notifikacije.Update(notifikacijaDb);
            await _db.SaveChangesAsync();
            return Ok();
        }

        public JsonResult GetBrojNotifikacija()
        {
            int userId = 1;
            var broj = _db.Notifikacije.Where(x => x.KupacId == userId && x.Procitano == false).Count();
            return Json(broj);
        }
        //public ActionResult GetNotifikacije(int userId)
        //{
        //    var listaProizvoda = _db.Notifikacije.Where(x => x.KupacId == userId).ToList();
        //    NotifikacijeVM model = new NotifikacijeVM();
        //    model.Rows = new List<NotifikacijeVM.Row>();
        //    foreach (var item in listaProizvoda)
        //    {
        //        NotifikacijeVM.Row row = new NotifikacijeVM.Row();
        //        row.ProizvodId = item.ProizvodId.Value;
        //        Proizvodi p = _db.Proizvodi.Include(x => x.ProizvodDetalji).Where(x => x.ProizvodId == item.ProizvodId).SingleOrDefault();
        //        row.Proizvod = p.ProizvodDetalji.Naziv;
        //        row.Cijena = p.Cijena.ToString();
        //        row.AkcijskaCijena = _db.AkcijeProizvodi.Where(x => x.ProizvodId == x.ProizvodId).Select(x => x.AkcijskaCijena.ToString()).SingleOrDefault();
        //        row.Slika = _db.Slike.Where(x => x.ProizvodDetaljiId == p.ProizvodDetaljiId).Select(x=>x.SlikaUrl).SingleOrDefault();
        //        model.Rows.Add(row);
        //    }
            
        //    return PartialView("NotifikacijePartial", model);
        //}
        //public ActionResult KreirajNotifikacije(List<int> listaProizvoda)
        //{

        //    var listaFavorita = _db.FavoritiStavke.Where(x=>listaProizvoda.Any(y=>y==x.ProizvodId)).Include(x=>x.Favorit).ToList();

        //    foreach (var item in listaFavorita)
        //    {
        //        Notifikacije novaNotifikacija = new Notifikacije
        //        {
        //            ProizvodId = item.ProizvodId,
        //            Procitano = false,
        //            KupacId = item.Favorit.KupacId
        //        };
        //        _db.Notifikacije.Add(novaNotifikacija);
        //    }
        //    _db.SaveChanges();

        //    return Ok();
        //}
    }
}
