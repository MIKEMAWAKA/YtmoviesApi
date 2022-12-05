using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using YtmoviesApi.Model.DTO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace YtmoviesApi.Controllers
{
    [Route("api/[controller]/{action}")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
      
        public IActionResult GetData()
        {
            var status = new Status();
            status.StatusCode = 1;
            status.Message = "Data from admin controller";
            return Ok(status);
        }
    }
}

