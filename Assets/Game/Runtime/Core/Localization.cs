using Game.Runtime.Data;

namespace Game.Runtime.Core
{
    public static class LocalizationManager
    {
        public static Language CurrentLanguage { get; private set; } = Language.English;

        private static LocalizationData _data;

        public static void Init(LocalizationData data, Language lang = Language.English)
        {
            _data = data;
            CurrentLanguage = lang;
        }

        public static void SetLanguage(Language lang)
        {
            CurrentLanguage = lang;
            EventHandler.CallLanguageChangedEvent();
        }

        public static string Get(string key)
            => _data.Get(CurrentLanguage, key);
    }
}