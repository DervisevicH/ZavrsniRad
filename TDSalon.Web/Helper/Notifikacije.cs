using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDSalon.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TDSalon.Web.Helper
{
    public static class NotifikacijeHelper
    {
        public static List<Notifikacije> ZaposlenikNotifikacije(this HttpContext context,int userId)
        {
            TDSalondbContext db = context.RequestServices.GetService<TDSalondbContext>();
            return db.Notifikacije.Where(x => x.ZaposlenikId == userId && x.Procitano == false).ToList();
        }
        
    }
}
