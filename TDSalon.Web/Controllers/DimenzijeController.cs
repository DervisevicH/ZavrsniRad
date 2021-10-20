using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TDSalon.Data;

namespace TDSalon.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DimenzijeController : ControllerBase
    {
        private TDSalondbContext _db;
        public DimenzijeController (TDSalondbContext db) { _db = db; }
       
    }
}
