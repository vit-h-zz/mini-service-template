using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using Common.Tests;
using MiniService.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MiniService.Tests.Helpers
{
    public sealed class AutoMoqWithInMemoryDbAttribute : AutoDataAttribute
    {
        public AutoMoqWithInMemoryDbAttribute() : base(() =>
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .RegisterFakers<AutoMoqWithInMemoryDbAttribute>();

            fixture.Inject(new AppContext(new DbContextOptionsBuilder<AppContext>()
                    .UseInMemoryDatabase("MiniServiceDbContext").Options));

            return fixture;
        })
        { }
    }
}
