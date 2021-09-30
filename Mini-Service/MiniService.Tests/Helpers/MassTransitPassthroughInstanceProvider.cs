using MassTransit.Registration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace MiniService.Tests.Helpers
{
    public class MassTransitPassthroughInstanceProvider : IConfigurationServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public MassTransitPassthroughInstanceProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object? GetService(Type serviceType)
        {
            return _serviceProvider.GetService(serviceType);
        }

        public T GetRequiredService<T>() where T : class
        {
            return _serviceProvider.GetRequiredService<T>();
        }

        public T GetService<T>() where T : class
        {
            return _serviceProvider.GetService<T>()!;
        }
    }
}
