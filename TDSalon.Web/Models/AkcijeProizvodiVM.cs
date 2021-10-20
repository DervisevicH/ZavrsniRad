using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TDSalon.Web.Models
{
    public class AkcijeProizvodiVM
    {
        public int ProizvodId { get; set; }
        public int AkcijaProizvodiId { get; set; }
        public string Sifra{ get; set; }
        public int AkcijaId { get; set; }
        public string Naziv { get; set; }
        public decimal Cijena{ get; set; }
        public int Postotak{ get; set; }
        public decimal AkcijskaCijena{ get; set; }

    }
}
