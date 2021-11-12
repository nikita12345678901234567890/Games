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
        static HttpClient client;
        static readonly string baseURL = System.Configuration.ConfigurationManager.AppSettings["ServerBaseURL"];


        static ApiCalls()
        {
            client = new HttpClient();
        }

        

        public static async Task ResetBoard(Guid gameID, Guid playerID)
        {
            var result = await client.GetAsync($"{baseURL}/game/ResetBoard/{gameID}/{playerID}");
        }


        public static async Task<Guid> GetPlayerId(Guid gameID)
        {
            var result = await client.GetAsync($"{baseURL}/game/GetPlayerID/{gameID}");
            var temp = await result.Content.ReadAsStringAsync();
            temp = temp.Substring(1, temp.Length - 2);
            return Guid.Parse(temp);
        }


        public static async Task<bool?> GetGameColor(Guid gameID, Guid playerID, bool wantsWhite)
        {
            var result = await client.GetAsync($"{baseURL}/game/GetGameColor/{gameID}/{playerID}/{wantsWhite}");
            var temp = await result.Content.ReadAsStringAsync();

            return bool.Parse(temp);
        }


        public static async Task<Point[]> GetMoves(Guid gameID, Point piece)
        {
            //.NET 5 solution
            var options = new JsonSerializerOptions { IncludeFields = true, PropertyNamingPolicy = null };
            string json = JsonSerializer.Serialize(piece, options);

            //new string(Array.ConvertAll<byte, char>(s._content, n => (char)n))

            StringContent s = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var result = await client.PostAsync($"{baseURL}/game/GetMoves/{gameID}", s);
            var temp = await result.Content.ReadAsStringAsync();

            var squares = JsonSerializer.Deserialize<Square[]>(temp, options);
            return squares.Select(s => s.ToPoint()).ToArray();
        }


        public static async Task Move(Guid gameID, Guid playerID, Point piece, Point destination)
        {
            var result = await client.GetAsync($"{baseURL}/game/Move/{gameID}/{playerID}/{piece.X}/{piece.Y}/{destination.X}/{destination.Y}");
        }


        public static async Task<bool> CheckForNoMoves(Guid gameID)
        {
            var result = await client.GetAsync($"{baseURL}/game/CheckForNoMoves/{gameID}");
            var temp = await result.Content.ReadAsStringAsync();

            return bool.Parse(temp);
        }


        public static async Task<string> MakeFEN(Guid gameID)
        {
            var result = await client.GetAsync($"{baseURL}/game/MakeFEN/{gameID}");
            var temp = await result.Content.ReadAsStringAsync();

            return temp.ToString();
        }

        public static async Task CheckPromotion(Guid gameID)
        {
            var result = await client.GetAsync($"{baseURL}/game/CheckPromotion/{gameID}");
        }

        public static async Task Promote(Guid gameID, Guid playerID, string pieceChoice)
        {
            var result = await client.GetAsync($"{baseURL}/game/Promote/{gameID}/{playerID}/{pieceChoice}");
        }
    }
}