using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TDSalon.Web.Models
{
    public class KupciVM
    {
        public int KupacId { get; set; }
        public string ImePrezime { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public string Adresa { get; set; }       
        public DateTime? DatumRegistracije { get; set; }
        public string Grad { get; set; }
        public bool IsAktivan{ get; set; }

    }
}
