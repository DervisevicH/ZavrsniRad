using System;
using System.Collections.Generic;

namespace TDSalon.Data
{
    public partial class Akcije
    {
        public Akcije()
        {
            AkcijeProizvodi = new HashSet<AkcijeProizvodi>();
        }

        public int AkcijaId { get; set; }
        public string Naziv { get; set; }
        public DateTime? DatumOd { get; set; }
        public DateTime? DatumDo { get; set; }
        public bool? IsAktivna { get; set; }
        public decimal? Postotak { get; set; }

        public virtual ICollection<AkcijeProizvodi> AkcijeProizvodi { get; set; }
    }
}
