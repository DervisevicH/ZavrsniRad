using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TDSalon.Web.Models
{
    public class RegistracijaVM
    {
        public int KupacId { get; set; }
        [Required(ErrorMessage = "Unesite email adresu")]
        [EmailAddress(ErrorMessage = "Niste unijeli validnu email adresu")]
        public string Email { get; set; }
        [MinLength(5, ErrorMessage = "Korisničko ime je prekratko")]
        [MaxLength(15)]
        [Required(ErrorMessage = "Unesite korisničko ime")]
        public string Username { get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Unesite lozinku")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Niste unijeli iste lozinke")]
        [Required(ErrorMessage = "Ponovite lozinku")]
        public string PasswordRepeat { get; set; }
        [Required(ErrorMessage = "Odaberite spol")]
        public string Spol { get; set; }
    }
}
