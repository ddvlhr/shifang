using System.Linq;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.DataBase;

public class ApplicationDbContext : DbContext
{
    public readonly ILoggerFactory MyLoggerFactory = LoggerFactory.Create(
        c => { c.AddConsole(); });

    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
        optionsBuilder.UseLoggerFactory(MyLoggerFactory);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var entityType = typeof(Entity);
        entityType
            .Assembly
            .GetExportedTypes()
            .Where(type => !type.IsAbstract && !type.IsInterface && entityType.IsAssignableFrom(type))
            .ToList()
            .ForEach(type => modelBuilder.Entity(type));
    }
}