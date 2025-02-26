using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace FonTech.Domain.Extensions;

// для многих сервисов
// расширяет класс IDistributedCache
public static class DistributedCacheExtensions
{
    /// <summary>
    /// Передаем ключ во второй параметр по этому ключу получаем значение из массива байтов
    ///
    /// А если массив байтов не пустой то десериализуем
    /// </summary>
    /// <param name="cache"></param>
    /// <param name="key"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T? GetObject<T>(this IDistributedCache cache, string key)
    {
        var data = cache.Get(key);
        return data?.Length > 0 ? JsonSerializer.Deserialize<T>(data) : default(T);
    }
    
    
    /// <summary>
    /// Key - первый параметр и по опциональности передаём некоторые настройки
    ///
    /// можем сделать сразу
    ///
    /// GetObject достаём данные и далее десериализуем
    ///
    /// 
    /// </summary>
    /// <param name="cache"></param>
    /// <param name="key"></param>
    /// <param name="obj"></param>
    /// <param name="ops"></param>
    /// <typeparam name="T"></typeparam>
    public static void SetObject<T>(this IDistributedCache cache, string key, T obj, DistributedCacheEntryOptions? ops = null)
    {
        var data = JsonSerializer.SerializeToUtf8Bytes(obj);
        if (data?.Length > 0)
        {
            // добавляем объект сериализованный в массив байтов
            cache.Set(key, data, ops ?? new DistributedCacheEntryOptions());
        }
    }
    
    public static void SetObjects<T>(this IDistributedCache cache, string key, List<T> objs, DistributedCacheEntryOptions? ops = null)
    {
        for (var index = 0; index < objs.Count; index++)
        {
            var obj = objs[index];
            var data = JsonSerializer.SerializeToUtf8Bytes(obj);
            if (data?.Length > 0)
            {
                var resultKey = $"{key}_{index}";
                cache.Set(resultKey, data, ops ?? new DistributedCacheEntryOptions());
            }
        }
    }
    
    /// <summary>
    /// Обновляет время жизни самого объекта а не данные
    /// </summary>
    /// <param name="cache"></param>
    /// <param name="key"></param>
    /// <param name="ops"></param>
    /// <typeparam name="T"></typeparam>
    public static void RefreshObject<T>(this IDistributedCache cache, string key, DistributedCacheEntryOptions? ops = null)
    {
        var isObj = cache.GetObject<T>(key);
        if (isObj == null)
            return;
        
        cache.Refresh(key);
    }
}