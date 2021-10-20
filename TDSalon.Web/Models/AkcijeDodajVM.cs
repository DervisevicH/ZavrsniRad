using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using TDSalon.Data;

namespace TDSalon.Web.Models
{
    public class AkcijeDodajVM
    {
        public int AkcijaId { get; set; }
        public string Naziv{ get; set; }
        public DateTime AkcijaOd{ get; set; }
        public DateTime AkcijaDo{ get; set; }
        public int Postotak { get; set; }
        public bool IsAktivna{ get; set; }
        public bool IsObavijesti{ get; set; }
        public int ProizvodId { get; set; }
        public List<SelectListItem> ListaProizvoda{ get; set; }
    }
}