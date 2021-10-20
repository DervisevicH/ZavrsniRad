using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TDSalon.Web.Models
{
    public class ProfilLozinkeVM
    {
        public int KupacId { get; set; }
        [DataType(DataType.Password)]
        public string StaraLozinka{ get; set; }
        public string NovaLozinka{ get; set; }

        [DataType(DataType.Password)]
        [Compare("NovaLozinka", ErrorMessage = "Niste unijeli iste lozinke")]
        public string NovaLozinkaPotvrda { get; set; }
        
    }
}
