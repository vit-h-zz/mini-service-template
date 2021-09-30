using AutoFixture;
using AutoFixture.AutoMoq;
using AutoFixture.Xunit2;
using Common.Tests;

namespace MiniService.Tests.Helpers
{
    public sealed class AutoMoqDataAttribute : AutoDataAttribute
    {
        public AutoMoqDataAttribute() : base(() =>
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization())
                .RegisterFakers<AutoMoqDataAttribute>();

            // Register Type Relays
            //foreach (var typeRelay in Initializer.TypeRelays)
            //{
            //    fixture.Customizations.Add(typeRelay);
            //}

            return fixture;
        })
        { }
    }
}
