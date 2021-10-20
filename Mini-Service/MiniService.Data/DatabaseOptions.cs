using System;
using Common.Options;

namespace MiniService.Data
{
    public class DatabaseOptions
    {
        internal const string SectionName = "Database";

        public bool Enable { get; set; }
        public bool UseInMemory { get; set; } = true;
        public bool DetailedErrors { get; set; }
        public string ConnectionString { get; set; } = null!;
        public HealthChecksOptions HealthChecks { get; set; } = null!;
        public Version Version { get; set; } = null!;
    }
}
