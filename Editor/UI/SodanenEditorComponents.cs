using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using static Brightness.Localization.Loc;

namespace Brightness.Utility
{
    public static class SodanenEditorComponents
    {
        public static void DrawFeatureWithMaterials(string name, string desc,
            ref bool enabled, ref bool showMaterials, Dictionary<string, bool> materials,
            ICollection<string> allMaterialPaths)
        {
            EditorGUILayout.BeginHorizontal();

            enabled = EditorGUILayout.Toggle(enabled, GUILayout.Width(20));
            EditorGUILayout.LabelField(new GUIContent(name, desc), GUILayout.MinWidth(80));

            using (new EditorGUI.DisabledScope(!enabled || !allMaterialPaths.Any()))
            {
                int selectedCount = materials.Count(x => x.Value);
                if (GUILayout.Button($"[{selectedCount}/{materials.Count}]", GUILayout.Width(50)))
                {
                    showMaterials = !showMaterials;
                }
            }

            EditorGUILayout.EndHorizontal();

            if (showMaterials && enabled && materials.Any())
            {
                EditorGUI.indentLevel++;
                DrawMaterialList(materials);
                EditorGUI.indentLevel--;
            }
        }

        private static void DrawMaterialList(Dictionary<string, bool> materials)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button(L("material.select_all"), GUILayout.Width(45))) SetAllMaterials(materials, true);
            if (GUILayout.Button(L("material.deselect_all"), GUILayout.Width(45))) SetAllMaterials(materials, false);
            
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(3);

            var materialKeys = materials.Keys.ToList();
            foreach (var path in materialKeys)
            {
                string displayName = path.Split('/').LastOrDefault() ?? path;
                materials[path] = EditorGUILayout.ToggleLeft(displayName, materials[path]);
            }

            EditorGUILayout.EndVertical();
        }
        
        private static void SetAllMaterials(Dictionary<string, bool> materials, bool value)
        {
            var keys = materials.Keys.ToList();
            foreach (var key in keys)
            {
                materials[key] = value;
            }
        }
    }
}
