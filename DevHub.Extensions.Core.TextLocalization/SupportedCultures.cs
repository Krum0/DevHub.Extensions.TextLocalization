using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevHub.Extensions.Core.TextLocalization
{
    public class SupportedCultures
    {
        public static CultureInfo[] Cultures { get; } = new CultureInfo[] {
            new CultureInfo("en-US"),
            new CultureInfo("bg-BG"),
        };
    }
}
