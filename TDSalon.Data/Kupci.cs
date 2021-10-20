using System;
using System.Collections.Generic;

namespace TDSalon.Data
{
    public partial class Kupci
    {
        public Kupci()
        {
            Favoriti = new HashSet<Favoriti>();
            Komentari = new HashSet<Komentari>();
            Narudzbe = new HashSet<Narudzbe>();
            Notifikacije = new HashSet<Notifikacije>();
            Pitanja = new HashSet<Pitanja>();
        }

        public int KupacId { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        public string Adresa { get; set; }
        public byte[] SlikaProfila { get; set; }
        public DateTime? DatumRegistracije { get; set; }
        public int? PostanskiBroj { get; set; }
        public string Spol { get; set; }
        public int? KorisnickiNalogId { get; set; }
        public int? GradId { get; set; }

        public virtual Gradovi Grad { get; set; }
        public virtual KorisnickiNalozi KorisnickiNalog { get; set; }
        public virtual Korpe Korpe { get; set; }
        public virtual ICollection<Favoriti> Favoriti { get; set; }
        public virtual ICollection<Komentari> Komentari { get; set; }
        public virtual ICollection<Narudzbe> Narudzbe { get; set; }
        public virtual ICollection<Notifikacije> Notifikacije { get; set; }
        public virtual ICollection<Pitanja> Pitanja { get; set; }
    }
}
