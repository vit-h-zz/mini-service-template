using MassTransit.Registration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace TemplateService.Tests.Helpers
{
    public class MassTransitPassthroughInstanceProvider : IConfigurationServiceProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public MassTransitPassthroughInstanceProvider(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;
        public object? GetService(Type serviceType) => _serviceProvider.GetService(serviceType);
        public T GetRequiredService<T>() where T : class => _serviceProvider.GetRequiredService<T>();
        public T GetService<T>() where T : class => _serviceProvider.GetService<T>()!;
    }
}
