using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDSalon.Data;

namespace TDSalon.Web.Hubs
{
    public interface INotificationManager
    {
        void KeepUserConnection(string userId, string connectionId);
        void RemoveUserConnection(string connectionId);
        List<string> GetUserConnections(string userId);
    }
}
