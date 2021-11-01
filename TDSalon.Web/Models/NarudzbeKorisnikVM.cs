using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TDSalon.Web.Models
{
    public class NarudzbeKorisnikVM
    {
        public List<Row> rows { get; set; }

        public class Row
        {
            public int NarudzbaId { get; set; }
            public DateTime Datum { get; set; }
            public int KorisnikId { get; set; }
            public string Status { get; set; }
            public decimal Ukupno { get; set; }
            public int BrojNarudzbe { get; set; }
            public string Komentar{ get; set; }
        }
    }
}
