namespace DevHub.Extensions.Core.TextLocalization
{
    using System;
    using System.Globalization;

    using Microsoft.AspNetCore.Builder;
    using Microsoft.Extensions.DependencyInjection;


    public static class ServiceExtensions
    {
        private static TextLocalizationConfiguration defConfiguration { get; set; } = new();

        /// <summary>
        /// Extendet methotd for TextLocalizator<T>.
        /// </summary>
        /// <typeparam name="T">TLanguage class</typeparam>
        /// <param name="textLocalizator">TextLocalizator Service</param>
        /// <returns>Default service configuration.</returns>
        public static TextLocalizationConfiguration Configuration<T>(this TextLocalizator<T> textLocalizator)
            where T : new()
        {
            return defConfiguration;
        }

        /// <summary>
        /// Add Default implementation of DevHub.Extensions.Core.TextLocalization.
        /// </summary>
        /// <retrn name="options">Localization options</param>
        /// <param name="supportedCultures">list of suported cultures. By default: [en-EN,bg-BG]</param>
        /// <param name="configuration">Service Configuration</param>
        public static void AddDevHubTextLocalization(this IServiceCollection services,
                                                          Action<RequestLocalizationOptions> options = null,
                                                          CultureInfo[] supportedCultures = null,
                                                          Action<TextLocalizationConfiguration> configuration = null)
        {
            bool isOptions = options == null ? true : false;
            options = (o) =>
            {
                if (isOptions)
                {
                    o.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("en");
                }
                o.SupportedUICultures = supportedCultures ?? SupportedCultures.Cultures;
                o.SupportedCultures = supportedCultures ?? SupportedCultures.Cultures;
            };

            if(configuration != null) { configuration.Invoke(defConfiguration); }

            services.Configure<RequestLocalizationOptions>(options);
            services.AddDistributedMemoryCache();
            services.AddSingleton(typeof(ITextLocalizator<>), typeof(TextLocalizator<>));

        }
    }
}
