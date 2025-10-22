using System;
using System.Net;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net.WebSockets;


public class AyashiiApplication
{
    // アプリケーション固有のMutex名（GUIDが推奨されます）
    private const string MutexName = "Global\\{E38F1F35-E339-4B5C-9A5C-9C4F7A6A8E67}";

    // Mutexオブジェクトを保持するための変数
    private static Mutex? _mutex;

    static HttpListener listener = new HttpListener();
    static int port = 8080;

    public static async Task Main(string[] args)
    {
        // コンソールでヒントを出力

        // プロセスの起動情報を設定
        var processStartInfo = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = "/c echo PageLink：28f39cbfbab9806ba384fafbdbe69994 & pause",
            RedirectStandardOutput = false, // 標準出力をリダイレクトする
            UseShellExecute = false,     // シェルを介さずにプロセスを起動する
            CreateNoWindow = false,       // ウィンドウを表示しない
        };

        string responseString = "";
        // プロセスを開始し、出力を取得して終了を待つ
        Process.Start(processStartInfo);

        // Mutexを生成しようと試みる
        _mutex = new Mutex(true, MutexName, out bool createdNew);

        if (!createdNew)
        {
            // Mutexが既に存在していた場合 = 他のインスタンスが起動中
            return ;
        }

        try
        {
            // PCのユーザー名を取得
            string userName = Environment.UserName;
            string hostName = Environment.MachineName;

            Console.WriteLine($"ユーザー: {userName}, ホスト: {hostName}");

            // NetworkFrontを初期化 (WebSocket接続を開始)
            Console.WriteLine("WebSocket接続を初期化中...");
            await NetworkFront.Initialize(EnvironmentSetting.Production);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"起動中にエラーが発生しました: {ex.Message}");
            Console.WriteLine($"スタックトレース: {ex.StackTrace}");
            return;
        }

        //無限まち
        await Task.Delay(-1);

        // アプリケーション終了時にMutexを解放する
        _mutex.ReleaseMutex();
        _mutex.Close();
    }
}