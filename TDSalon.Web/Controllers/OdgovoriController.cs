using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using TDSalon.Data;
using TDSalon.Web.Helper;
using TDSalon.Web.Hubs;
using TDSalon.Web.Models;

namespace TDSalon.Web.Controllers
{
    public class OdgovoriController : Controller
    {
        private TDSalondbContext _db;
        IMapper _mapper;
        private readonly IHubContext<NotificationHub> _notificationHubContext;
        private readonly INotificationManager _notificationManager;
        public OdgovoriController(TDSalondbContext db, IMapper mapper, IHubContext<NotificationHub> notificationHubContext, INotificationManager notificationManager) { _db = db; _mapper = mapper; _notificationHubContext = notificationHubContext; _notificationManager = notificationManager; }

        public IActionResult Index()
        {
            return View();
        }
        [Authorize(Roles = "Zaposlenik")]
        [HttpGet]
        public async Task<ActionResult> Dodaj(int pitanjeId)
        {
            OdgovoriDodajVM noviOdgovor = new OdgovoriDodajVM() { PitanjeId = pitanjeId };
            return View(noviOdgovor);
        }
        [Authorize(Roles = "Zaposlenik")]
        [HttpPost] 
        public async Task<ActionResult> Sacuvaj(OdgovoriDodajVM model)
        {
            if (ModelState.IsValid)
            {
                int zaposlenikId = HttpContext.GetUserId();
                Pitanja pitanjeDb = _db.Pitanja.Where(x => x.PitanjeId == model.PitanjeId).SingleOrDefault();    
                Odgovori odgovorDb = _mapper.Map<Odgovori>(model);
                odgovorDb.ZaposlenikId = zaposlenikId;
                await _db.Odgovori.AddAsync(odgovorDb);
                await _db.SaveChangesAsync();

                //posalji notifikaciju
                Notifikacije novaNotifikacija = new Notifikacije()
                {
                    KupacId = pitanjeDb.KupacId,
                    Sadrzaj = "Zaposlenik je odgovorio na vaše pitanje",
                    TipNotifikacije = "Odgovori",
                    DatumKreiranja = System.DateTime.Now,
                    Procitano = false
                };
                await _db.Notifikacije.AddAsync(novaNotifikacija);
                await _db.SaveChangesAsync();

                var connections = _notificationManager.GetUserConnections(novaNotifikacija.KupacId.ToString());
                if (connections != null && connections.Count > 0)
                {
                    foreach (var connectionId in connections)
                    {                         
                      await _notificationHubContext.Clients.Client(connectionId).SendAsync(
                          "novaNotifikacija",                          
                          novaNotifikacija.SadrzajId,
                          novaNotifikacija.Sadrzaj,
                          novaNotifikacija.TipNotifikacije);                        
                    }
                }
                return RedirectToAction("Index", "Pitanja");
            }
            else
                return View("Dodaj", model);

        }

    }
}
