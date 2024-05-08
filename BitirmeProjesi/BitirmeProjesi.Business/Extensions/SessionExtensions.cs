using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitirmeProjesi.Business.Extensions;

public static class SessionExtensions
{
    public static void Set<T>(this ISession session, string key, T value)
    {
        var json = JsonConvert.SerializeObject(value); // Verilen 'value' nesnesini JSON formatına dönüştürür
        session.SetString(key, json); // JSON verisini 'key' anahtarı ile session'da (oturumda) saklar.
    }

    public static T Get<T>(this ISession session, string key)
    {
        var value = session.GetString(key); // Session'dan belirtilen 'key' anahtarı ile kaydedilmiş string'i alır.
        return value == null ? default(T) : JsonConvert.DeserializeObject<T>(value); // String boş ise varsayılan değeri döndürür, değilse JSON'u nesneye dönüştürür.
    }
}