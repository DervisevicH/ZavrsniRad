using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TDSalon.Web.Models
{
    public class FavoritiVM
    {
        public int FavoritId { get; set; }
        public List<Row> Rows{ get; set; }
        public class Row
        {
            public int ProizvodId { get; set; }
            public string Proizvod { get; set; }
            public decimal Cijena { get; set; }
            public string Slika{ get; set; }
            public bool IsAkcija{ get; set; }
            public int Akcija{ get; set; }
            public int FavoritProizvodId{ get; set; }
            public decimal AkcijskaCijena { get; set; }
        }
    }
}
