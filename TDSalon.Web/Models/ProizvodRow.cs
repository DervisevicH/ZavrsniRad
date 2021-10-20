using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TDSalon.Web.Models
{
    public class ProizvodRow
    {
        public int ProizvodId { get; set; }
        public string Naziv { get; set; }
        public string Cijena { get; set; }
        public string SlikaUrl { get; set; }
        public bool? IsAkcija { get; set; }
        public string AkcijskaCijena { get; set; }
        public int Akcija { get; set; }
    }
}
