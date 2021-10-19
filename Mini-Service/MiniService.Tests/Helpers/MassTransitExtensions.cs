using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.Testing;
using System;
using System.Linq;
using System.Reflection;

namespace MiniService.Tests.Helpers
{
    public static class MassTransitExtensions
    {
        public static void AddConsumerTestHarnesses(this IServiceCollectionBusConfigurator cfg, Assembly consumerAssembly)
        {
            var genericMethod = typeof(DependencyInjectionTestingExtensions).GetMethod(
                                    name: nameof(DependencyInjectionTestingExtensions.AddConsumerTestHarness),
                                    bindingAttr: BindingFlags.Public | BindingFlags.Static)
                                ?? throw new Exception($"Couldn't find consumer test harness method.");

            var consumers = consumerAssembly
                .GetTypes()
                .Where(o => o.IsAssignableTo(typeof(IConsumer)) && !o.IsInterface && !o.IsAbstract);

            foreach (var consumerType in consumers)
            {
                var method = genericMethod.MakeGenericMethod(consumerType);
                method.Invoke(obj: null, parameters: new object?[] { cfg });
            }
        }

        public static void AddMassTransitTestMiddleware(this InMemoryTestHarness inMemoryTestHarness, IServiceProvider serviceProvider)
        {
            inMemoryTestHarness.OnConfigureInMemoryBus += o =>
            {
                o.UseConsumeFilter(typeof(ConsumeFilterMock<>), new MassTransitPassthroughInstanceProvider(serviceProvider));
            };
        }
    }
}
