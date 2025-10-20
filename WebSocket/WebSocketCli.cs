using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// WebSocketのクライアント実装
/// </summary>
public class WebSocketCli
{
    private ClientWebSocket? _ws;
    private Action<byte[]>? _onMessage;
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _receiveTask;

    /// <summary>
    /// WebSocketサーバーに接続
    /// </summary>
    public async Task Connect(string address, Action<byte[]> onMessage)
    {
        _onMessage = onMessage;

        try
        {
            _ws = new ClientWebSocket();
            _cancellationTokenSource = new CancellationTokenSource();

            Console.WriteLine($"WebSocket接続開始: {address}");
            await _ws.ConnectAsync(new Uri(address), _cancellationTokenSource.Token);
            Console.WriteLine($"WebSocket接続成功: {address}");

            // 受信ループを開始
            _receiveTask = Task.Run(() => ReceiveLoop(_cancellationTokenSource.Token));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebSocket接続失敗: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// メッセージ受信ループ
    /// </summary>
    private async Task ReceiveLoop(CancellationToken cancellationToken)
    {
        var buffer = new byte[1024 * 4];

        try
        {
            while (_ws.State == WebSocketState.Open && !cancellationToken.IsCancellationRequested)
            {
                var result = await _ws.ReceiveAsync(new ArraySegment<byte>(buffer), cancellationToken);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    Console.WriteLine($"WebSocket切断: {result.CloseStatus}");
                    await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                    break;
                }
                else
                {
                    // メッセージを受信
                    var receivedData = new byte[result.Count];
                    Array.Copy(buffer, receivedData, result.Count);
                    _onMessage?.Invoke(receivedData);
                }
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("WebSocket受信ループがキャンセルされました");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebSocketエラー: {ex.Message}");
        }
    }

    /// <summary>
    /// メッセージを送信
    /// </summary>
    public async Task Send(string message)
    {
        if (_ws != null && _ws.State == WebSocketState.Open)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            await _ws.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true, CancellationToken.None);
        }
        else
        {
            Console.WriteLine("WebSocketが接続されていません");
        }
    }

    /// <summary>
    /// 接続を切断
    /// </summary>
    public async Task Close()
    {
        if (_ws != null && _ws.State == WebSocketState.Open)
        {
            _cancellationTokenSource?.Cancel();
            await _ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "クライアントが切断", CancellationToken.None);
        }
        _ws?.Dispose();
        _cancellationTokenSource?.Dispose();
    }

    /// <summary>
    /// 接続状態を取得
    /// </summary>
    public bool IsConnected => _ws != null && _ws.State == WebSocketState.Open;
}
