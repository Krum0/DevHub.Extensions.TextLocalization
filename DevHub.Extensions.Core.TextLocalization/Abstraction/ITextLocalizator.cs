namespace DevHub.Extensions.Core.TextLocalization
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TLanguage">TLanguage class</typeparam>
    public interface ITextLocalizator<TLanguage>
    {
        public string this[string key] { get; }
        public TLanguage Text { get; }
    }
}
