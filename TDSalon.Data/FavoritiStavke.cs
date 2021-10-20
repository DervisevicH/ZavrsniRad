using System;
using System.Collections.Generic;

namespace TDSalon.Data
{
    public partial class FavoritiStavke
    {
        public int FavoritStavkaId { get; set; }
        public DateTime? DatumDodavanja { get; set; }
        public int? FavoritId { get; set; }
        public int? ProizvodId { get; set; }

        public virtual Favoriti Favorit { get; set; }
        public virtual Proizvodi Proizvod { get; set; }
    }
}
