using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using VH.MiniService.Common.Tests;
using TemplateService.Data.Persistence;
using Microsoft.EntityFrameworkCore;

namespace TemplateService.Tests.Helpers
{
    public sealed class AutoMoqWithInMemoryDbAttribute : AutoDataAttribute
    {
        public AutoMoqWithInMemoryDbAttribute() : base(() =>
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .RegisterFakers<AutoMoqWithInMemoryDbAttribute>();

            fixture.Inject(new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
                    .UseInMemoryDatabase("TemplateServiceDbContext").Options));

            return fixture;
        })
        { }
    }
}
