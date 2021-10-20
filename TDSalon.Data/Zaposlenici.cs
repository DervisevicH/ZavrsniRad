using System;
using System.Collections.Generic;

namespace TDSalon.Data
{
    public partial class Zaposlenici
    {
        public Zaposlenici()
        {
            Narudzbe = new HashSet<Narudzbe>();
            Notifikacije = new HashSet<Notifikacije>();
            Obavijesti = new HashSet<Obavijesti>();
            Odgovori = new HashSet<Odgovori>();
        }

        public int ZaposlenikId { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public int? KorisnickiNalogId { get; set; }

        public virtual KorisnickiNalozi KorisnickiNalog { get; set; }
        public virtual ICollection<Narudzbe> Narudzbe { get; set; }
        public virtual ICollection<Notifikacije> Notifikacije { get; set; }
        public virtual ICollection<Obavijesti> Obavijesti { get; set; }
        public virtual ICollection<Odgovori> Odgovori { get; set; }
    }
}
