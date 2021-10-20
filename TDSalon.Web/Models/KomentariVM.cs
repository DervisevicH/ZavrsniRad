using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TDSalon.Web.Models
{
    public class KomentariVM
    {
        public List<Rows> rows { get; set; }

        public class Rows
        {
            public int KomentarId { get; set; }
            public string Komentar { get; set; }
            public string Datum { get; set; }
            public int KorisnikId { get; set; }
            public string Korisnik { get; set; }
            public int ProizvodId { get; set; }
            public string SlikaProfila { get; set; }
            public string Naslov { get; set; }
            public int Ocjena { get; set; }
        }
    }
}
