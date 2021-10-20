using System;
using System.Collections.Generic;

namespace TDSalon.Data
{
    public partial class Slike
    {
        public int SlikaId { get; set; }
        public string SlikaUrl { get; set; }
        public byte[] Slika { get; set; }
        public int? ProizvodDetaljiId { get; set; }

        public virtual ProizvodiDetalji ProizvodDetalji { get; set; }
    }
}
