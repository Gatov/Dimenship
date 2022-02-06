using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using DimenshipBase;

namespace DimenshipBaseTests;

public class Utils
{
    public static string AsJSON<T>(T o)
    {
        DataContractJsonSerializer ser = new DataContractJsonSerializer(o.GetType());
        using (MemoryStream ms = new MemoryStream())
        {
            ser.WriteObject(ms, o);
            byte[] json = ms.ToArray();
            ms.Close();
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }
    }
}