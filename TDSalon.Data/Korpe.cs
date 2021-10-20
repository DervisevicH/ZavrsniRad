using System;
using System.Collections.Generic;

namespace TDSalon.Data
{
    public partial class Korpe
    {
        public Korpe()
        {
            KorpaProizvodi = new HashSet<KorpaProizvodi>();
        }

        public int KorpaId { get; set; }
        public DateTime? DatumModifikacije { get; set; }
        public decimal? Ukupno { get; set; }
        public int? KupacId { get; set; }

        public virtual Kupci Kupac { get; set; }
        public virtual ICollection<KorpaProizvodi> KorpaProizvodi { get; set; }
    }
}
