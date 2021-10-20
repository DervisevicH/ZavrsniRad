using System;
using System.Collections.Generic;

namespace TDSalon.Data
{
    public partial class Obavijesti
    {
        public int ObavijestId { get; set; }
        public DateTime? DatumObjave { get; set; }
        public string Obavijest { get; set; }
        public string SlikaUrl { get; set; }
        public byte[] Slika { get; set; }
        public int? ZaposlenikId { get; set; }
        public string Naslov { get; set; }

        public virtual Zaposlenici Zaposlenik { get; set; }
    }
}
