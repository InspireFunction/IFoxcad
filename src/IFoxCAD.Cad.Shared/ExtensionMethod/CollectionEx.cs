﻿using System.ComponentModel;
using System.Xml.Linq;

namespace IFoxCAD.Cad;

/// <summary>
/// 集合扩展类
/// </summary>
public static class CollectionEx
{
    /// <summary>
    /// 对象id迭代器转换为集合
    /// </summary>
    /// <param name="ids">对象id的迭代器</param>
    /// <returns>对象id集合</returns>
    public static ObjectIdCollection ToCollection(this IEnumerable<ObjectId> ids)
    {
        return new ObjectIdCollection(ids.ToArray());
    }

    /// <summary>
    /// 实体迭代器转换为集合
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="objs">实体对象的迭代器</param>
    /// <returns>实体集合</returns>
    public static DBObjectCollection ToCollection<T>(this IEnumerable<T> objs) where T : DBObject
    {
        DBObjectCollection objCol = new();
        foreach (T obj in objs)
            objCol.Add(obj);
        return objCol;
    }

    /// <summary>
    /// double 数值迭代器转换为 double 数值集合
    /// </summary>
    /// <param name="doubles">double 数值迭代器</param>
    /// <returns>double 数值集合</returns>
    public static DoubleCollection ToCollection(this IEnumerable<double> doubles)
    {
        return new DoubleCollection(doubles.ToArray());
    }

    /// <summary>
    /// 二维点迭代器转换为二维点集合
    /// </summary>
    /// <param name="pts">二维点迭代器</param>
    /// <returns>二维点集合</returns>
    public static Point2dCollection ToCollection(this IEnumerable<Point2d> pts)
    {
        return new Point2dCollection(pts.ToArray());
    }

    /// <summary>
    /// 三维点迭代器转换为三维点集合
    /// </summary>
    /// <param name="pts">三维点迭代器</param>
    /// <returns>三维点集合</returns>
    public static Point3dCollection ToCollection(this IEnumerable<Point3d> pts)
    {
        return new Point3dCollection(pts.ToArray());
    }

    /// <summary>
    /// 对象id集合转换为对象id列表
    /// </summary>
    /// <param name="ids">对象id集合</param>
    /// <returns>对象id列表</returns>
    public static List<ObjectId> ToList(this ObjectIdCollection ids)
    {
        return ids.Cast<ObjectId>().ToList();
    }


    /// <summary>
    /// 遍历集合,执行委托
    /// </summary>
    /// <typeparam name="T">集合值的类型</typeparam>
    /// <param name="source">集合</param>
    /// <param name="action">委托</param>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        foreach (var element in source)
            action.Invoke(element);
    }
    /// <summary>
    /// 遍历集合,执行委托<br/>
    /// 输出索引值
    /// </summary>
    /// <typeparam name="T">集合值的类型</typeparam>
    /// <param name="source">集合</param>
    /// <param name="action">委托</param>
    public static void ForEach<T>(this IEnumerable<T> source, Action<int, T> action)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        int i = 0;
        foreach (var element in source)
        {
            action.Invoke(i, element);
            i++;
        }
    }
    /// <summary>
    /// 遍历集合,执行委托<br/>
    /// 输出索引值,允许循环中断
    /// </summary>
    /// <typeparam name="T">集合值的类型</typeparam>
    /// <param name="source">集合</param>
    /// <param name="action">委托</param>
    public static void ForEach<T>(this IEnumerable<T> source, Action<int, T, LoopState> action)
    {
        if (action == null)
            throw new ArgumentNullException(nameof(action));

        LoopState state = new();/*这种方式比Action改Func更友好*/
        int i = 0;
        foreach (var element in source)
        {
            action.Invoke(i, element, state);
            if (!state.IsRun)
                break;
            i++;
        }
    }


    #region 关键字集合
    public enum KeywordName
    {
        GlobalName,
        LocalName,
        DisplayName,
    }

    /// <summary>
    /// 含有关键字
    /// </summary>
    /// <param name="collection">关键字集合</param>
    /// <param name="name">关键字</param>
    /// <param name="keywordName">关键字容器字段名</param>
    /// <returns>true含有</returns>
    public static bool Contains(this KeywordCollection collection, string name,
                                KeywordName keywordName = KeywordName.GlobalName)
    {
        bool contains = false;
        switch (keywordName)
        {
            case KeywordName.GlobalName:
            for (int i = 0; i < collection.Count; i++)
            {
#if gcad
                var item = collection.get_Item(i);
#else
                var item = collection[i];
#endif
                if (item.GlobalName == name)
                {
                    contains = true;
                    break;
                }
            }
            break;
            case KeywordName.LocalName:
            for (int i = 0; i < collection.Count; i++)
            {
#if gcad
                var item = collection.get_Item(i);
#else
                var item = collection[i];
#endif
                if (item.LocalName == name)
                {
                    contains = true;
                    break;
                }
            }
            break;
            case KeywordName.DisplayName:
            for (int i = 0; i < collection.Count; i++)
            {
#if gcad
                var item = collection.get_Item(i);
#else
                var item = collection[i];
#endif
                if (item.DisplayName == name)
                {
                    contains = true;
                    break;
                }
            }
            break;
            default:
            break;
        }
        return contains;
    }

    /// <summary>
    /// 获取词典<see langword="(GlobalName"/>,<see langword="DisplayName)"/>
    /// <para>KeywordCollection是允许重复关键字的,没有哈希索引,在多次判断时候会遍历多次O(n),所以生成一个词典进行O(1)</para>
    /// </summary>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static Dictionary<string, string> GetDict(this KeywordCollection collection)
    {
        Dictionary<string, string> map = new();
        for (int i = 0; i < collection.Count; i++)
        {
#if gcad
            var item = collection.get_Item(i);
#else
            var item = collection[i];
#endif
            map.Add(item.GlobalName, item.DisplayName);
        }
        return map;
    }
    #endregion


    #region IdMapping
    /// <summary>
    /// 旧块名
    /// </summary>
    /// <param name="idmap"></param>
    /// <returns></returns>
    public static List<ObjectId> GetKeys(this IdMapping idmap)
    {
        List<ObjectId> ids = new();
        foreach (IdPair item in idmap)
            ids.Add(item.Key);
        return ids;
    }

    /// <summary>
    /// 新块名
    /// </summary>
    /// <param name="idmap"></param>
    /// <returns></returns>
    public static List<ObjectId> GetValues(this IdMapping idmap)
    {
        List<ObjectId> ids = new();
        foreach (IdPair item in idmap)
            ids.Add(item.Value);
        return ids;
    }

    /// <summary>
    /// 转换为词典
    /// </summary>
    /// <param name="mapping"></param>
    /// <returns></returns>
    public static Dictionary<ObjectId, ObjectId> ToDictionary(this IdMapping mapping)
    {
        var keyValuePairs = new Dictionary<ObjectId, ObjectId>();
        foreach (IdPair item in mapping)
            keyValuePairs.Add(item.Key, item.Value);
        return keyValuePairs;
    }
    #endregion
}