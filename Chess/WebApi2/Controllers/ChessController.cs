using Microsoft.AspNetCore.Mvc;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SharedLibrary;
using WebApi2.Models;

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

        [HttpGet("Contains")]
        public bool Contains()
        {
            return Class1.Contains();
        }

        [HttpGet("IndexOf")]
        public int IndexOf()
        {
            return Class1.IndexOf();
        }

        [HttpGet("IsChecking")]
        public bool IsChecking()
        {
            return Class1.IsChecking();
        }

        [HttpGet("UnderAttack")]
        public bool UnderAttack()
        {
            return Class1.UnderAttack();
        }

        [HttpGet("Move")]
        public void Move()
        {
            Class1.Move();
        }

        [HttpGet("CheckForNoMoves")]
        public bool CheckForNoMoves()
        {
            return Class1.CheckForNoMoves();
        }

        [HttpGet("DecodeFEN")]
        public void DecodeFEN()
        {
            return Class1.DecodeFEN();
        }
    }
}
