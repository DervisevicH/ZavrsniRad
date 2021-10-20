using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TDSalon.Web.Models
{
    public class AkcijeVM
    {
        public int AkcijaId { get; set; }
        public string Naziv { get; set; }
        public string DatumOd { get; set; }
        public string DatumDo { get; set; }
        public bool? IsAktivna { get; set; }
        public decimal? Postotak { get; set; }
    }
}
