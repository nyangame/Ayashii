using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

public class NetworkInfoPost
{
    // HttpClientは再利用することが推奨されています
    private static readonly HttpClient client = new HttpClient();

    // AWSの情報集約場所のリンク
    const string EventURL = "https://jyl5w9zfz3.execute-api.ap-northeast-1.amazonaws.com/default/Event";


    public static async Task Send(string name, string url)
    {
        // 送信するデータ (JSON形式の例)
        var jsonData = String.Format("{\"name\":\"{0}\", \"age\":}",name, url);

        // HttpContentを作成
        var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

        try
        {
            // POSTリクエストを送信
            HttpResponseMessage response = await client.PostAsync(EventURL, content);

            // レスポンスが成功したか確認
            response.EnsureSuccessStatusCode();

            // レスポンスボディを文字列として読み取る
            string responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine("▼ Response Body");
            Console.WriteLine(responseBody);
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine($"\nException Caught!");
            Console.WriteLine($"Message: {e.Message}");
        }
    }
}