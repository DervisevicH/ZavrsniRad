using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TDSalon.Web.Helper;

namespace TDSalon.Web.Models
{
    public class ProizvodiDodajVM
    {
        [Required(ErrorMessage = "Polje naziv je obavezno")]
        public string Naziv { get; set; }
        [Required(ErrorMessage = "Polje opis je obavezno")]
        public string Opis { get; set; }
        public string PreporucenoZa { get; set; }
        public string Boja { get; set; }
        [Required(ErrorMessage = "Polje zemlja porijekla je obavezno.")]
        public string NapravljenoU { get; set; }
        [RequiredGreaterThanZero(ErrorMessage = "Jedinica mjere je obavezno polje")]
        public int? JedinicaMjereId { get; set; }
        [RequiredGreaterThanZero(ErrorMessage = "Dobavljač je obavezno polje")]
        public int? DobavljacId { get; set; }
        [RequiredGreaterThanZero(ErrorMessage = "Kategorija je obavezno polje")]
        public int? KategorijaId { get; set; }       
        public List<SelectListItem> DobavljaciLista { get; set; }
        public List<SelectListItem> JedinicaMjereLista { get; set; }
        public List<SelectListItem> KategorijeLista { get; set; }
        public ProizvodiCijeneVM ProizvodiCijene{ get; set; }
    }
}
