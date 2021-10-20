using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TDSalon.Web.Models
{
    public class LoginVM
    {
        [StringLength(50, ErrorMessage = "Korisničko ime mora sadržavati najmanje tri karaktera", MinimumLength = 3)]
        public string KorisnickoIme { get; set; }
        [Required(ErrorMessage = "Unesite lozinku")]
        [DataType(DataType.Password)]
        public string Lozinka { get; set; }
        public bool ZapamtiLozinku { get; set; }
    }
}
