using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TDSalon.Data;

namespace TDSalon.Web.Models
{
    public class ProizvodDetaljiVM
    {
        public int ProizvodDetaljiId { get; set; }
        public int ProizvodId { get; set; }
        public List<string> Slike{ get; set; }
        public int DimenzijaId { get; set; }
        public string Naziv{ get; set; }
        public string Cijena{ get; set; }
        public string Boja{ get; set; }
        public string Kategorija{ get; set; }
        public string Opis{ get; set; }
        public string PreporucenoZa { get; set; }
        public string ZemljaPorijekla { get; set; }
        public List<SelectListItem> ListaDimenzija{ get; set; }
        public int BrojKomentara { get; set; }
    }
}
