using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DevHub.Extensions.Core.TextLocalization
{
    public class TextLocalizationConfiguration
    {
        public TextLocalizationConfiguration()
        {
            var exePath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            this.DefaultPath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", "Language");
        }

        public string DefaultPath { get; set; }
    }
}
