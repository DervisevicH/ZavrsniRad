using System;
using System.Collections.Generic;

namespace TDSalon.Data
{
    public partial class JediniceMjere
    {
        public JediniceMjere()
        {
            ProizvodiDetalji = new HashSet<ProizvodiDetalji>();
        }

        public int JedinicaMjereId { get; set; }
        public string Naziv { get; set; }

        public virtual ICollection<ProizvodiDetalji> ProizvodiDetalji { get; set; }
    }
}
