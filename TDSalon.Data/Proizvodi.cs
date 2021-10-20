using System;
using System.Collections.Generic;

namespace TDSalon.Data
{
    public partial class Proizvodi
    {
        public Proizvodi()
        {
            AkcijeProizvodi = new HashSet<AkcijeProizvodi>();
            FavoritiStavke = new HashSet<FavoritiStavke>();
            KorpaProizvodi = new HashSet<KorpaProizvodi>();
            NarudzbaStavke = new HashSet<NarudzbaStavke>();
            Pitanja = new HashSet<Pitanja>();
        }

        public int ProizvodId { get; set; }
        public string Sifra { get; set; }
        public decimal? Cijena { get; set; }
        public decimal? Stanje { get; set; }
        public decimal? Prodato { get; set; }
        public int? ProizvodDetaljiId { get; set; }
        public int? DimenzijaId { get; set; }
        public bool? IsAkcija { get; set; }

        public virtual Dimenzije Dimenzija { get; set; }
        public virtual ProizvodiDetalji ProizvodDetalji { get; set; }
        public virtual ICollection<AkcijeProizvodi> AkcijeProizvodi { get; set; }
        public virtual ICollection<FavoritiStavke> FavoritiStavke { get; set; }
        public virtual ICollection<KorpaProizvodi> KorpaProizvodi { get; set; }
        public virtual ICollection<NarudzbaStavke> NarudzbaStavke { get; set; }
        public virtual ICollection<Pitanja> Pitanja { get; set; }
    }
}
