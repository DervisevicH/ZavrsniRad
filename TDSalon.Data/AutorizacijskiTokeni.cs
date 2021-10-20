using System;
using System.Collections.Generic;

namespace TDSalon.Data
{
    public partial class AutorizacijskiTokeni
    {
        public int TokenId { get; set; }
        public string Vrijednost { get; set; }
        public DateTime? Vrijeme { get; set; }
        public int? KorisnickiNalogId { get; set; }

        public virtual KorisnickiNalozi KorisnickiNalog { get; set; }
    }
}
