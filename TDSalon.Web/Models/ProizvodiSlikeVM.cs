using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TDSalon.Web.Models
{
    public class ProizvodiSlikeVM
    {
        public IFormFile[] slike { get; set; }
    }
}
