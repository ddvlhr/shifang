using System.Threading.Tasks;

namespace Infrastructure.DataBase;

public interface IUnitOfWork
{
    int Save();
    Task<int> SaveAsync();
}