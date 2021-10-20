using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TDSalon.Web.Models
{
    public class AkcijeUrediVM
    {
        public int AkcijaId { get; set; }
        public string Naziv { get; set; }
        public DateTime DatumOd { get; set; }
        public DateTime DatumDo { get; set; }
        public int Postotak { get; set; }
        public bool IsAktivna { get; set; }
        public int ProizvodId { get; set; }
        public List<SelectListItem> ListaProizvoda { get; set; }
        public List<AkcijeProizvodiVM> AkcijeProizvodi { get; set; }

    }
}
