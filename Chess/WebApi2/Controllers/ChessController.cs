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
        static Dictionary<Guid, ChessGame> games = new Dictionary<Guid, ChessGame>();

        [HttpGet("GetPlayerId/{gameID}/{crazyhouse}")]
        public ActionResult<Guid> GetPlayerId(Guid gameID, bool crazyhouse)
        {
            Guid id = Guid.NewGuid();
            if (!games.ContainsKey(gameID))
            {
                if (crazyhouse)
                {
                    games[gameID] = new CrazyhouseGame();
                }
                else
                {
                    games[gameID] = new();
                }
            }

            return id;
        }

        [HttpGet("GetGameColor/{gameID}/{playerID}/{wantsWhite}")]
        public bool? GetGameColor(Guid gameID, Guid playerID, bool wantsWhite)
        {
            bool? result = games[gameID].GetGameColor(playerID, wantsWhite);

            games[gameID].ResetBoard(playerID);

            return result;
        }


        [HttpGet("ResetBoard/{gameID}/{playerID}")]
        public void ResetBoard(Guid gameID, Guid playerID)
        {
            games[gameID].ResetBoard(playerID);
        }


        [HttpPost("GetMoves/{gameID}")]
        public Square[] GetMoves(Guid gameID, [FromBody]Square piece)
        {
            return games[gameID].GetMoves(piece);
        }


        [HttpGet("Move/{gameID}/{playerID}/{pieceX}/{pieceY}/{destinationX}/{destinationY}")]
        public void Move(Guid gameID, Guid playerID, int pieceX, int pieceY, int destinationX, int destinationY)
        {
            games[gameID].Move(playerID, new Square(pieceX, pieceY), new Square(destinationX, destinationY));
        }


        [HttpGet("CheckForNoMoves/{gameID}")]
        public bool CheckForNoMoves(Guid gameID)
        {
            return games[gameID].CheckForNoMoves();
        }


        [HttpGet("MakeFEN/{gameID}")]
        public string MakeFEN(Guid gameID)
        {
            return games[gameID].MakeFEN();
        }

        [HttpGet("CheckPromotion/{gameID}")]
        public void CheckPromotion(Guid gameID)
        {
            games[gameID].CheckPromotion();
        }

        [HttpGet("Promote/{gameID}/{playerID}/{pieceChoice}")]
        public void Promote(Guid gameID, Guid playerID, string pieceChoice)
        {
            games[gameID].Promote(playerID, pieceChoice);
        }

        [HttpPost("PlacePiece/{gameID}/{playerID}/{piece}")]
        public ActionResult<bool> PlacePiece(Guid gameID, Guid playerID, PieceTypes piece, [FromBody] Square destination)
        {
            ChessGame game = games[gameID];
            if (game is CrazyhouseGame chgame)
            {
                return chgame.PlacePiece(playerID, piece, destination);
            }
            else
            {
                return BadRequest(false);
            }
        }
    }
}