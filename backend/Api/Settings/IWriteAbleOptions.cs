using System;
using Microsoft.Extensions.Options;

namespace Api.Settings;

public interface IWriteAbleOptions<out T> : IOptions<T> where T : class, new()
{
    void Update(Action<T> applyChanges);
}