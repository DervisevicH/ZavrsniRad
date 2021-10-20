using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TDSalon.Web.Models
{
    public class KomentarDodajVM
    {
        public string Komentar { get; set; }
        public int KupacId { get; set; }
        public int ProizvodId { get; set; }
        public string Ime { get; set; }
        public string Email { get; set; }
        public string Naslov { get; set; }
        public int Ocjena { get; set; }
    }
}
