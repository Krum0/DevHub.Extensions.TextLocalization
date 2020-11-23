using DevHub.Extensions.Core.TextLocalization.Abstraction;
using DevHub.Extensions.Core.TextLocalization.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevHub.Extensions.Core.TextLocalization
{
    public static class ServiceExtensions
    {
        private static TextLocalizationConfiguration defConfiguration { get; set; }

        public static TextLocalizationConfiguration Configuration<T>(this TextLocalizator<T> textLocalizator)
            where T : new()
        {
            return defConfiguration;
        }

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

            if(configuration == null) { defConfiguration = new(); } else
            {
                configuration.Invoke(defConfiguration);
            }



            services.Configure<RequestLocalizationOptions>(options);
            services.AddDistributedMemoryCache();
            services.AddSingleton(typeof(ITextLocalizator<>), typeof(TextLocalizator<>));

        }
    }
}
