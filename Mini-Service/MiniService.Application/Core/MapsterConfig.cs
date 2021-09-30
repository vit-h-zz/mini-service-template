using Mapster;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MiniService.Application.Core
{
    public static class MapsterConfig
    {
        [ModuleInitializer]
        public static void Setup()
        {
            TypeAdapterConfig.GlobalSettings.Default.MapToConstructor(true);
            TypeAdapterConfig.GlobalSettings.Default.RequireDestinationMemberSource(true);
            TypeAdapterConfig.GlobalSettings.Default.NameMatchingStrategy(NameMatchingStrategy.IgnoreCase);
            TypeAdapterConfig.GlobalSettings.RequireDestinationMemberSource = true;
            TypeAdapterConfig.GlobalSettings.Scan(Assembly.GetExecutingAssembly());
            TypeAdapterConfig.GlobalSettings.Compile();
        }
    }
}
