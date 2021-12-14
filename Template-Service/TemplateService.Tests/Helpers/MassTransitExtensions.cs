using VH.MiniService.Common.Service.MassTransit;
using MassTransit;
using MassTransit.ExtensionsDependencyInjectionIntegration;
using MassTransit.Testing;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace TemplateService.Tests.Helpers
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
                var provider = new MassTransitPassthroughInstanceProvider(serviceProvider);
                o.UseConsumeFilter(typeof(ConsumeFilterMock<>), provider);
                o.UseConsumeFilter(typeof(ConsumeRequestContextExtractorFilter<>), provider);
                //o.UsePublishFilter(typeof(PublishRequestContextSetterFilter<>), provider);
            };
        }

        public static void AddDefaultHeaders(this InMemoryTestHarness harness, params (string, object)[] headers)
        {
            harness.BusControl.ConnectPublishObserver(new PutHeaderDataPublishObserver(headers));
        }
    }

    public class PutHeaderDataPublishObserver : IPublishObserver
    {
        private readonly (string, object)[] _headers;

        public PutHeaderDataPublishObserver(params (string, object)[] headers)
        {
            _headers = headers;
        }

        public Task PrePublish<T>(PublishContext<T> context) where T : class
        {
            foreach (var (key, value) in _headers)
            {
                context.Headers.Set(key, value);
            }
            return Task.FromResult(true);
        }

        public Task PostPublish<T>(PublishContext<T> context) where T : class
            => Task.FromResult(true);

        public Task PublishFault<T>(PublishContext<T> context, Exception exception) where T : class
            => Task.FromResult(true);
    }
}
