using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.DataBase;

public interface IRepository<T>
{
    void Add(T item);
    IQueryable<T> All();
    T Get(int id);
    void Delete(T item);
    void Update(T item);
    void AddRange(IEnumerable<T> items);
    void UpdateRange(IEnumerable<T> items);
    void DeleteRange(IEnumerable<T> items);
}