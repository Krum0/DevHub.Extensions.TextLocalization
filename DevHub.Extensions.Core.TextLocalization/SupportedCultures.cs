namespace DevHub.Extensions.Core.TextLocalization
{
    using System.Globalization;

    public class SupportedCultures
    {
        /// <summary>
        /// List of default Cultures
        /// </summary>
        public static CultureInfo[] Cultures { get; } = new CultureInfo[] {
            new CultureInfo("en-US"),
            new CultureInfo("bg-BG"),
        };
    }
}
