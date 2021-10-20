using System;
using System.Collections.Generic;

namespace TDSalon.Data
{
    public partial class AkcijeProizvodi
    {
        public int AkcijaProizvodiId { get; set; }
        public int? Postotak { get; set; }
        public int? AkcijaId { get; set; }
        public int? ProizvodId { get; set; }
        public decimal? Cijena { get; set; }
        public decimal? AkcijskaCijena { get; set; }

        public virtual Akcije Akcija { get; set; }
        public virtual Proizvodi Proizvod { get; set; }
    }
}
