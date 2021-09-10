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

        [HttpGet("GetGameColor/{wantsWhite}")]
        public bool? GetGameColor(Guid playerId, bool wantsWhite)
        {
            return Class1.GetGameColor(playerId, wantsWhite);
        }


        [HttpGet("ResetBoard")]
        public void ResetBoard()
        {
            Class1.ResetBoard();
        }


        [HttpPost("GetMoves")]
        public Square[] GetMoves([FromBody]Square piece)
        {
            return Class1.GetMoves(piece);
        }


        [HttpGet("Move/{pieceX}/{pieceY}/{destinationX}/{destinationY}")]
        public void Move(int pieceX, int pieceY, int destinationX, int destinationY)
        {
            Class1.Move(new Square(pieceX, pieceY), new Square(destinationX, destinationY));
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

        [HttpGet("Promote/{pieceChoice}")]
        public void Promote(string pieceChoice)
        {
            Class1.Promote(pieceChoice);
        }
    }
}