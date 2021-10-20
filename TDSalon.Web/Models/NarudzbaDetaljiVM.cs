using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TDSalon.Web.Models
{
    public class NarudzbaDetaljiVM
    {
        public int NarudzbaId { get; set; }
        public string Status { get; set; }
        public string Komentar { get; set; }
        public int BrojNarudzbe { get; set; }
        public DateTime Datum { get; set; }
        public string Kupac { get; set; }
        public string Adresa { get; set; }
        public int Telefon { get; set; }
        public string Napomena { get; set; }
        public List<Stavke> listaStavki { get; set; }
        public decimal Medusuma { get; set; }
        public decimal TroskoviDostave { get; set; }
        public decimal UkupnoZaPlatit { get; set; }
        public bool Procesirana { get; set; }
        public class Stavke
        {
            public int StavkaId { get; set; }
            public string Sifra { get; set; }
            public string Proizvod { get; set; }
            public string Dimenzija { get; set; }
            public decimal Cijena { get; set; }
            public decimal Kolicina { get; set; }
            public decimal Popust { get; set; }
            public decimal Ukupno { get; set; }
        }
    }
}
