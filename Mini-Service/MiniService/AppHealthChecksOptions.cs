namespace MiniService
{
    public class AppHealthChecksOptions
    {
        public const string SectionName = "AppHealthChecks";

        public bool Enable { get; set; }
        public bool EnableUi { get; set; }
    }
}
