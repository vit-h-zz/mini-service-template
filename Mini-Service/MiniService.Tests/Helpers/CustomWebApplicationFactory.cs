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
using MiniService.Data.Persistence;
using Common.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using NodaTime;

namespace MiniService.Tests.Helpers
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        public Mock<IClock> ClockMock { get; } = new();
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
                    {"AppHealthChecks:Enable", "false"}
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
                    b.UseInMemoryDatabase("MiniServiceTestDb_" + guid);
                    b.EnableDetailedErrors();
                    b.EnableSensitiveDataLogging();
                });

                services.Replace(ServiceDescriptor.Scoped<IUserContext>(_ => new TestUserContext(TestUserId)));
                services.Replace(ServiceDescriptor.Singleton(_ => ClockMock.Object));

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

                //var sp = services.BuildServiceProvider(true);
            });
    }
}
