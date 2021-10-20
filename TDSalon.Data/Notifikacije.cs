using System;
using System.Collections.Generic;

namespace TDSalon.Data
{
    public partial class Notifikacije
    {
        public int NotifikacijaId { get; set; }
        public bool? Procitano { get; set; }
        public string Sadrzaj { get; set; }
        public DateTime? DatumKreiranja { get; set; }
        public int? SadrzajId { get; set; }
        public int? ZaposlenikId { get; set; }
        public int? KupacId { get; set; }
        public string TipNotifikacije { get; set; }

        public virtual Kupci Kupac { get; set; }
        public virtual Zaposlenici Zaposlenik { get; set; }
    }
}
