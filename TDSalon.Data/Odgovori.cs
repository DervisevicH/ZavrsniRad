using System;
using System.Collections.Generic;

namespace TDSalon.Data
{
    public partial class Odgovori
    {
        public int OdgovorId { get; set; }
        public string Odgovor { get; set; }
        public bool? Procitano { get; set; }
        public int? ZaposlenikId { get; set; }
        public int? PitanjeId { get; set; }

        public virtual Pitanja Pitanje { get; set; }
        public virtual Zaposlenici Zaposlenik { get; set; }
    }
}
