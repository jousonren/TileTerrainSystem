using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 对Dictory的扩展 
/// </summary>
public static class DictionaryExtension
{
    /// <summary>
    /// 根据key得到value
    /// </summary>
    /// <typeparam name="Tkey"></typeparam>
    /// <typeparam name="Tvalue"></typeparam>
    /// <param name="dict"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    public static Tvalue TryGet<Tkey,Tvalue>(this Dictionary<Tkey,Tvalue>dict,Tkey key)
    {
        Tvalue value;
        dict.TryGetValue(key,out value);
        return value;
    }
}

	
