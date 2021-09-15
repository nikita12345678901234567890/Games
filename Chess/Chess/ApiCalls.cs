using Microsoft.Xna.Framework;
using SharedLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Chess
{

    public static class ApiCalls
    {
        static HttpClient client = new HttpClient();


        public static async Task ResetBoard(Guid playerID)
        {
            var result = await client.GetAsync($"https://localhost:5001/game/ResetBoard/{playerID}");
        }


        public static async Task<Guid> GetPlayerId()
        {
           var result = await client.GetAsync($"https://localhost:5001/game/GetPlayerID");
            var temp = await result.Content.ReadAsStringAsync();
            temp = temp.Substring(1, temp.Length - 2);
            return Guid.Parse(temp);
        }


        public static async Task<bool?> GetGameColor(Guid playerID, bool wantsWhite)
        {
            var result = await client.GetAsync($"https://localhost:5001/game/GetGameColor/{playerID}/{wantsWhite}");
            var temp = await result.Content.ReadAsStringAsync();

            return bool.Parse(temp);
        }


        public static async Task<Point[]> GetMoves(Point piece)
        {
            //.NET 5 solution
            var options = new JsonSerializerOptions { IncludeFields = true, PropertyNamingPolicy = null };
            string json = JsonSerializer.Serialize(piece, options);

            //new string(Array.ConvertAll<byte, char>(s._content, n => (char)n))

            StringContent s = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var result = await client.PostAsync($"https://localhost:5001/game/GetMoves", s);
            var temp = await result.Content.ReadAsStringAsync();

            var squares = JsonSerializer.Deserialize<Square[]>(temp, options);
            return squares.Select(s => s.ToPoint()).ToArray();
        }


        public static async Task Move(Guid playerID, Point piece, Point destination)
        {
            var result = await client.GetAsync($"https://localhost:5001/game/Move/{playerID}/{piece.X}/{piece.Y}/{destination.X}/{destination.Y}");
        }


        public static async Task<bool> CheckForNoMoves()
        {
            var result = await client.GetAsync($"https://localhost:5001/game/CheckForNoMoves");
            var temp = await result.Content.ReadAsStringAsync();

            return bool.Parse(temp);
        }


        public static async Task<string> MakeFEN()
        {
            var result = await client.GetAsync($"https://localhost:5001/game/MakeFEN");
            var temp = await result.Content.ReadAsStringAsync();

            return temp.ToString();
        }

        public static async Task CheckPromotion()
        {
            var result = await client.GetAsync($"https://localhost:5001/game/CheckPromotion");
        }

        public static async Task Promote(Guid playerID, string pieceChoice)
        {
            var result = await client.GetAsync($"https://localhost:5001/game/Promote/{playerID}/{pieceChoice}");
        }
    }
}