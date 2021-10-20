using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using TDSalon.Data;
using TDSalon.Web.Controllers;
using TDSalon.Web.Helper;
using TDSalon.Web.Models;
namespace TDSalon.Web.Hubs
{
    public class NotificationHub: Hub
    {
        private readonly INotificationManager _userConnectionManager;
        public NotificationHub(INotificationManager userConnectionManager)
        {
            _userConnectionManager = userConnectionManager;
        }
        public string GetUserId()
        {
            var user = this.Context.User.Identity as ClaimsIdentity;
            string korisnikId = "";

            Claim claim = user?.FindFirst(ClaimTypes.Role);
            if (user.IsAuthenticated)
            {
                if (claim.Value == "Zaposlenik")
                {
                    korisnikId = user.FindFirst("ZaposlenikId").Value;


                }
                if (claim.Value == "Kupac")
                {
                    korisnikId = user.FindFirst("KupacId").Value;
                }
            }            

            return korisnikId;
        }
        //Called when a connection with the hub is terminated.


        public async  Task AfterConnected()
        {
            string korisnikId = GetUserId();
            _userConnectionManager.KeepUserConnection(korisnikId.ToString(), Context.ConnectionId);
            await GetUserNotification();
            
        }
        public override Task OnConnectedAsync()
        {
            string korisnikId = GetUserId();

            _userConnectionManager.KeepUserConnection(korisnikId.ToString(), Context.ConnectionId);
            return base.OnConnectedAsync();
        }
       
        public string GetConnectionId()
        {
            string korisnikId = GetUserId();
            _userConnectionManager.KeepUserConnection(korisnikId, Context.ConnectionId);

            return Context.ConnectionId;
        }
        public async override Task OnDisconnectedAsync(Exception exception)
        {
            //get the connectionId
            var connectionId = Context.ConnectionId;
            _userConnectionManager.RemoveUserConnection(connectionId);
            var value = await Task.FromResult(0);
        }
        public async Task GetUserNotification()
        {
            var connections = _userConnectionManager.GetUserConnections("1003");
            if (connections != null && connections.Count > 0)
            {
                foreach (var connectionId in connections)
                {
                    await Clients.Client(connectionId).SendAsync("notification");                   

                }
            }

        }
        public async void SendUserNotification(List<Notifikacije> notifikacije, string userId)
        {
            var connections = _userConnectionManager.GetUserConnections(userId);
            if (connections != null && connections.Count > 0)
            {
                foreach (var connectionId in connections)
                {
                    foreach (var item in notifikacije)
                    {
                        await Clients.Client(connectionId).SendAsync("novaNotifikacija",item.SadrzajId, item.Sadrzaj, item.TipNotifikacije);
                    }
                   
                }
            }
        }


    }
}
