using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DevHub.Extensions.Core.TextLocalization.Abstraction
{
    public interface ITextLocalizator<T>
    {
        public string this[string key] { get; }
        public T Text { get; }
    }
}
