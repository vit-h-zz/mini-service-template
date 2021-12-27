using MassTransit;
using MassTransit.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using TemplateService.Data.Persistence;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using NodaTime;
using Xunit.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Distributed;

namespace TemplateService.Tests.Helpers
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public CustomWebApplicationFactory(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        public Mock<IClock> ClockMock { get; } = new();
        public Mock<IDistributedCache> CacheMock { get; } = new();
        public string TestUserId { get; } = Guid.NewGuid().ToString();

        protected override IHostBuilder CreateHostBuilder() => base
            .CreateHostBuilder()
            .ConfigureAppConfiguration(cb => cb.Add(new MemoryConfigurationSource()
            {
                InitialData = new Dictionary<string, string>
                {
                    {"Database:Enable", "false"},
                    //{"Database:UseInMemory", "true"},
                    //{"Database:DetailedErrors", "true"},
                    {"Database:HealthChecks:Enable", "false"},
                    {"MassTransit:Enable", "false"},
                    {"AppHealthChecks:Enable", "false"},
                    {"Redis:Enable", "false"}
                }
            }));

        protected override void ConfigureWebHost(IWebHostBuilder builder) =>
            // Configure app after TStartup class
            // May consider to creating a custom startup to speedup tests setup
            builder.ConfigureServices(services =>
            {
                var guid = Guid.NewGuid().ToString();
                services.AddDbContext<AppDbContext>(b =>
                {
                    b.UseInMemoryDatabase("TemplateServiceTestDb_" + guid);
                    b.EnableDetailedErrors();
                    b.EnableSensitiveDataLogging();
                });

                services.AddLogging(builder => builder.ClearProviders().AddConsole().AddDebug());
                ////https://www.meziantou.net/how-to-get-asp-net-core-logs-in-the-output-of-xunit-tests.htm
                //services.AddLogging(loggingBuilder =>
                //{
                //    loggingBuilder.ClearProviders();
                //    //loggingBuilder.Services.AddSingleton<ILoggerProvider>(serviceProvider => new XUnitLoggerProvider(_testOutputHelper));
                //});

                services.Replace(ServiceDescriptor.Singleton(_ => ClockMock.Object));
                services.Replace(ServiceDescriptor.Singleton(_ => CacheMock.Object));

                // Register MassTransit consumers
                services.Scan(scan => scan
                    .FromAssemblyOf<TStartup>()
                    .AddClasses(o => o.AssignableTo<IConsumer>())
                    .AsSelf()
                    .WithScopedLifetime());

                //Inmemory transport may be used in case of TestHarness limitations
                services.AddMassTransitInMemoryTestHarness(cfg =>
                {
                    var consumerAssembly = typeof(TStartup).Assembly;
                    cfg.AddConsumers(consumerAssembly);
                    cfg.AddConsumerTestHarnesses(consumerAssembly);
                    //cfg.AddTransactionalBus();
                });

                services.AddSingleton<IBusControl>(sp => sp.GetRequiredService<BusTestHarness>().BusControl);
                services.AddSingleton<IBus>(sp => sp.GetRequiredService<BusTestHarness>().Bus);

                services.AddSingleton<IPublishEndpoint>(provider => provider.GetRequiredService<IBusControl>());
                services.AddSingleton<ISendEndpointProvider>(provider => provider.GetRequiredService<IBusControl>());
            });
    }
}
