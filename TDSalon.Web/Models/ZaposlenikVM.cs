using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TDSalon.Web.Models
{
    public class ZaposlenikVM
    {
        public int ZaposlenikId { get; set; }
        public string Ime { get; set; }
        public string Prezime { get; set; }
        public string Email { get; set; }
        public string Telefon { get; set; }
        [DataType(DataType.Password)]
        public string StaraLozinka { get; set; }
        public string NovaLozinka { get; set; }

        [DataType(DataType.Password)]
        [Compare("NovaLozinka", ErrorMessage = "Niste unijeli iste lozinke")]
        public string NovaLozinkaPotvrda { get; set; }
    }
}
