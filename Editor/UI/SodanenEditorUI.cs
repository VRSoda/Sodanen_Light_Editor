using UnityEditor;
using UnityEngine;

namespace Brightness.Utility
{
    public static class SodanenEditorUI
    {
        // Colors
        public static readonly Color HeaderColor = new Color(0.4f, 0.7f, 1f);
        public static readonly Color AccentColor = new Color(0.3f, 0.8f, 0.5f);
        public static readonly Color WarningColor = new Color(1f, 0.7f, 0.3f);
        public static readonly Color DarkBgColor = new Color(0.18f, 0.18f, 0.18f);
        private static readonly Color s_separatorColor = new Color(0.4f, 0.4f, 0.4f);

        // Cached styles
        private static GUIStyle s_titleStyle;
        private static GUIStyle s_subtitleStyle;
        private static GUIStyle s_sectionTitleStyle;
        private static GUIStyle s_boxStyle;
        private static GUIStyle s_groupStyle;
        private static GUIStyle s_statusStyle;
        private static Color s_lastStatusColor;

        public static void DrawBackground(Rect position)
        {
            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), DarkBgColor);
        }

        public static void DrawHeader()
        {
            s_titleStyle ??= new GUIStyle(EditorStyles.boldLabel) { fontSize = 20, alignment = TextAnchor.MiddleLeft, normal = { textColor = HeaderColor } };
            s_subtitleStyle ??= new GUIStyle(EditorStyles.label) { fontSize = 11, alignment = TextAnchor.MiddleLeft, normal = { textColor = Color.gray } };

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("SODANEN", s_titleStyle);
            EditorGUILayout.LabelField("밝기 조절 에디터", s_subtitleStyle);
            GUILayout.Space(8);
            DrawSeparator(HeaderColor, 2);
            EditorGUILayout.EndVertical();
        }

        public static void DrawSectionBox(string title, System.Action content)
        {
            s_sectionTitleStyle ??= new GUIStyle(EditorStyles.boldLabel) { fontSize = 11, normal = { textColor = Color.white } };
            
            EditorGUILayout.BeginVertical(GetBoxStyle());
            EditorGUILayout.LabelField(title, s_sectionTitleStyle);
            DrawSeparator(s_separatorColor, 1);
            content?.Invoke();
            GUILayout.Space(5);
            EditorGUILayout.EndVertical();
        }

        public static GUIStyle GetBoxStyle()
        {
            s_boxStyle ??= new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(12, 12, 8, 8),
                margin = new RectOffset(5, 5, 3, 3)
            };
            return s_boxStyle;
        }

        public static void DrawSeparator(Color color, int height)
        {
            var rect = EditorGUILayout.GetControlRect(false, height);
            EditorGUI.DrawRect(rect, color);
        }

        public static void DrawStatusBox(string message, Color color)
        {
            if (s_statusStyle == null || s_lastStatusColor != color)
            {
                s_statusStyle = new GUIStyle(EditorStyles.helpBox)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 10,
                    normal = { textColor = color }
                };
                s_lastStatusColor = color;
            }
            EditorGUILayout.LabelField(message, s_statusStyle);
        }

        public static void DrawGroupLabel(string groupName)
        {
            s_groupStyle ??= new GUIStyle(EditorStyles.miniLabel) { fontStyle = FontStyle.Bold, normal = { textColor = HeaderColor } };
            EditorGUILayout.LabelField(groupName, s_groupStyle);
        }
    }
}
