using System;
using System.Threading.Tasks;

public enum EnvironmentSetting
{
    Local,
    Develop,
    Production
}

/// <summary>
/// ネットワークまわりここに全部詰め込んだクラス
/// NOTE: すべてここからアクセスできます。
/// </summary>
public class NetworkFront
{
    //ゲームサーバーまわり。

    static public IEnvironment Environment => _instance._environment;

    /// <summary>WebSocketの準備ができているか</summary>
    static public bool IsSetup => _instance._wsManager?.IsConnecting ?? false;

    /// <summary>イベントを送信(イベントデータを作って送信)</summary>
    static public void Send(EventData data) { _instance.SendImplement(data); }

    /// <summary>初期化処理</summary>
    static public async Task Initialize(EnvironmentSetting env)
    {
        await _instance.Run(env);
    }

    /// <summary>停止処理</summary>
    static public void Stop()
    {
        _instance._wsManager?.Stop();
    }

    #region 内部処理用
    static NetworkFront _instance = new NetworkFront();
    NetworkFront() { }

    //セットアップ時に実態が生まれるもの
    IEnvironment _environment = new LocalEnvironment();
    WebSocketEventManager? _wsManager = null;

    //内部インタフェース

    //エントリポイント(ゲーム開始時にコールされる)
    async Task Run(EnvironmentSetting env)
    {
        //環境構成
        switch (env)
        {
            case EnvironmentSetting.Local:
                _environment = new LocalEnvironment();
                break;

            case EnvironmentSetting.Develop:
            case EnvironmentSetting.Production:
                _environment = new ProductionEnvironment();
                break;
        }

        //WebSocket管理オブジェクトの生成
        _wsManager = new WebSocketEventManager();

        //WebSocket接続開始
        await _wsManager.Setup(_environment);
    }

    /// <summary>
    /// イベントの送信
    /// NOTE:
    /// </summary>
    void SendImplement(EventData d)
    {
        _wsManager?.Send(d);
    }

    #endregion
}