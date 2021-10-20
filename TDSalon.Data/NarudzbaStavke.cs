using System;
using System.Collections.Generic;

namespace TDSalon.Data
{
    public partial class NarudzbaStavke
    {
        public int StavkaId { get; set; }
        public decimal? Kolicina { get; set; }
        public decimal? Cijena { get; set; }
        public decimal? Popust { get; set; }
        public int? NarudzbaId { get; set; }
        public int? ProizvodId { get; set; }

        public virtual Narudzbe Narudzba { get; set; }
        public virtual Proizvodi Proizvod { get; set; }
    }
}
