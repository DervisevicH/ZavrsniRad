using System;
using System.Collections.Generic;

namespace TDSalon.Data
{
    public partial class Narudzbe
    {
        public Narudzbe()
        {
            NarudzbaStavke = new HashSet<NarudzbaStavke>();
        }

        public int NarudzbaId { get; set; }
        public int? BrojNarudzbe { get; set; }
        public DateTime? Datum { get; set; }
        public bool? Procesirana { get; set; }
        public bool? Otkazano { get; set; }
        public int? RokZaDostavu { get; set; }
        public string Napomena { get; set; }
        public decimal? TroskoviDostave { get; set; }
        public decimal? Ukupno { get; set; }
        public string StatusNarudzbe { get; set; }
        public string Komentar { get; set; }
        public int? KupacId { get; set; }
        public int? ZaposlenikId { get; set; }

        public virtual Kupci Kupac { get; set; }
        public virtual Zaposlenici Zaposlenik { get; set; }
        public virtual ICollection<NarudzbaStavke> NarudzbaStavke { get; set; }
    }
}
