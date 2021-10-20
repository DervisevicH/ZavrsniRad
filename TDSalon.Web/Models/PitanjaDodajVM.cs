using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TDSalon.Web.Models
{
    public class PitanjaDodajVM
    {
        public int PitanjeId { get; set; }
        [Required(ErrorMessage = "Unesite pitanje")]
        public string Pitanje { get; set; }
        public string ProizvodNaziv{ get; set; }
        public DateTime? Datum { get; set; }
        public int? KupacId { get; set; }
        public int? ProizvodId { get; set; }
    }
}
