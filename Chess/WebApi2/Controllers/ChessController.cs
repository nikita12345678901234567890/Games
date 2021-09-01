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


        [HttpPost("GetMoves")]
        public Point[] GetMoves([FromBody]Point piece)
        {
            return Class1.GetMoves(piece);
        }


        [HttpGet("Move/{pieceX}/{pieceY}/{destinationX}/{destinationY}")]
        public void Move(int pieceX, int pieceY, int destinationX, int destinationY)
        {
            Class1.Move(new Point(pieceX, pieceY), new Point(destinationX, destinationY));
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


        [HttpGet("Promote/{pieceChoice}")]
        public void Promote(string pieceChoice)
        {
            Class1.Promote(pieceChoice);
        }
    }
}