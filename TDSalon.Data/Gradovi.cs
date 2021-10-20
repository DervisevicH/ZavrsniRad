using System;
using System.Collections.Generic;

namespace TDSalon.Data
{
    public partial class Gradovi
    {
        public Gradovi()
        {
            Kupci = new HashSet<Kupci>();
        }

        public int GradId { get; set; }
        public string Naziv { get; set; }
        public int? KantonId { get; set; }

        public virtual Kantoni Kanton { get; set; }
        public virtual ICollection<Kupci> Kupci { get; set; }
    }
}
