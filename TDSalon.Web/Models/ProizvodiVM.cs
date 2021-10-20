using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TDSalon.Web.Models
{
    public class ProizvodiVM
    {
        public List<ProizvodRow> Rows{ get; set; }
        public List<SelectListItem> ListaDimenzija{ get; set; }
        public List<SelectListItem> ListaKategorija { get; set; }
        public int? DimenzijaId { get; set; }
        public string Boja{ get; set; }
        public int KategorijaId{ get; set; }
        public string PreporucenoZa{ get; set; }
        public decimal? CijenaOd{ get; set; }
        public decimal? CijenaDo{ get; set; }
        public string Pretraga{ get; set; }     
    }
}
