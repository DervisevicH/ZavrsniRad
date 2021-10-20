using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TDSalon.Web.Models
{
    public class NarudzbeIndexVM
    {
        
            public int NarudzbaId { get; set; }
            public int BrojNarudzbe { get; set; }
            public string Kupac { get; set; }
            public DateTime Datum { get; set; }
            public int RokZaDostavu { get; set; }
            public string  StatusNarudzbe{ get; set; }
            public string Komentar{ get; set; }
            public string Napomena { get; set; }
            public decimal Ukupno{ get; set; }

    }
}
