using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TDSalon.Web.Models
{
    public class KupciDodajInfoVM
    {
        public int KupacId { get; set; }
        [Required(ErrorMessage ="Polje ime je obavezno")]
        public string Ime { get; set; }
        [Required(ErrorMessage = "Polje prezime je obavezno")]
        public string Prezime { get; set; }
        [Required(ErrorMessage = "Unesite email adresu")]
        [EmailAddress(ErrorMessage = "Niste unijeli validnu email adresu")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Unesite broj telefona")]
        public string Telefon{ get; set; }
        [Required(ErrorMessage = "Unesite adresu")]
        public string Adresa { get; set; }
        [Range(1, int.MaxValue,ErrorMessage ="Odaberite kanton")]
        [Required(ErrorMessage ="Odaberite kanton")]

        public int KantonId { get; set; }
        public List<SelectListItem> listaKantona { get; set; }
        [Required(ErrorMessage = "Odaberite grad")]
        [Range(1, int.MaxValue, ErrorMessage = "Odaberite grad")]

        public int GradId { get; set; }
        public List<SelectListItem> listaGradova { get; set; }        
    }
}
