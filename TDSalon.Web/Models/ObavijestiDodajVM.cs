using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TDSalon.Web.Models
{
    public class ObavijestiDodajVM
    {
        public int ObavijestId { get; set; }
        public DateTime? DatumObjave { get; set; }
        [Required(ErrorMessage = "Unesite obavijest")]
        public string Obavijest { get; set; }       
        public int? ZaposlenikId { get; set; }
        [Required(ErrorMessage = "Unesite naslov")]
        public string Naslov { get; set; }
    }
}
