using System;
using WebSocketSharp;

/// <summary>
/// WebSocketのクライアント実装
/// </summary>
public class WebSocketCli
{
    private WebSocket _ws;
    private Action<byte[]> _onMessage;

    /// <summary>
    /// WebSocketサーバーに接続
    /// </summary>
    public void Connect(string address, Action<byte[]> onMessage)
    {
        _onMessage = onMessage;

        try
        {
            _ws = new WebSocket(address);

            _ws.OnOpen += (sender, e) =>
            {
                Console.WriteLine($"WebSocket接続成功: {address}");
            };

            _ws.OnMessage += (sender, e) =>
            {
                if (e.IsBinary)
                {
                    _onMessage?.Invoke(e.RawData);
                }
                else if (e.IsText)
                {
                    _onMessage?.Invoke(System.Text.Encoding.UTF8.GetBytes(e.Data));
                }
            };

            _ws.OnError += (sender, e) =>
            {
                Console.WriteLine($"WebSocketエラー: {e.Message}");
            };

            _ws.OnClose += (sender, e) =>
            {
                Console.WriteLine($"WebSocket切断: Code={e.Code}, Reason={e.Reason}");
            };

            _ws.Connect();
        }
        catch (Exception ex)
        {
            //Console.WriteLine($"WebSocket接続失敗: {ex.Message}");
        }
    }

    /// <summary>
    /// メッセージを送信
    /// </summary>
    public void Send(string message)
    {
        if (_ws != null && _ws.IsAlive)
        {
            _ws.Send(message);
        }
        else
        {
            Console.WriteLine("WebSocketが接続されていません");
        }
    }

    /// <summary>
    /// 接続を切断
    /// </summary>
    public void Close()
    {
        _ws?.Close();
    }

    /// <summary>
    /// 接続状態を取得
    /// </summary>
    public bool IsConnected => _ws != null && _ws.IsAlive;
}
