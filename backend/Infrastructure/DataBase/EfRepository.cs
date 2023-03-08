using System;
using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataBase;

public class EfRepository<T> : IRepository<T> where T : Entity
{
    private readonly ApplicationDbContext _context;
    private readonly DbSet<T> _entities;

    public EfRepository(ApplicationDbContext context)
    {
        _context = context;
        _entities = context.Set<T>();
    }

    public void Add(T item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));

        item.CreatedAtUtc = DateTime.Now;
        item.ModifiedAtUtc = DateTime.Now;

        _entities.Add(item);
    }

    public IQueryable<T> All()
    {
        return _entities;
    }

    public T Get(int id)
    {
        return _entities.Find(id);
    }

    public void Delete(T item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));

        _entities.Remove(item);
    }

    public void Update(T item)
    {
        if (item == null) throw new ArgumentNullException(nameof(item));

        item.ModifiedAtUtc = DateTime.Now;
    }

    public void AddRange(IEnumerable<T> items)
    {
        foreach (var item in items) Add(item);
    }

    public void UpdateRange(IEnumerable<T> items)
    {
        foreach (var item in items) Update(item);
    }

    public void DeleteRange(IEnumerable<T> items)
    {
        foreach (var item in items) Delete(item);
    }
}