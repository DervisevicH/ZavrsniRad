using System;
using System.Collections.Generic;

namespace TDSalon.Data
{
    public partial class KorpaProizvodi
    {
        public int KorpaProizvodId { get; set; }
        public decimal? Kolicina { get; set; }
        public int? ProizvodId { get; set; }
        public int? KorpaId { get; set; }
        public decimal? Cijena { get; set; }

        public virtual Korpe Korpa { get; set; }
        public virtual Proizvodi Proizvod { get; set; }
    }
}
