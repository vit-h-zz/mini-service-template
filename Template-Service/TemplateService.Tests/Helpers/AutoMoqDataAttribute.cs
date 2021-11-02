using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using VH.MiniService.Common.Tests;

namespace TemplateService.Tests.Helpers
{
    public sealed class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute() : base(() =>
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .RegisterFakers<AutoMoqDataAttribute>();

            return fixture;
        })
        { }
    }
}
