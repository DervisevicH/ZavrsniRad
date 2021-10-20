using System;
using System.Collections.Generic;

namespace TDSalon.Data
{
    public partial class Kategorije
    {
        public Kategorije()
        {
            Dimenzije = new HashSet<Dimenzije>();
            ProizvodiDetalji = new HashSet<ProizvodiDetalji>();
        }

        public int KategorijaId { get; set; }
        public string Naziv { get; set; }

        public virtual ICollection<Dimenzije> Dimenzije { get; set; }
        public virtual ICollection<ProizvodiDetalji> ProizvodiDetalji { get; set; }
    }
}
