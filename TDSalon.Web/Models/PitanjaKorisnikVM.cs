using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TDSalon.Web.Models
{
    public class PitanjaKorisnikVM
    {
        public List<Row> Rows{ get; set; }
       
        public int KupacId { get; set; }
        public class Row
        {
            public int PitanjeId { get; set; }
            public int OdgovorId { get; set; }
            public string Pitanje { get; set; }
            public string Odgovor { get; set; }
        }
    }
}
