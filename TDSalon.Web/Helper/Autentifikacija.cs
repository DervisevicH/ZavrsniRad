using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDSalon.Data;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace TDSalon.Web.Helper
{
    public static class Autentifikacija
    {
        public static int  GetUserId(this HttpContext context)
        {             
            var user = context.User.Identity as ClaimsIdentity;
            Claim claim = user?.FindFirst(ClaimTypes.Role);
            if (user.IsAuthenticated)
            {
                int korisnikId = 0;
                if (claim.Value == "Zaposlenik")
                {
                    Int32.TryParse(user.FindFirst("ZaposlenikId").Value, out korisnikId);


                }
                if (claim.Value == "Kupac")
                {
                    Int32.TryParse(user.FindFirst("KupacId").Value, out korisnikId);
                }
                return korisnikId;
            }
            else return 0;
        }
        public static string GetUsername(this HttpContext context)
        {
            var user = context.User.Identity as ClaimsIdentity;
            Claim claim = user?.FindFirst(ClaimTypes.Name);
            if (user.IsAuthenticated)
            {
                return user.FindFirst(ClaimTypes.Name).Value;
            }
            else return "";
        }
        public static bool IsAuthenticated(this HttpContext context)
        {
            var user = context.User.Identity as ClaimsIdentity;
            if (user.IsAuthenticated)
                return true;
            else
                return false;
        }
        public static bool isKupac(this HttpContext context)
        {
            var user = context.User.Identity as ClaimsIdentity;
            Claim claim = user?.FindFirst(ClaimTypes.Role);
            if (user.IsAuthenticated)
            {
                if (claim.Value == "Kupac")
                    return true;
                else
                    return false;
            }
            else 
                return false;
            
        }
        public static bool isZaposlenik(this HttpContext context)
        {
            var user = context.User.Identity as ClaimsIdentity;
            Claim claim = user?.FindFirst(ClaimTypes.Role);
            if (user.IsAuthenticated)
            {
                if (claim.Value == "Zaposlenik")
                    return true;
                else
                    return false;
            }
            else
                return false;

        }
    }
}
