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
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return;
            }

            throw new Exception("Something went terribly wrong and we don't care what!");
        }


        public static async Task<Guid> GetPlayerId(Guid gameID, bool crazyhouse)
        {
            var result = await client.GetAsync($"{baseURL}/game/GetPlayerID/{gameID}/{crazyhouse}");

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var temp = await result.Content.ReadAsStringAsync();
                temp = temp.Substring(1, temp.Length - 2);
                return Guid.Parse(temp);
            }

            throw new Exception("Something went terribly wrong and we don't care what!");
        }


        public static async Task<bool?> GetGameColor(Guid gameID, Guid playerID, bool wantsWhite)
        {
            var result = await client.GetAsync($"{baseURL}/game/GetGameColor/{gameID}/{playerID}/{wantsWhite}");

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var temp = await result.Content.ReadAsStringAsync();

                return bool.Parse(temp);
            }

            throw new Exception("Something went terribly wrong and we don't care what!");
        }


        public static async Task<Point[]> GetMoves(Guid gameID, Point piece)
        {
            //.NET 5 solution
            var options = new JsonSerializerOptions { IncludeFields = true, PropertyNamingPolicy = null };
            string json = JsonSerializer.Serialize(piece, options);

            //new string(Array.ConvertAll<byte, char>(s._content, n => (char)n))

            StringContent s = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var result = await client.PostAsync($"{baseURL}/game/GetMoves/{gameID}", s);
            
            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var temp = await result.Content.ReadAsStringAsync();
                var squares = JsonSerializer.Deserialize<Square[]>(temp, options);
                return squares.Select(s => s.ToPoint()).ToArray();
            }

            throw new Exception("Something went terribly wrong and we don't care what!");
        }


        public static async Task Move(Guid gameID, Guid playerID, Point piece, Point destination)
        {
            var result = await client.GetAsync($"{baseURL}/game/Move/{gameID}/{playerID}/{piece.X}/{piece.Y}/{destination.X}/{destination.Y}");

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return;
            }

            throw new Exception("Something went terribly wrong and we don't care what!");
        }


        public static async Task<bool> CheckForNoMoves(Guid gameID)
        {
            var result = await client.GetAsync($"{baseURL}/game/CheckForNoMoves/{gameID}");

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var temp = await result.Content.ReadAsStringAsync();

                return bool.Parse(temp);
            }

            throw new Exception("Something went terribly wrong and we don't care what!");
        }


        public static async Task<string> MakeFEN(Guid gameID)
        {
            var result = await client.GetAsync($"{baseURL}/game/MakeFEN/{gameID}");

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var temp = await result.Content.ReadAsStringAsync();

                return temp.ToString();
            }

            throw new Exception("Something went terribly wrong and we don't care what!");
        }

        public static async Task CheckPromotion(Guid gameID)
        {
            var result = await client.GetAsync($"{baseURL}/game/CheckPromotion/{gameID}");

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return;
            }

            throw new Exception("Something went terribly wrong and we don't care what!");
        }

        public static async Task Promote(Guid gameID, Guid playerID, string pieceChoice)
        {
            var result = await client.GetAsync($"{baseURL}/game/Promote/{gameID}/{playerID}/{pieceChoice}");

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return;
            }

            throw new Exception("Something went terribly wrong and we don't care what!");
        }

        public static async Task<bool> PlacePiece(Guid gameID, Guid playerID, PieceTypes piece, Point destination)
        {
            var options = new JsonSerializerOptions { IncludeFields = true, PropertyNamingPolicy = null };
            string json = JsonSerializer.Serialize(destination, options);

            StringContent s = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

            var result = await client.PostAsync($"{baseURL}/game/PlacePiece/{gameID}/{playerID}/{piece}", s);

            if (result.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var temp = await result.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<bool>(temp, options);
            }

            throw new Exception("Something went terribly wrong and we don't care what!");
        }
    }
}