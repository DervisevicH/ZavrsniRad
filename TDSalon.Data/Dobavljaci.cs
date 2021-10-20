using System;
using System.Collections.Generic;

namespace TDSalon.Data
{
    public partial class Dobavljaci
    {
        public Dobavljaci()
        {
            ProizvodiDetalji = new HashSet<ProizvodiDetalji>();
        }

        public int DobavljacId { get; set; }
        public string NazivFirme { get; set; }
        public string KontaktOsoba { get; set; }
        public string Telefon { get; set; }
        public string Email { get; set; }

        public virtual ICollection<ProizvodiDetalji> ProizvodiDetalji { get; set; }
    }
}
