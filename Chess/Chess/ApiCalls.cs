using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public static class ApiCalls
    {
        static HttpClient client = new HttpClient();


        public static async Task ResetBoard()
        {
            var result = await client.GetAsync($"https://localhost:44399/game/ResetBoard");
            while (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                result = await client.GetAsync($"https://localhost:44399/game/ResetBoard");
            }
        }


        public static async Task<bool> GetMoves()
        {
            throw new Exception("this should return a Point[] not a bool");
            var result = await client.GetAsync($"https://localhost:44399/game/GetMoves");
            var temp = await result.Content.ReadAsStringAsync();

            return bool.Parse(temp);
        }


        public static async Task Move()
        {
            var result = await client.GetAsync($"https://localhost:44399/game/Move");
            while (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                result = await client.GetAsync($"https://localhost:44399/game/Move");
            }
        }


        public static async Task<bool> CheckForNoMoves()
        {
            var result = await client.GetAsync($"https://localhost:44399/game/CheckForNoMoves");
            var temp = await result.Content.ReadAsStringAsync();

            return bool.Parse(temp);
        }


        public static async Task<string> MakeFEN()
        {
            var result = await client.GetAsync($"https://localhost:44399/game/MakeFEN");
            var temp = await result.Content.ReadAsStringAsync();

            return temp.ToString();
        }
    }
}
