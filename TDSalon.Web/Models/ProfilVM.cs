using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TDSalon.Web.Models
{
    public class ProfilVM
    {
        public int KupacId { get; set; }
        [Required(ErrorMessage = "Polje ime je obavezno")]
        public string Ime { get; set; }
        [Required(ErrorMessage = "Polje prezime je obavezno")]
        public string Prezime { get; set; }
        [Required(ErrorMessage = "Unesite email adresu")]
        [EmailAddress(ErrorMessage = "Niste unijeli validnu email adresu")]
        public string Email { get; set; }
        public string Telefon { get; set; }
        public string Spol{ get; set; }
    }
}
