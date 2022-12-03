using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql.Replication;

namespace Infrastructure.DataBase;

public interface IBaseRepository<T> where T : class, new()
{
    #region Add
    /// <summary>
    /// 新增单条数据
    /// </summary>
    /// <param name="model">实体对象</param>
    /// <returns>操作是否成功</returns>
    public Task<bool> Add(T model);
    
    /// <summary>
    /// 新增多条数据
    /// </summary>
    /// <param name="list">实体集合</param>
    /// <returns>操作是否成功</returns>
    public Task<bool> AddRange(List<T> list);
    
    /// <summary>
    /// 新增单条数据并返回自增列
    /// </summary>
    /// <param name="model">实体对象</param>
    /// <returns></returns>
    public Task<bool> AddReturnIdentity(T model);
    
    /// <summary>
    /// 新增单条数据并返回实体
    /// </summary>
    /// <param name="model">实体对象</param>
    /// <returns></returns>
    public Task<T> AddReturnEntity(T model);

    /// <summary>
    /// 只新增指定列
    /// </summary>
    /// <param name="model">实体对象</param>
    /// <param name="columns">指定要新增的列</param>
    /// <returns></returns>
    public Task<bool> AddColumns(T model, params string[] columns);
    
    /// <summary>
    /// 不插入指定列
    /// </summary>
    /// <param name="model">实体对象</param>
    /// <param name="ignoreColumns">要忽略的列</param>
    /// <returns></returns>
    public Task<bool> AddColumnsByIgnoreColumns(T model, params string[] ignoreColumns);
    
    #endregion

    #region Delete

    /// <summary>
    /// 根据主键删除, 并返回操作是否成功
    /// </summary>
    /// <param name="key">主键</param>
    /// <typeparam name="S">主键的类型</typeparam>
    /// <returns></returns>
    public Task<bool> Delete<S>(S key);

    /// <summary>
    /// 根据主键删除, 并返回操作是否成功
    /// </summary>
    /// <param name="keys">主键</param>
    /// <typeparam name="S">主键类型</typeparam>
    /// <returns></returns>
    public Task<bool> DeleteRange<S>(params S[] keys);

    #endregion
}