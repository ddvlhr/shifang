using SqlSugar;

namespace Infrastructure.DataBase;

public class Repository<T>: SimpleClient<T> where T: class, new()
{
    public Repository(ISqlSugarClient context = null): base(context)
    {
        base.Context = context;
    }
}