using System;
using System.Collections.Generic;

namespace TDSalon.Data
{
    public partial class ProizvodiDetalji
    {
        public ProizvodiDetalji()
        {
            Komentari = new HashSet<Komentari>();
            Proizvodi = new HashSet<Proizvodi>();
            Slike = new HashSet<Slike>();
        }

        public int ProizvodDetaljiId { get; set; }
        public string Naziv { get; set; }
        public string Opis { get; set; }
        public string Boja { get; set; }
        public string PreporucenoZa { get; set; }
        public string NapravljenoU { get; set; }
        public DateTime? DatumIzmjene { get; set; }
        public bool? IsAktivan { get; set; }
        public int? JedinicaMjereId { get; set; }
        public int? DobavljacId { get; set; }
        public int? KategorijaId { get; set; }

        public virtual Dobavljaci Dobavljac { get; set; }
        public virtual JediniceMjere JedinicaMjere { get; set; }
        public virtual Kategorije Kategorija { get; set; }
        public virtual ICollection<Komentari> Komentari { get; set; }
        public virtual ICollection<Proizvodi> Proizvodi { get; set; }
        public virtual ICollection<Slike> Slike { get; set; }
    }
}
