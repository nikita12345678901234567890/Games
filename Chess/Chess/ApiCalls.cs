﻿using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
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
            var result = await client.GetAsync($"https://localhost:44399/game/ResetBoard");
            while (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                result = await client.GetAsync($"https://localhost:44399/game/ResetBoard");
            }
        }


        public static async Task<Point[]> GetMoves(Point piece)
        {
            string json = JsonSerializer.Serialize<Point>(piece);
            StringContent s = new StringContent(json, System.Text.Encoding.UTF8, "application/json");


            var result = await client.PostAsync($"https://localhost:44399/game/GetMoves", s);
            var temp = await result.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<Point[]>(temp);
        }


        public static async Task Move(Point piece, Point destination)
        {
            var result = await client.GetAsync($"https://localhost:44399/game/Move/{piece}/{destination}");
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
