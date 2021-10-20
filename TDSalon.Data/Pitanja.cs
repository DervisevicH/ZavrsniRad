using System;
using System.Collections.Generic;

namespace TDSalon.Data
{
    public partial class Pitanja
    {
        public Pitanja()
        {
            Odgovori = new HashSet<Odgovori>();
        }

        public int PitanjeId { get; set; }
        public string Pitanje { get; set; }
        public DateTime? Datum { get; set; }
        public bool? Procitano { get; set; }
        public int? KupacId { get; set; }
        public int? ProizvodId { get; set; }

        public virtual Kupci Kupac { get; set; }
        public virtual Proizvodi Proizvod { get; set; }
        public virtual ICollection<Odgovori> Odgovori { get; set; }
    }
}
