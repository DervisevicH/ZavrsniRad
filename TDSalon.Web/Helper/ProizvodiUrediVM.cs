using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using TDSalon.Data;

namespace TDSalon.Web.Helper
{
    public class ProizvodiUrediVM
    {
        public int ProizvodId { get; set; }
        [Required(ErrorMessage = "Polje naziv je obavezno")]
        public string Naziv { get; set; }
        [Required(ErrorMessage = "Polje opis je obavezno")]
        public string Opis { get; set; }
        public string Boja { get; set; }
        [Required(ErrorMessage = "Polje zemlja porijekla je obavezno.")]
        public string NapravljenoU { get; set; }
        [RequiredGreaterThanZero(ErrorMessage = "Jedinica mjere je obavezno polje")]
        public int? DimenzijaId { get; set; }
        public List<SelectListItem>  DimenzijeLista{ get; set; }
        public decimal Cijena { get; set; }
        public decimal Kolicina { get; set; }
        public string Sifra { get; set; }
        [FileExtensions(ErrorMessage = "Niste odabrali pravilan format slike", Extensions = ".jpg, .jpeg, .gif, .png")]
        public IFormFile[] slikeForm { get; set; }

        public List<Slike> slike { get; set; }
        public int KategorijaId { get; set; }
        public string PreporucenoZa{ get; set; }

    }
}
