using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TDSalon.Web.Models
{
    public class ProizvodiIndexVM
    {
        public int KategorijaId { get; set; }
        public List<SelectListItem> ListaKategorija { get; set; }
        public bool IsAktivan { get; set; }
        public string Naziv { get; set; }
        public List<ProizvodiVM> ListaProizvodi { get; set; }
        public class ProizvodiVM
           {
                public int ProizvodId { get; set; }
                public string Sifra { get; set; }
                public string Naziv { get; set; }
                public string Cijena { get; set; }
                public string Kolicina { get; set; }
                public string Kategorija { get; set; }
                public bool IsAktivan { get; set; }
                public bool? IsAkcija { get; set; }
                public string AkcijskaCijena { get; set; }
                public int Akcija{ get; set; }
        }
           
    }
}
