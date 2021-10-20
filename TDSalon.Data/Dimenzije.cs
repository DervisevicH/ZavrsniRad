using System;
using System.Collections.Generic;

namespace TDSalon.Data
{
    public partial class Dimenzije
    {
        public Dimenzije()
        {
            Proizvodi = new HashSet<Proizvodi>();
        }

        public int DimenzijaId { get; set; }
        public string Sirina { get; set; }
        public string Duzina { get; set; }
        public string Debljina { get; set; }
        public int? KategorijaId { get; set; }

        public virtual Kategorije Kategorija { get; set; }
        public virtual ICollection<Proizvodi> Proizvodi { get; set; }
    }
}
