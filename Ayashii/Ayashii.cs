using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Ayashii
{
    static string[] CommandList = new string[] {
        "/c dir",
        "/c ipconfig",
        "/c echo このファイルをスタートアップに登録したよ(してない)",
        "/c echo これがバックドア体験プログラムだよ",
    };

    static public void DoAction(string msg)
    {
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
        using (var process = Process.Start(processStartInfo))
        {
            // 標準出力のストリームを最後まで読み込む
            responseString = process.StandardOutput.ReadToEnd();
            responseString = responseString.Replace("\n", "<br>");

            // プロセスが終了するのを待つ
            process.WaitForExit();
        }

        EventData evt = new EventData(0);
        evt.DataPack("Data", responseString);
        NetworkFront.Send(evt);

        //ユーザにお知らせ
        // プロセスの起動情報を設定
        var processStartInfo2 = new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = command + " & pause",
            RedirectStandardOutput = false, // 標準出力をリダイレクトする
            UseShellExecute = true,     // シェルを介さずにプロセスを起動する
            CreateNoWindow = false,       // ウィンドウを表示しない
        };
        Process.Start(processStartInfo2);
    }
}
