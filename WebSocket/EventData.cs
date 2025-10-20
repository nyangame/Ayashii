using System;
using System.Diagnostics;

/// <summary>
/// イベントデータクラス
/// NOTE: データはすべてこのクラスを派生してやり取りされます
/// </summary>
[Serializable]
public class EventData
{
    /// <summary>
    /// データを格納します
    /// NOTE: ToString出来ない場合は正常に動作しない可能性があります。
    /// </summary>
    public void DataPack<T>(string Key, T data)
    {
        Payload.Add(new ParamData()
        {
            Key = Key,
            TypeName = typeof(T).Name,
            Data = data.ToString()
        });
    }

    /// <summary>
    /// データをJsonでシリアライズして格納します
    /// NOTE: シリアライズ出来ない場合は正常に動作しない可能性があります。
    /// </summary>
    public void DataJsonSerializePack<T>(string Key, T data)
    {
        //シリアライズ可能かどうかチェック
        if (!typeof(T).IsSerializable)
        {
            Console.Write("シリアライズ可能な値ではありませんでした。代わりにDataPackを試みます");
            DataPack(Key, data);
            return;
        }

        Payload.Add(new ParamData()
        {
            Key = Key,
            TypeName = typeof(T).Name,
            Data = data.ToString()
        });
    }

    /// <summary>
    /// データを返します
    /// NOTE: もし構造体などを展開する場合は、TypeNameからデータ内容を推察してください。
    /// </summary>
    public ParamData GetData(string Key)
    {
        var target = Payload.Where(p => p.Key == Key);
        if (target.Count() == 0)
        {
            Console.Write($"データがNULLです");
            return null;
        }
        return target.First();
    }

    /// <summary>
    /// 数字データを返します
    /// NOTE: 数字じゃないデータは0またはNaNが帰ります
    /// </summary>
    public int GetIntData(string Key)
    {
        var data = GetData(Key);
        if (data == null) return 0;
        if (data.TypeName != "Integer" && !data.TypeName.StartsWith("Int"))
        {
            Console.Write($"Intじゃない値かもしれません:{data.Data}({data.TypeName})");
        }
        return int.Parse(data.Data);
    }

    /// <summary>
    /// 数字データを返します
    /// NOTE: 数字じゃないデータは0またはNaNが帰ります
    /// </summary>
    public float GetFloatData(string Key)
    {
        var data = GetData(Key);
        if (data == null) return 0;
        if (data.TypeName != "Float" && data.TypeName != "Single")
        {
            Console.Write($"Floatじゃない値かもしれません:{data.Data}({data.TypeName})");
        }
        return float.Parse(data.Data);
    }

    /// <summary>
    /// 文字列データを返します
    /// </summary>
    public string GetStringData(string Key)
    {
        var data = GetData(Key);
        if (data == null) return "";
        return data.Data;
    }


    /// <summary>
    /// データパック
    /// </summary>
    [Serializable]
    public class ParamData
    {
        public string Key;
        public string TypeName;
        public string Data;
    }

    #region 内部実装
    //シリアライズされるメンバ
    public int EventId = -1;
    protected List<ParamData> Payload = new List<ParamData>();
    //ここまで

    //主にコピーやイベントの変換に使う
    protected EventData(EventData d)
    {
        EventId = d.EventId;
        Payload = d.Payload;
    }

    //新しいイベントの作成に使う
    public EventData(int eventId)
    {
        EventId = eventId;
    }
    #endregion
}