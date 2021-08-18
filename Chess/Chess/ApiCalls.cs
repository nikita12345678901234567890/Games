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

        public static async Task<bool> CheckForNoMoves()
        {
            var result = await client.GetAsync($"https://localhost:44399/game/CheckForNoMoves");
            var temp = await result.Content.ReadAsStringAsync();

            return bool.Parse(temp);
        }


    }
}
