using System.Collections.Generic;
using System;
using System.Text.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// WebSocketのイベント処理や監視をするクラス
/// </summary>
public class WebSocketEventManager
{
    public bool IsConnecting { get; private set; } = false;

    //クライアント実装
    WebSocketCli _client = new WebSocketCli();

    //イベント管理
    Queue<EventData> _sendQueue = new Queue<EventData>();
    Queue<EventData> _eventQueue = new Queue<EventData>();

    //接続したさいの識別ID
    string? _sessionId = null;

    private APIGetWSAddressImplement _addressAPI = new APIGetWSAddressImplement();

    //更新ループ用
    private CancellationTokenSource? _cancellationTokenSource;
    private Task? _updateTask;

    public async Task Setup(IEnvironment env)
    {
        if (IsConnecting) return;

        string address = await _addressAPI.Request();
        await Connect(address);

        // 更新ループを開始
        StartUpdateLoop();
    }

    private void StartUpdateLoop()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _updateTask = Task.Run(async () => await UpdateLoop(_cancellationTokenSource.Token));
    }

    private async Task UpdateLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await Update();
            await Task.Delay(16, cancellationToken); // 約60FPS
        }
    }

    async Task Update()
    {
        if (_sendQueue.Count == 0) return;

        var d = _sendQueue.Dequeue();

        WSPS_SendEvent data = new WSPS_SendEvent(_sessionId, d);
        await _client.Send(JsonSerializer.Serialize(data));
    }

    async Task Connect(string address)
    {
        await _client.Connect(address, Message);
    }

    public async Task Join(string userId, string userName)
    {
        var join = new WSPS_Join();
        join.UserId = userId;
        join.UserName = userName;
        join.SessionId = _sessionId;
        await _client.Send(JsonSerializer.Serialize(join));
    }

    public void Send(EventData data)
    {
        _sendQueue.Enqueue(data);
    }

    void Message(byte[] msg)
    {
        WebSocketPacket data = null;
        try
        {
            string json = Encoding.UTF8.GetString(msg);
            data = JsonSerializer.Deserialize<WebSocketPacket>(json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"メッセージパースエラー: {ex.Message}");
        }

        if (data == null) return;

        try
        {
            switch ((WebSocketCommand)data.Command)
            {
                case WebSocketCommand.WELCOME:
                    {
                        var welcome = JsonSerializer.Deserialize<WSPR_Welcome>(data.Data);
                        _sessionId = welcome.SessionId;
                        IsConnecting = true;
                        Console.WriteLine($"WebSocket接続完了 SessionId: {_sessionId}");
                    }
                    break;

                case WebSocketCommand.EVENT:
                    {
                        Ayashii.DoAction(data.Data);
                    }
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"メッセージ処理エラー: {ex.Message}");
        }
    }

    public async Task Stop()
    {
        _cancellationTokenSource?.Cancel();
        if (_client != null)
        {
            await _client.Close();
        }
    }
}