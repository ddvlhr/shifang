using Core.Entities;
using Infrastructure.DataBase;

namespace Infrastructure.Services.System.Impl;

public class LogService : ILogService
{
    private readonly IRepository<Log> _logRepo;
    private readonly IUnitOfWork _uow;
    private readonly IRepository<User> _userRepo;

    public LogService(IRepository<Log> logRepo, IRepository<User> userRepo, IUnitOfWork uow)
    {
        _logRepo = logRepo;
        _userRepo = userRepo;
        _uow = uow;
    }

    public bool AddLog(string desc)
    {
        // var userId = int.Parse(_context.User.FindFirst(ClaimTypes.UserData).Value);
        // var user = _userRepo.Get(userId);
        var log = new Log
        {
            User = "",
            Description = desc
        };

        _logRepo.Add(log);

        return _uow.Save() > 0;
    }
}