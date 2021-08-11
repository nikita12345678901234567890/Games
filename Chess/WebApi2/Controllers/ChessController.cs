using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApi2.Controllers
{
    [Route("[controller]")]
    public class ChessController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("Do")]
        [HttpGet]
        public int DoSomething()
        {
            return 5;
        }

        [Route("Other")]
        [HttpGet]
        public int OtherFunction(int num)
        {
            return 5 * num;
        }

    }
}
