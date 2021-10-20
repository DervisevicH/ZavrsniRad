using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TDSalon.Web.Models
{
    public class ProfilAdreseVM
    {
        public int KupacId { get; set; }
        public string Adresa { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Odaberite kanton")]
        [Required(ErrorMessage = "Odaberite kanton")]

        public int KantonId { get; set; }
        public List<SelectListItem> listaKantona { get; set; }
        [Required(ErrorMessage = "Odaberite grad")]
        [Range(1, int.MaxValue, ErrorMessage = "Odaberite grad")]

        public int GradId { get; set; }
        public List<SelectListItem> listaGradova { get; set; }
    }
}
