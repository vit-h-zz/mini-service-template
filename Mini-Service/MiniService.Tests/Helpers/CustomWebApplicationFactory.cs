﻿using MassTransit;
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
using System.Linq;
using Moq;
using NodaTime;

namespace MiniService.Tests.Helpers
{
    public class CustomWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
    {
        public Mock<IClock> ClockMock { get; private set; } = null!;

        protected override IHostBuilder CreateHostBuilder() => base
            .CreateHostBuilder()
            .ConfigureAppConfiguration(cb => cb.Add(new MemoryConfigurationSource()
            {
                InitialData = new Dictionary<string, string>
                {
                    { "EF:Disable", "true" },
                    { "MassTransit:Enable", "false" },
                    { "HealthCheckz:Disable", "true" }
                }
            })
            );

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            //configure app after TStartup class
            builder.ConfigureServices(services =>
            {
                //If full startup is too slow. We may need to create a custom startup that is similar/derived from full app startup

                var guid = Guid.NewGuid().ToString();
                services.AddDbContext<Data.Persistence.AppContext>(x => { x.UseInMemoryDatabase(guid); });

                services.Remove(services.Single(d => d.ServiceType == typeof(IClock)));
                ClockMock = new Mock<IClock>();
                services.AddSingleton(ClockMock.Object);

                // Register MassTransit consumers
                services.Scan(scan => scan
                    .FromAssemblyOf<TStartup>()
                    .AddClasses(o => o.AssignableTo<IConsumer>())
                    .AsSelf()
                    .WithScopedLifetime());

                services.AddMassTransitInMemoryTestHarness(cfg =>
                {
                    var consumerAssembly = typeof(TStartup).Assembly;
                    cfg.AddConsumers(consumerAssembly);
                    cfg.AddConsumerTestHarnesses(consumerAssembly);
                });

                services.AddSingleton<IBusControl>(sp => sp.GetRequiredService<BusTestHarness>().BusControl);
                services.AddSingleton<IBus>(sp => sp.GetRequiredService<BusTestHarness>().Bus);

                services.AddSingleton<IPublishEndpoint>(provider => provider.GetRequiredService<IBusControl>());
                services.AddSingleton<ISendEndpointProvider>(provider => provider.GetRequiredService<IBusControl>());

                //var sp = services.BuildServiceProvider(true);

                //if there are limitations to testharness, it is possible to use inmemory transport, see masstransit's own test suite
            });
        }
    }

    public static class SetupExtensions
    {
        public static void SetupMassTransitTestHarnessMiddleware(this InMemoryTestHarness inMemoryTestHarness, IServiceProvider serviceProvider)
        {
            // Error middleware can't be added typically thru DI setup with test harness
            var instanceProvider = new MassTransitPassthroughInstanceProvider(serviceProvider);
            inMemoryTestHarness.OnConfigureInMemoryBus += o =>
            {
                o.UseConsumeFilter(typeof(ConsumeFilterMock<>), instanceProvider);
            };
        }
    }
}
