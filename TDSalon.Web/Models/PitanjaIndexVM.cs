using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace TDSalon.Web.Models
{
    public class PitanjaIndexVM
    {
        public List<SelectListItem> ListaProizvoda { get; set; }
        public int ProizvodId { get; set; }
        public bool IsOdgovorena { get; set; }
        public List<PitanjaVM> ListaPitanja{ get; set; }
    }
}
