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


        [HttpGet("GetPlayerId")]
        public Guid GetPlayerId()
        {
            return Guid.NewGuid();
        }

        [HttpGet("GetGameColor/{playerID}/{wantsWhite}")]
        public bool? GetGameColor(Guid playerID, bool wantsWhite)
        {
            return Class1.GetGameColor(playerID, wantsWhite);
        }


        [HttpGet("ResetBoard/{playerID}")]
        public void ResetBoard(Guid playerID)
        {
            Class1.ResetBoard(playerID);
        }


        [HttpPost("GetMoves")]
        public Square[] GetMoves([FromBody]Square piece)
        {
            return Class1.GetMoves(piece);
        }


        [HttpGet("Move/{playerID}/{pieceX}/{pieceY}/{destinationX}/{destinationY}")]
        public void Move(Guid playerID, int pieceX, int pieceY, int destinationX, int destinationY)
        {
            Class1.Move(playerID, new Square(pieceX, pieceY), new Square(destinationX, destinationY));
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

        [HttpGet("CheckPromotion")]
        public void CheckPromotion()
        {
            Class1.CheckPromotion();
        }

        [HttpGet("Promote/{playerID}/{pieceChoice}")]
        public void Promote(Guid playerID, string pieceChoice)
        {
            Class1.Promote(playerID, pieceChoice);
        }
    }
}