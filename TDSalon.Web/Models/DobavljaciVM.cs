using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TDSalon.Web.Models
{
    public class DobavljaciVM
    {
        public int DobavljacId { get; set; }
        [Required(ErrorMessage = "Polje naziv je obavezno")]
        public string NazivFirme { get; set; }
        public string KontaktOsoba { get; set; }
        [Required(ErrorMessage = "Polje telefon je obavezno")]
        public string Telefon { get; set; }
        [Required(ErrorMessage = "Polje email je obavezno")]
        public string Email { get; set; }
    }
}
