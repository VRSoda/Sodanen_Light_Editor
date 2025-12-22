using System;
using System.Collections.Generic;
using UnityEditor;

namespace Brightness.Localization
{
    public enum Language
    {
        Korean,
        English,
        Japanese,
        Chinese
    }

    public static class LocalizationManager
    {
        private const string PREF_KEY = "SodanenEditor_Language";

        private static Language _currentLanguage;
        private static Dictionary<string, string> _currentStrings;
        private static bool _initialized = false;
        private static string _lastLilToonLanguage = "";

        public static Language CurrentLanguage
        {
            get
            {
                EnsureInitialized();
                return _currentLanguage;
            }
            set
            {
                if (_currentLanguage != value)
                {
                    _currentLanguage = value;
                    EditorPrefs.SetInt(PREF_KEY, (int)value);
                    LoadLanguage(value);
                }
            }
        }

        public static string[] LanguageNames => new[] { "한국어", "English", "日本語", "中文" };

        private static void EnsureInitialized()
        {
            if (!_initialized)
            {
                SyncWithLilToonLanguage();
                LoadLanguage(_currentLanguage);
                _initialized = true;
            }
        }

        /// <summary>
        /// lilToon 언어 설정을 확인하고 에디터 언어를 동기화합니다.
        /// lilToon 언어 설정이 항상 우선됩니다.
        /// </summary>
        public static void CheckAndSyncLilToonLanguage()
        {
            string lilToonLang = GetLilToonLanguage();
            if (string.IsNullOrEmpty(lilToonLang)) return;

            // lilToon 언어가 변경되었는지 확인
            if (lilToonLang == _lastLilToonLanguage) return;

            _lastLilToonLanguage = lilToonLang;
            Language mappedLanguage = MapLilToonLanguageCode(lilToonLang);

            // lilToon 언어와 현재 언어가 다르면 동기화
            if (_currentLanguage != mappedLanguage)
            {
                _currentLanguage = mappedLanguage;
                EditorPrefs.SetInt(PREF_KEY, (int)_currentLanguage);
                LoadLanguage(_currentLanguage);
            }
        }

        /// <summary>
        /// lilToon Settings에서 언어 코드를 가져옵니다.
        /// </summary>
        private static string GetLilToonLanguage()
        {
            try
            {
                // Unity Preferences 폴더 경로 가져오기
                string prefsFolder = UnityEditorInternal.InternalEditorUtility.unityPreferencesFolder;
                string prefsPath = System.IO.Path.Combine(prefsFolder, "jp.lilxyzw/liltoon.asset");

                if (!System.IO.File.Exists(prefsPath)) return null;

                // YAML 파일에서 language 필드 읽기
                string[] lines = System.IO.File.ReadAllLines(prefsPath);
                foreach (string line in lines)
                {
                    if (line.Trim().StartsWith("language:"))
                    {
                        return line.Substring(line.IndexOf(':') + 1).Trim();
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        private static void SyncWithLilToonLanguage()
        {
            string lilToonLang = GetLilToonLanguage();
            if (!string.IsNullOrEmpty(lilToonLang))
            {
                _lastLilToonLanguage = lilToonLang;
                _currentLanguage = MapLilToonLanguageCode(lilToonLang);
                EditorPrefs.SetInt(PREF_KEY, (int)_currentLanguage);
            }
            else
            {
                _currentLanguage = (Language)EditorPrefs.GetInt(PREF_KEY, 0);
            }
        }

        /// <summary>
        /// lilToon 언어 코드를 Language enum으로 변환합니다.
        /// lilToon codes: "en-US", "ja-JP", "ko-KR", "zh-Hans", "zh-Hant"
        /// </summary>
        private static Language MapLilToonLanguageCode(string langCode)
        {
            if (string.IsNullOrEmpty(langCode)) return Language.Korean;

            // 언어 코드의 앞 2글자로 판단
            string prefix = langCode.Length >= 2 ? langCode.Substring(0, 2).ToLower() : langCode.ToLower();

            return prefix switch
            {
                "ko" => Language.Korean,
                "en" => Language.English,
                "ja" => Language.Japanese,
                "zh" => Language.Chinese,
                _ => Language.Korean
            };
        }

        private static void LoadLanguage(Language language)
        {
            _currentStrings = language switch
            {
                Language.Korean => Korean.Strings,
                Language.English => English.Strings,
                Language.Japanese => Japanese.Strings,
                Language.Chinese => Chinese.Strings,
                _ => Korean.Strings
            };
        }

        /// <summary>
        /// Get localized string by key. Short alias for convenience.
        /// </summary>
        public static string L(string key)
        {
            EnsureInitialized();

            if (_currentStrings != null && _currentStrings.TryGetValue(key, out var value))
            {
                return value;
            }

            // Fallback to Korean
            if (Korean.Strings.TryGetValue(key, out var fallback))
            {
                return fallback;
            }

            return $"[{key}]";
        }

        /// <summary>
        /// Get localized string with format arguments.
        /// </summary>
        public static string L(string key, params object[] args)
        {
            var format = L(key);
            try
            {
                return string.Format(format, args);
            }
            catch
            {
                return format;
            }
        }
    }

    // Static import helper for shorter syntax
    public static class Loc
    {
        public static string L(string key) => LocalizationManager.L(key);
        public static string L(string key, params object[] args) => LocalizationManager.L(key, args);
    }
}
