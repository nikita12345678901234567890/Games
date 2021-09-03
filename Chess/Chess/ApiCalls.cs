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


        public static async Task ResetBoard()
        {
            var result = await client.GetAsync($"https://localhost:5001/game/ResetBoard");
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


        public static async Task Move(Point piece, Point destination)
        {
            var result = await client.GetAsync($"https://localhost:5001/game/Move/{piece.X}/{piece.Y}/{destination.X}/{destination.Y}");
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


        public static async Task Promote(string pieceChoice)
        {
            var result = await client.GetAsync($"https://localhost:5001/game/Promote/{pieceChoice}");
        }
    }
}