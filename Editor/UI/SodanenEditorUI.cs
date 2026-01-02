using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Brightness.Localization;

namespace Brightness.Utility
{
    public static class SodanenEditorUI
    {
        #region Constants

        private const string PackageName = "com.sodanen.sodanenlighteditor";
        private const string GitHubApiUrl = "https://api.github.com/repos/VRSoda/Sodanen_Light_Editor/releases/latest";
        private const float UpdateCheckInterval = 900f; // 15분마다 체크

        #endregion

        #region Update Check State

        private static bool s_updateCheckStarted;
        private static bool s_updateAvailable;
        private static string s_latestVersion = "";
        private static string s_currentVersion;
        private static double s_lastCheckTime;
        private static UnityWebRequest s_webRequest;

        private static string CurrentVersion
        {
            get
            {
                if (string.IsNullOrEmpty(s_currentVersion))
                {
                    s_currentVersion = GetPackageVersion();
                }
                return s_currentVersion;
            }
        }

        #endregion

        #region Colors

        // Theme - 전문적인 다크 테마
        public static readonly Color HeaderColor = new(191f / 255f, 222f / 255f, 255f / 255f);
        public static readonly Color AccentColor = new(191f / 255f, 222f / 255f, 255f / 255f);
        public static readonly Color WarningColor = new(0.95f, 0.7f, 0.4f);
        public static readonly Color SuccessColor = new(0.5f, 0.8f, 0.55f);
        public static readonly Color SubtleGray = new(0.5f, 0.5f, 0.5f);
        public static readonly Color DarkBgColor = new(0.15f, 0.15f, 0.15f);


        // Internal
        private static readonly Color SeparatorLight = new(0.32f, 0.32f, 0.32f);
        private static readonly Color SeparatorDark = new(0.18f, 0.18f, 0.18f);
        private static readonly Color BoxBgColor = new(0.2f, 0.2f, 0.2f);
        private static readonly Color BoxBorderColor = new(0.12f, 0.12f, 0.12f);
        private static readonly Color TextBright = new(0.88f, 0.88f, 0.88f);
        private static readonly Color TextMedium = new(0.65f, 0.65f, 0.65f);
        private static readonly Color TextDim = new(0.45f, 0.45f, 0.45f);

        #endregion

        #region Cached Styles

        private static GUIStyle s_titleStyle;
        private static GUIStyle s_subtitleStyle;
        private static GUIStyle s_sectionTitleStyle;
        private static GUIStyle s_boxStyle;
        private static GUIStyle s_innerBoxStyle;
        private static GUIStyle s_groupStyle;
        private static GUIStyle s_statusStyle;
        private static GUIStyle s_buttonStyle;
        private static GUIStyle s_smallButtonStyle;
        private static GUIStyle s_infoStyle;
        private static GUIStyle s_subHeaderStyle;
        private static GUIStyle s_versionStyle;
        private static GUIStyle s_updateStyle;
        private static Texture2D s_boxBgTexture;
        private static Color s_lastStatusColor;

        #endregion

        #region Drawing - Background & Header

        public static void DrawBackground(Rect position)
        {
            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), DarkBgColor);
        }

        public static void DrawHeader(string title = "Sodanen Light Editor")
        {
            s_titleStyle ??= new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(4, 0, 0, 0),
                margin = new RectOffset(0, 0, 0, 0),
                normal = { textColor = HeaderColor }
            };
            s_subtitleStyle ??= new GUIStyle(EditorStyles.label)
            {
                fontSize = 9,
                alignment = TextAnchor.MiddleLeft,
                padding = new RectOffset(5, 0, 0, 0),
                normal = { textColor = TextDim }
            };
            s_versionStyle ??= new GUIStyle(EditorStyles.miniLabel)
            {
                fontSize = 9,
                alignment = TextAnchor.MiddleRight,
                normal = { textColor = TextDim }
            };

            // Header
            EditorGUILayout.BeginVertical();
            {
                GUILayout.Space(4);
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField($"{title} v{CurrentVersion}", s_titleStyle);
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.LabelField("Discord : Sodanen", s_versionStyle, GUILayout.Width(110));
                }
                EditorGUILayout.EndHorizontal();

                // 업데이트 체크 시작
                CheckForUpdates();

                // 업데이트 있을 때만 알림 표시
                if (s_updateAvailable)
                {
                    s_updateStyle ??= new GUIStyle(EditorStyles.miniLabel)
                    {
                        fontSize = 9,
                        normal = { textColor = WarningColor }
                    };
                    EditorGUILayout.LabelField(LocalizationManager.L("update.available", s_latestVersion), s_updateStyle);
                }
                GUILayout.Space(4);
            }
            EditorGUILayout.EndVertical();

            // Bottom separator (AccentColor)
            var separatorRect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(separatorRect, AccentColor);
        }

        public static void DrawSeparator(Color color, int height = 1)
        {
            GUILayout.Space(4);
            var rect = EditorGUILayout.GetControlRect(false, height);
            EditorGUI.DrawRect(rect, color);
            GUILayout.Space(4);
        }

        public static void DrawThinSeparator()
        {
            GUILayout.Space(2);
            var rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, SeparatorDark);
            GUILayout.Space(2);
        }

        public static void Space(float pixels = 6) => GUILayout.Space(pixels);

        #endregion

        #region Drawing - Sections & Boxes

        public static void DrawSectionBox(string title, System.Action content)
        {
            s_sectionTitleStyle ??= new GUIStyle(EditorStyles.label)
            {
                fontSize = 10,
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(0, 0, 0, 0),
                normal = { textColor = TextBright }
            };

            EditorGUILayout.BeginVertical(BoxStyle);
            {
                // Section header
                EditorGUILayout.LabelField(title.ToUpper(), s_sectionTitleStyle);
                DrawThinSeparator();
                content?.Invoke();
            }
            EditorGUILayout.EndVertical();
        }

        public static void DrawSubSection(string title, string description, System.Action content)
        {
            EditorGUILayout.BeginVertical(InnerBoxStyle);
            {
                if (!string.IsNullOrEmpty(title))
                {
                    EditorGUILayout.LabelField(title, SubHeaderStyle);
                    GUILayout.Space(2);
                }

                if (!string.IsNullOrEmpty(description))
                {
                    EditorGUILayout.LabelField(description, InfoStyle);
                    GUILayout.Space(4);
                }

                content?.Invoke();
            }
            EditorGUILayout.EndVertical();
        }

        public static void DrawStatusBox(string message, Color color)
        {
            if (s_statusStyle == null || s_lastStatusColor != color)
            {
                s_statusStyle = new GUIStyle(EditorStyles.helpBox)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 10,
                    fontStyle = FontStyle.Normal,
                    padding = new RectOffset(10, 10, 8, 8),
                    normal = { textColor = color }
                };
                s_lastStatusColor = color;
            }
            EditorGUILayout.LabelField(message, s_statusStyle);
        }

        public static void DrawGroupLabel(string groupName)
        {
            s_groupStyle ??= new GUIStyle(EditorStyles.miniLabel)
            {
                fontSize = 9,
                fontStyle = FontStyle.Bold,
                padding = new RectOffset(0, 0, 4, 2),
                normal = { textColor = HeaderColor }
            };

            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField(groupName.ToUpper(), s_groupStyle);
                GUILayout.FlexibleSpace();

                // Decorative line
                var lineRect = GUILayoutUtility.GetRect(60, 1);
                lineRect.y += 8;
                EditorGUI.DrawRect(lineRect, SeparatorDark);

                GUILayout.Space(4);
            }
            EditorGUILayout.EndHorizontal();
        }

        #endregion

        #region Drawing - Buttons

        // 무채색 버튼 색상
        public static readonly Color ButtonGray = new(0.4f, 0.4f, 0.4f);

        public static bool DrawButton(string text, float height = 28f)
        {
            var style = height > 24 ? ButtonStyle : SmallButtonStyle;
            var originalHeight = style.fixedHeight;
            style.fixedHeight = height;

            bool clicked = GUILayout.Button(text, style);

            style.fixedHeight = originalHeight;
            return clicked;
        }

        public static bool DrawButton(string text, Color bgColor, float height = 28f)
        {
            var prevColor = GUI.backgroundColor;
            GUI.backgroundColor = bgColor;

            var style = height > 24 ? ButtonStyle : SmallButtonStyle;
            var originalHeight = style.fixedHeight;
            style.fixedHeight = height;

            bool clicked = GUILayout.Button(text, style);

            style.fixedHeight = originalHeight;
            GUI.backgroundColor = prevColor;
            return clicked;
        }

        public static void BeginButtonRow() => EditorGUILayout.BeginHorizontal();
        public static void EndButtonRow() => EditorGUILayout.EndHorizontal();

        #endregion

        #region Style Properties

        private static Texture2D BoxBgTexture
        {
            get
            {
                if (s_boxBgTexture == null)
                {
                    s_boxBgTexture = new Texture2D(1, 1);
                    s_boxBgTexture.SetPixel(0, 0, BoxBgColor);
                    s_boxBgTexture.Apply();
                }
                return s_boxBgTexture;
            }
        }

        public static GUIStyle BoxStyle => s_boxStyle ??= new GUIStyle(EditorStyles.helpBox)
        {
            padding = new RectOffset(14, 14, 10, 12),
            margin = new RectOffset(0, 0, 2, 2),
            normal = { background = BoxBgTexture }
        };

        public static GUIStyle InnerBoxStyle => s_innerBoxStyle ??= new GUIStyle(EditorStyles.helpBox)
        {
            padding = new RectOffset(12, 12, 10, 10),
            margin = new RectOffset(0, 0, 4, 4)
        };

        public static GUIStyle ButtonStyle => s_buttonStyle ??= new GUIStyle(GUI.skin.button)
        {
            fontSize = 10,
            fontStyle = FontStyle.Bold,
            fixedHeight = 26,
            padding = new RectOffset(14, 14, 4, 4),
            margin = new RectOffset(0, 0, 2, 2)
        };

        public static GUIStyle SmallButtonStyle => s_smallButtonStyle ??= new GUIStyle(GUI.skin.button)
        {
            fontSize = 9,
            fixedHeight = 20,
            padding = new RectOffset(10, 10, 2, 2),
            margin = new RectOffset(0, 0, 1, 1)
        };

        public static GUIStyle InfoStyle => s_infoStyle ??= new GUIStyle(EditorStyles.miniLabel)
        {
            wordWrap = true,
            fontSize = 9,
            padding = new RectOffset(0, 0, 0, 0),
            normal = { textColor = TextDim }
        };

        public static GUIStyle SubHeaderStyle => s_subHeaderStyle ??= new GUIStyle(EditorStyles.boldLabel)
        {
            fontSize = 10,
            padding = new RectOffset(0, 0, 0, 0),
            normal = { textColor = TextMedium }
        };

        // Legacy
        public static GUIStyle GetBoxStyle() => BoxStyle;
        public static GUIStyle GetButtonStyle() => ButtonStyle;
        public static GUIStyle GetSmallButtonStyle() => SmallButtonStyle;
        public static GUIStyle GetInfoStyle() => InfoStyle;
        public static GUIStyle GetSubHeaderStyle() => SubHeaderStyle;

        #endregion

        #region Update Check

        private static void CheckForUpdates()
        {
            // 이미 체크 중이거나 간격이 안 지났으면 스킵
            if (s_updateCheckStarted && EditorApplication.timeSinceStartup - s_lastCheckTime < UpdateCheckInterval)
            {
                // 진행 중인 요청 처리
                ProcessWebRequest();
                return;
            }

            // 새 요청 시작
            s_lastCheckTime = EditorApplication.timeSinceStartup;
            s_updateCheckStarted = true;

            s_webRequest = UnityWebRequest.Get(GitHubApiUrl);
            s_webRequest.SetRequestHeader("User-Agent", "Unity");
            s_webRequest.SendWebRequest();
        }

        private static void ProcessWebRequest()
        {
            if (s_webRequest == null || !s_webRequest.isDone) return;

            if (s_webRequest.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    var json = s_webRequest.downloadHandler.text;
                    s_latestVersion = ParseVersionFromJson(json);

                    if (!string.IsNullOrEmpty(s_latestVersion))
                    {
                        s_updateAvailable = CompareVersions(s_latestVersion, CurrentVersion) > 0;
                    }
                }
                catch (Exception)
                {
                    s_updateAvailable = false;
                }
            }

            s_webRequest.Dispose();
            s_webRequest = null;
        }

        private static string ParseVersionFromJson(string json)
        {
            // "tag_name":"v1.0.3" 또는 "tag_name":"1.0.3" 형식 파싱
            const string tagKey = "\"tag_name\":\"";
            int startIndex = json.IndexOf(tagKey, StringComparison.Ordinal);
            if (startIndex < 0) return "";

            startIndex += tagKey.Length;
            int endIndex = json.IndexOf("\"", startIndex, StringComparison.Ordinal);
            if (endIndex < 0) return "";

            var version = json.Substring(startIndex, endIndex - startIndex);
            return version.TrimStart('v', 'V');
        }

        private static int CompareVersions(string v1, string v2)
        {
            var parts1 = v1.Split('.');
            var parts2 = v2.Split('.');
            int maxLength = Math.Max(parts1.Length, parts2.Length);

            for (int i = 0; i < maxLength; i++)
            {
                int num1 = i < parts1.Length && int.TryParse(parts1[i], out int n1) ? n1 : 0;
                int num2 = i < parts2.Length && int.TryParse(parts2[i], out int n2) ? n2 : 0;

                if (num1 > num2) return 1;
                if (num1 < num2) return -1;
            }
            return 0;
        }

        private static string GetPackageVersion()
        {
            // Unity PackageManager API로 버전 가져오기
            var packageInfo = UnityEditor.PackageManager.PackageInfo.FindForAssetPath("Packages/" + PackageName);
            if (packageInfo != null)
            {
                return packageInfo.version;
            }

            // Assets 폴더 내 패키지인 경우 package.json 직접 읽기
            string[] guids = AssetDatabase.FindAssets("package t:TextAsset", new[] { "Assets/SodanenLightEditor" });
            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.EndsWith("package.json"))
                {
                    string json = File.ReadAllText(path);
                    return ParseVersionFromPackageJson(json);
                }
            }

            // 직접 경로로 시도
            string directPath = "Assets/SodanenLightEditor/package.json";
            if (File.Exists(directPath))
            {
                string json = File.ReadAllText(directPath);
                return ParseVersionFromPackageJson(json);
            }

            return "Unknown";
        }

        private static string ParseVersionFromPackageJson(string json)
        {
            // "version":"1.0.3" 형식 파싱
            const string versionKey = "\"version\":";
            int startIndex = json.IndexOf(versionKey, StringComparison.Ordinal);
            if (startIndex < 0) return "Unknown";

            startIndex += versionKey.Length;
            // 공백 스킵
            while (startIndex < json.Length && (json[startIndex] == ' ' || json[startIndex] == '"'))
                startIndex++;

            int endIndex = json.IndexOf("\"", startIndex, StringComparison.Ordinal);
            if (endIndex < 0) return "Unknown";

            return json.Substring(startIndex, endIndex - startIndex);
        }

        #endregion
    }
}
