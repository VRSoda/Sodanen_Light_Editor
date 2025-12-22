using UnityEditor;
using UnityEngine;

namespace Brightness.Utility
{
    public partial class SodanenEditor
    {
        private void DrawCustomMaterialSectionContent()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawCustomMaterialHeader();

            if (_showCustomMaterialSection)
            {
                if (_customMaterialEntries.Count == 0)
                {
                    EditorGUILayout.LabelField("+ 버튼으로 마테리얼을 추가하세요", SodanenEditorUI.InfoStyle);
                }
                else
                {
                    GUILayout.Space(5);
                    DrawCustomMaterialList();

                    GUILayout.Space(5);
                    EditorGUILayout.LabelField("이 마테리얼들은 '마테리얼 속성 통일'에서 제외됩니다", SodanenEditorUI.InfoStyle);
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawCustomMaterialHeader()
        {
            EditorGUILayout.BeginHorizontal();
            _showCustomMaterialSection = EditorGUILayout.Foldout(
                _showCustomMaterialSection, $"개별 마테리얼 그림자 조절 ({_customMaterialEntries.Count}개)", true);

            GUI.backgroundColor = new Color(0.3f, 0.8f, 0.5f);
            if (GUILayout.Button("+", GUILayout.Width(25)))
            {
                _customMaterialEntries.Add(new CustomMaterialShadowEntry());
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
        }

        private void DrawCustomMaterialList()
        {
            var removeIndex = -1;

            for (var i = 0; i < _customMaterialEntries.Count; i++)
            {
                if (DrawCustomMaterialEntry(_customMaterialEntries[i]))
                {
                    removeIndex = i;
                }
            }

            if (removeIndex >= 0)
            {
                _customMaterialEntries.RemoveAt(removeIndex);
            }
        }

        private bool DrawCustomMaterialEntry(CustomMaterialShadowEntry entry)
        {
            var shouldRemove = false;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            EditorGUILayout.BeginHorizontal();
            entry.IsExpanded = EditorGUILayout.Foldout(
                entry.IsExpanded, entry.Material != null ? entry.Material.name : "(마테리얼 선택)", true);

            GUI.backgroundColor = new Color(1f, 0.4f, 0.4f);
            if (GUILayout.Button("X", GUILayout.Width(22)))
            {
                shouldRemove = true;
            }
            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();

            if (entry.IsExpanded)
            {
                DrawCustomMaterialEntryContent(entry);
            }

            EditorGUILayout.EndVertical();
            GUILayout.Space(2);

            return shouldRemove;
        }

        private void DrawCustomMaterialEntryContent(CustomMaterialShadowEntry entry)
        {
            DrawMaterialField(entry);

            if (entry.Material == null) return;

            GUILayout.Space(3);
            DrawGroupSlider("강도", ref entry.ShadowStrength, 0f, 1f);
            GUILayout.Space(3);

            DrawShadow1stGroup(entry);
            GUILayout.Space(3);
            DrawShadow2ndGroup(entry);
            GUILayout.Space(3);
            DrawShadow3rdGroup(entry);
        }

        private void DrawMaterialField(CustomMaterialShadowEntry entry)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("마테리얼", GUILayout.Width(60));
            var newMaterial = (Material)EditorGUILayout.ObjectField(entry.Material, typeof(Material), false);
            if (newMaterial != entry.Material)
            {
                entry.Material = newMaterial;
                entry.ReadFromMaterial();
            }
            if (entry.Material != null && GUILayout.Button("현재값", GUILayout.Width(50)))
            {
                entry.ReadFromMaterial();
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawShadow1stGroup(CustomMaterialShadowEntry entry)
        {
            EditorGUILayout.LabelField("그림자 1", EditorStyles.miniLabel);
            DrawGroupColor("색상", ref entry.Shadow1stColor);
            DrawGroupSlider("범위", ref entry.Shadow1stBorder, 0f, 1f);
            DrawGroupSlider("흐리게", ref entry.Shadow1stBlur, 0f, 1f);
        }

        private void DrawShadow2ndGroup(CustomMaterialShadowEntry entry)
        {
            EditorGUILayout.LabelField("그림자 2", EditorStyles.miniLabel);
            DrawGroupColor("색상", ref entry.Shadow2ndColor);
            DrawGroupSlider("투명도", ref entry.Shadow2ndAlpha, 0f, 1f);
            DrawGroupSlider("범위", ref entry.Shadow2ndBorder, 0f, 1f);
            DrawGroupSlider("흐리게", ref entry.Shadow2ndBlur, 0f, 1f);
        }

        private void DrawShadow3rdGroup(CustomMaterialShadowEntry entry)
        {
            EditorGUILayout.LabelField("그림자 3", EditorStyles.miniLabel);
            DrawGroupColor("색상", ref entry.Shadow3rdColor);
            DrawGroupSlider("투명도", ref entry.Shadow3rdAlpha, 0f, 1f);
        }

        private static void DrawGroupSlider(string label, ref float value, float min, float max)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(60));
            value = EditorGUILayout.Slider(value, min, max);
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawGroupColor(string label, ref Color value)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(60));
            value = EditorGUILayout.ColorField(value);
            EditorGUILayout.EndHorizontal();
        }
    }
}
