using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace TDSalon.Web.Models
{
    public class PitanjaVM
    {
        public int PitanjeId { get; set; }
        public string Pitanje { get; set; }
        public DateTime Datum { get; set; }
        public bool Procitano { get; set; }
        public int KupacId { get; set; }
        public string ImePrezime{ get; set; }
        public int ProizvodId { get; set; }
        public string ProizvodNaziv{ get; set; }
        public string Odgovor{ get; set; }
    }
}
