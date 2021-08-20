using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedLibrary;
using WebApi2.Models;
using Microsoft.Xna.Framework;

namespace WebApi2.Controllers
{
    [ApiController]
    [Route("game")]
    public class ChessController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        //Start of example:
        [HttpGet("Do")]
        public int DoSomething()
        {
            return 5;
        }

        [HttpGet("Other/{num}")]
        public int OtherFunction(int num)
        {
            return 5 * num;
        }
        //end of example.

        //Start of Jason example:
        [HttpPost("Test")]
        public string TestFunction([FromBody]Person p)
        {

            return "blah";
        }
        //End of Jason example.


        [HttpGet("ResetBoard")]
        public void ResetBoard()
        {
            Class1.ResetBoard();
        }


        [HttpGet("GetMoves")]
        public void GetMoves()
        {
            throw new InvalidTimeZoneException("this should take in a Point and return Point[]");
     //       Class1.GetMoves();
        }


        [HttpGet("Move")]
        public void Move()
        {
        //    Class1.Move();
        }


        [HttpGet("CheckForNoMoves")]
        public bool CheckForNoMoves()
        {
            return Class1.CheckForNoMoves();
        }


        [HttpGet("MakeFEN")]
        public string MakeFEN()
        {
            return Class1.MakeFEN();
        }
    }
}
