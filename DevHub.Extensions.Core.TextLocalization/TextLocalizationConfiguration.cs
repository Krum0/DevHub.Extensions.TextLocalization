namespace DevHub.Extensions.Core.TextLocalization
{
    using System.IO;

    public class TextLocalizationConfiguration
    {
        public string DefaultPath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Language");
    }
}
