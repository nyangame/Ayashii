using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Network
{
    public static class WebRequest
    {
        private static readonly HttpClient client = new HttpClient();

        /// <summary>
        /// GETリクエストを送信してレスポンスを取得
        /// </summary>
        public static async Task<string> GetRequest(string url)
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"GETリクエストエラー: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// POSTリクエストを送信してレスポンスを取得
        /// </summary>
        public static async Task<string> PostRequest(string url, string jsonContent)
        {
            try
            {
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return responseBody;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"POSTリクエストエラー: {ex.Message}");
                throw;
            }
        }
    }
}
