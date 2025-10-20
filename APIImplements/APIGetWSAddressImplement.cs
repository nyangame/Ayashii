using System;
using System.Text.Json;

//このAPIはリクエストパラメータはなし

/// <summary>
/// 戻り値
/// </summary>
public class GetAddrResult
{
    public int Status { get; set; }
    public string Address { get; set; }
}

/// <summary>
/// WebSocketアドレス取得
/// </summary>
public class APIGetWSAddressImplement
{
    async public Task<string> Request()
    {
        string request = String.Format("{0}/getaddr", NetworkFront.Environment.APIServerURI);
        string json = await Network.WebRequest.GetRequest(request);
        var ret = JsonSerializer.Deserialize<GetAddrResult>(json);
        return ret?.Address;
    }
}
