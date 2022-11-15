﻿using System;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Extensions;

public static class LazyResolutionExtensions
{
    public static IServiceCollection AddLazyResolution(this IServiceCollection services)
    {
        return services.AddTransient(typeof(Lazy<>), typeof(LazilyResolved<>));
    }

    private class LazilyResolved<T> : Lazy<T>
    {
        public LazilyResolved(IServiceProvider serviceProvider) : base(serviceProvider.GetRequiredService<T>())
        {
        }
    }
}