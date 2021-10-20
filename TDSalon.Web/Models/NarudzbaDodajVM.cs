using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDSalon.Data;

namespace TDSalon.Web.Models
{
    public class NarudzbaDodajVM
    {
        public int KorpaId { get; set; }
        public decimal Ukupno { get; set; }
        public decimal UkupnoZaPlatiti { get; set; }
        public decimal TroskoviDostave { get; set; }
        public List<Row> rows { get; set; }
        public Kupci Kupac { get; set; }
        public string Napomena{ get; set; }
        public int RokZaDostavu{ get; set; }
        public class Row
        {
            public int StavkaId { get; set; }
            public string NazivProizvoda { get; set; }
            public string Slika { get; set; }
            public decimal Kolicina { get; set; }
            public decimal Cijena { get; set; }
            public string Dimenzija { get; set; }
            public bool IsAkcija{ get; set; }
            public int ProizvodId{ get; set; }

        }
    }
}
