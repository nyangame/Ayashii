using System;

enum WebSocketCommand
{
    WELCOME = 1,
    JOIN = 2,
    EVENT = 3,
    SEND_JOIN = 100,
    SEND_EVENT = 101
};

[Serializable]
public class WebSocketPacket
{
    public int UserId { get; set; }
    public int Target { get; set; }
    public int Command { get; set; }
    public string Data { get; set; }
};

[Serializable]
public class WSPR_Welcome
{
    public string SessionId { get; set; }
};

[Serializable]
public class WSPR_Join
{
    public string UserId { get; set; }
    public string UserName { get; set; }
};

[Serializable]
public class WSPS_SendEvent : EventData
{
    public string SessionId { get; set; }
    public int Command = (int)WebSocketCommand.SEND_EVENT;

    public WSPS_SendEvent(string sessionId, EventData d) : base(d)
    {
        SessionId = sessionId;
    }
};

[Serializable]
public class WSPS_Join
{
    public string UserId { get; set; }
    public string UserName { get; set; }
    public string SessionId { get; set; }
    public int Command = (int)WebSocketCommand.SEND_JOIN;
};
