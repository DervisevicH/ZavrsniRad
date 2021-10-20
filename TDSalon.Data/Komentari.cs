using System;
using System.Collections.Generic;

namespace TDSalon.Data
{
    public partial class Komentari
    {
        public int KomentarId { get; set; }
        public string Komentar { get; set; }
        public DateTime? Datum { get; set; }
        public string Naslov { get; set; }
        public int? Ocjena { get; set; }
        public int? KupacId { get; set; }
        public int? ProizvodDetaljiId { get; set; }

        public virtual Kupci Kupac { get; set; }
        public virtual ProizvodiDetalji ProizvodDetalji { get; set; }
    }
}
