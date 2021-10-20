using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TDSalon.Web.Models
{
    public class ProizvodiCijeneVM
    {
        public List<RowsDetalji> RowsDetalji { get; set; }
        public int KategorijaId { get; set; }    
    }
    public class RowsDetalji
    {
        public int ProizvodID { get; set; }
        public string Sifra { get; set; }
        public decimal? Cijena { get; set; }
        public decimal? Stanje { get; set; }
        public int? DimenzijaId { get; set; }
        public string Dimenzija { get; set; }
    }
}

