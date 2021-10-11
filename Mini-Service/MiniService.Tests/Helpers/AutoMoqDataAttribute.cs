using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using Common.Tests;
using FluentAssertions.Common;

namespace MiniService.Tests.Helpers
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
