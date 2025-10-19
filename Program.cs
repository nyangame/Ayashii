using System;
using System.Net;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

public class AyashiiApplication
{
    static string[] CommandList = new string[] {
        "/c dir",
        "/c ipconfig",
        "/c git config --list",
        "/c echo このファイルをスタートアップに登録したよ(してない)",
        "/c echo これがバックドア体験プログラムだよ",
    };


    public static async Task Main(string[] args)
    {
        // コンソールでヒントを出力
        Process.Start("cmd.exe", $"/c echo Page UUID：28f39cbfbab9806ba384fafbdbe69994 & pause");

        // HTTPサーバーの設定
        var listener = new HttpListener();

        // 起動時の処理
        try
        {
            // PCのユーザー名を取得
            string port = "2222";
            string userName = Environment.UserName;
            string hostName = Environment.MachineName;
            string myUrl = String.Format("http://{0}:{1}/", GetLocalIPv4Address(), port);

            //NetworkInfoPost.Send(userName, hostName);

            string url = String.Format("http://{0}:{1}/", "127.0.0.1", port);
            listener.Prefixes.Add(url);

            // HTTPサーバーを起動
            listener.Start();
            Console.WriteLine($"HTTPサーバーを開始しました。リクエスト待機中... -> {url}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"起動中にエラーが発生しました: {ex.Message}");
            return;
        }

        // 3. HTTPリクエストの待機ループ
        while (true)
        {
            // リクエストが来るまでここで待機
            HttpListenerContext context = await listener.GetContextAsync();
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;

            if (request == null) continue;
            if (request.Url == null) continue;
            if (request.Url.ToString().IndexOf("favicon") != -1) continue;

            Console.WriteLine($"リクエストを受信しました: {request.Url}");

            string command = "";
            
            Random rand = new Random();
            int next = rand.Next(CommandList.Length);
            command = CommandList[next];

            // プロセスの起動情報を設定
            var processStartInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = command,
                RedirectStandardOutput = true, // 標準出力をリダイレクトする
                UseShellExecute = false,     // シェルを介さずにプロセスを起動する
                CreateNoWindow = true,       // ウィンドウを表示しない
            };

            string responseString = "";

            // プロセスを開始し、出力を取得して終了を待つ
            using (var process = new Process())
            {
                process.StartInfo = processStartInfo;
                process.Start();

                // 標準出力のストリームを最後まで読み込む
                responseString = process.StandardOutput.ReadToEnd();
                responseString = responseString.Replace("\n", "<br>");

                // プロセスが終了するのを待つ
                process.WaitForExit();
            }

            //
            byte[] buffer = Encoding.UTF8.GetBytes(responseString);

            // レスポンスの設定と送信
            response.ContentType = "text/html; charset=utf-8";
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
            response.OutputStream.Close();

            //ユーザにお知らせ
            Process.Start("cmd.exe", command + " & pause");
        }
    }
    
    /// <summary>
     /// 利用可能なローカルIPv4アドレスを取得します。
     /// </summary>
     /// <returns>見つかったIPv4アドレスの文字列。見つからない場合は空文字列。</returns>
    public static string GetLocalIPv4Address()
    {
        // 1. 自分のホスト名を取得
        var hostName = Dns.GetHostName();

        // 2. ホスト名からIPアドレスのリストを取得
        var host = Dns.GetHostEntry(hostName);

        // 3. IPアドレスのリストから、IPv4のものを探す
        //    (AddressFamily.InterNetwork が IPv4 を示す)
        var ipv4Address = host.AddressList.FirstOrDefault(
            addr => addr.AddressFamily == AddressFamily.InterNetwork
        );

        return ipv4Address?.ToString() ?? string.Empty;
    }
}