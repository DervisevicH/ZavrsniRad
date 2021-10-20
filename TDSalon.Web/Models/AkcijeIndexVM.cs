using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDSalon.Data;

namespace TDSalon.Web.Models
{
    public class AkcijeIndexVM
    {
        public string Naziv { get; set; }
        public DateTime AkcijaOd { get; set; }
        public DateTime AkcijaDo { get; set; }
        public bool IsAktivna { get; set; }
        public List<AkcijeVM> ListaAkcija{ get; set;}
        
    }
}
