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
        public Point[] GetMoves([FromBody]Point piece)
        {
            return Class1.GetMoves(piece);
        }


        [HttpGet("Move/{piece}/{destination}")]
        public void Move(Point piece, Point destination)
        {
            Class1.Move(piece, destination);
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
