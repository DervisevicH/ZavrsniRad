using System;
using System.Collections.Generic;

namespace TDSalon.Data
{
    public partial class Favoriti
    {
        public Favoriti()
        {
            FavoritiStavke = new HashSet<FavoritiStavke>();
        }

        public int FavoritId { get; set; }
        public int? KupacId { get; set; }

        public virtual Kupci Kupac { get; set; }
        public virtual ICollection<FavoritiStavke> FavoritiStavke { get; set; }
    }
}
