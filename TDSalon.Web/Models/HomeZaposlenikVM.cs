using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TDSalon.Web.Models
{
    public class HomeZaposlenikVM
    {
        public int UkupnoNarudzbi { get; set; }
        public int UkupnoKupaca { get; set; }
        public int UkupnoPitanja { get; set; }
        public decimal UkupnoZaradjeno { get; set; }
        public List<KupacRow> ListaKupaca { get; set; }
        public List<NarudzbaRow> ListaNarudzbi { get; set; }

    }
    public class KupacRow
    {
        public string ImePrezime { get; set; }
        public DateTime DatumRegistracije { get; set; }
        public string Spol{ get; set; }
        public string KorisnickoIme { get; set; }
    }
    public class NarudzbaRow
    {
        public string BrojNarudzbe { get; set; }
        public string Kupac { get; set; }
        public DateTime Datum { get; set; }
        public decimal Ukupno { get; set; }
        public string Status { get; set; }
        public int  NarudzbaId { get; set; }
    }
}
