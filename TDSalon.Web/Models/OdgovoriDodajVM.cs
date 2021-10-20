using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TDSalon.Web.Models
{
    public class OdgovoriDodajVM
    {
        [Required(ErrorMessage = "Unesite odgovor")]
        public string Odgovor { get; set; }
        public int PitanjeId { get; set; }

    }
}
