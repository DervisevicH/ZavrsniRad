using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TDSalon.Web.Models
{
    public class NotifikacijeVM
    {
        public int KupacId { get; set; }
        public List<Row> Rows{ get; set; }
        public class Row
        {
            public string Proizvod { get; set; }
            public int ProizvodId { get; set; }
            public string Slika{ get; set; }
            public string Cijena { get; set; }
            public string AkcijskaCijena { get; set; }
        }
    }
}
