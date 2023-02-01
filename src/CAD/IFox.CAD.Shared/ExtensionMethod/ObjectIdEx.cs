﻿namespace IFoxCAD.Cad;

/// <summary>
/// 对象id扩展类
/// </summary>
public static class ObjectIdEx
{
    #region GetObject
    /// <summary>
    /// 获取指定类型对象
    /// </summary>
    /// <typeparam name="T">指定的泛型</typeparam>
    /// <param name="id">对象id</param>
    /// <param name="openMode">打开模式</param>
    /// <param name="trans">事务</param>
    /// <param name="openErased">是否打开已删除对象,默认为不打开</param>
    /// <param name="openLockedLayer">是否打开锁定图层对象,默认为不打开</param>
    /// <returns>指定类型对象</returns>
    public static T? GetObject<T>(this ObjectId id,
                                 OpenMode openMode = OpenMode.ForRead,
                                 Transaction? trans = null,
                                 bool openErased = false,
                                 bool openLockedLayer = false) where T : DBObject
    {
        trans ??= DBTrans.Top.Transaction;
        return trans.GetObject(id, openMode, openErased, openLockedLayer) as T;
    }

    /// <summary>
    /// 获取指定类型对象集合
    /// </summary>
    /// <typeparam name="T">指定的泛型</typeparam>
    /// <param name="ids">对象id集合</param>
    /// <param name="openMode">打开模式</param>
    /// <param name="trans">事务</param>
    /// <param name="openErased">是否打开已删除对象,默认为不打开</param>
    /// <param name="openLockedLayer">是否打开锁定图层对象,默认为不打开</param>
    /// <returns>指定类型对象集合</returns>
    [System.Diagnostics.DebuggerStepThrough]
    public static IEnumerable<T?> GetObject<T>(this IEnumerable<ObjectId> ids,
                                               OpenMode openMode = OpenMode.ForRead,
                                               Transaction? trans = null,
                                               bool openErased = false,
                                               bool openLockedLayer = false) where T : DBObject
    {
        trans ??= DBTrans.Top.Transaction;
        return ids.Select(id => id.GetObject<T>(openMode, trans, openErased, openLockedLayer));
    }

    /// <summary>
    /// 返回符合类型的对象id
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    /// <param name="ids">对象id集合</param>
    /// <returns>对象id集合</returns>
    public static IEnumerable<ObjectId> OfType<T>(this IEnumerable<ObjectId> ids) where T : DBObject
    {
        string dxfName = RXClass.GetClass(typeof(T)).DxfName;
        return ids.Where(id => id.ObjectClass().DxfName == dxfName);
    }
    #endregion GetObject

    public static RXClass ObjectClass(this ObjectId id)
    {
#if NET35
        return RXClass.GetClass(id.GetType());
#else
        return id.ObjectClass;
#endif
    }

    /// <summary>
    /// id是否有效,未被删除
    /// </summary>
    /// <param name="id">对象id</param>
    /// <returns>id有效返回 <see langword="true"/>，反之返回 <see langword="false"/></returns>
    public static bool IsOk(this ObjectId id)
    {
        return !id.IsNull && id.IsValid && !id.IsErased && !id.IsEffectivelyErased && id.IsResident;
    }

    /// <summary>
    /// 删除id代表的对象
    /// </summary>
    /// <param name="id">对象id</param>
    public static void Erase(this ObjectId id)
    {
        if (id.IsOk())
        {
            var ent = id.GetObject<DBObject>()!;
            using (ent.ForWrite())
            {
                ent.Erase();
            }// 第一种读写权限自动转换写法
            // Env.Editor.Regen();
        }
    }
}