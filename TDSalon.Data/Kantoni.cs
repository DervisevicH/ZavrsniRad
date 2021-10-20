using System;
using System.Collections.Generic;

namespace TDSalon.Data
{
    public partial class Kantoni
    {
        public Kantoni()
        {
            Gradovi = new HashSet<Gradovi>();
        }

        public int KantonId { get; set; }
        public string Naziv { get; set; }

        public virtual ICollection<Gradovi> Gradovi { get; set; }
    }
}
