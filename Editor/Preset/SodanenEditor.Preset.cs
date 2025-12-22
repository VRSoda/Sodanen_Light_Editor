using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Brightness.Localization;
using static Brightness.Localization.Loc;

namespace Brightness.Utility
{
    public partial class SodanenEditor
    {
        private bool _showPresetSection = false;
        private int _selectedPresetIndex = -1;
        private string _newPresetName = "";
        private string _newPresetDescription = "";
        private bool _showSavePresetPopup;
        private bool _showRenamePopup;
        private string _renamePresetName = "";
        private Vector2 _presetScrollPosition;

        private List<BrightnessPreset> _presets = new List<BrightnessPreset>();

        private void InitPresets()
        {
            RefreshPresetList();
            if (_presets.Count == 0)
            {
                PresetManager.CreateDefaultPresets();
                RefreshPresetList();
            }
        }

        private void RefreshPresetList()
        {
            _presets = PresetManager.GetAllPresets();
        }

        private void DrawPresetSection()
        {
            SodanenEditorUI.DrawSectionBox(L("preset.title"), () =>
            {
                GUILayout.Space(5);
                DrawPresetHeader();

                if (!_showPresetSection) return;

                GUILayout.Space(5);
                DrawPresetList();
                GUILayout.Space(5);
                DrawPresetButtons();

                if (_showSavePresetPopup)
                {
                    DrawSavePresetPopup();
                }

                if (_showRenamePopup)
                {
                    DrawRenamePopup();
                }
            });
        }

        private void DrawPresetHeader()
        {
            EditorGUILayout.BeginHorizontal();
            _showPresetSection = EditorGUILayout.Foldout(_showPresetSection, L("preset.management"), true);

            if (GUILayout.Button(L("preset.refresh"), SodanenEditorUI.SmallButtonStyle, GUILayout.Width(60)))
            {
                PresetManager.RefreshCache();
                RefreshPresetList();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawPresetList()
        {
            if (_presets.Count == 0)
            {
                EditorGUILayout.HelpBox(L("preset.empty"), MessageType.Info);
                return;
            }

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            var maxHeight = Mathf.Min(_presets.Count * 28 + 10, 150);
            _presetScrollPosition = EditorGUILayout.BeginScrollView(
                _presetScrollPosition,
                GUILayout.MaxHeight(maxHeight)
            );

            for (int i = 0; i < _presets.Count; i++)
            {
                DrawPresetItem(i, _presets[i]);
            }

            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();

            if (_selectedPresetIndex >= 0 && _selectedPresetIndex < _presets.Count)
            {
                var selectedPreset = _presets[_selectedPresetIndex];
                if (!string.IsNullOrEmpty(selectedPreset.Description))
                {
                    EditorGUILayout.LabelField(selectedPreset.Description, EditorStyles.miniLabel);
                }
            }
        }

        private void DrawPresetItem(int index, BrightnessPreset preset)
        {
            var isSelected = _selectedPresetIndex == index;
            var bgColor = isSelected ? new Color(0.3f, 0.5f, 0.8f, 0.5f) : Color.clear;

            var rect = EditorGUILayout.BeginHorizontal();
            EditorGUI.DrawRect(rect, bgColor);

            var customCount = preset.CustomMaterials?.Count ?? 0;
            var displayName = customCount > 0 ? $"{preset.Name} ({customCount})" : preset.Name;

            if (GUILayout.Button(displayName, isSelected ? EditorStyles.boldLabel : EditorStyles.label))
            {
                _selectedPresetIndex = index;
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawPresetButtons()
        {
            var hasSelection = _selectedPresetIndex >= 0 && _selectedPresetIndex < _presets.Count;

            SodanenEditorUI.BeginButtonRow();
            if (SodanenEditorUI.DrawButton(L("preset.save_current"), SodanenEditorUI.ButtonGray, 25))
            {
                _newPresetName = L("preset.new_name", _presets.Count + 1);
                _newPresetDescription = "";
                _showSavePresetPopup = true;
            }

            EditorGUI.BeginDisabledGroup(!hasSelection);
            if (SodanenEditorUI.DrawButton(L("preset.apply"), SodanenEditorUI.ButtonGray, 25))
            {
                ApplySelectedPreset();
            }
            EditorGUI.EndDisabledGroup();
            SodanenEditorUI.EndButtonRow();

            GUILayout.Space(3);

            SodanenEditorUI.BeginButtonRow();
            EditorGUI.BeginDisabledGroup(!hasSelection);

            if (SodanenEditorUI.DrawButton(L("preset.rename"), SodanenEditorUI.ButtonGray, 22))
            {
                if (hasSelection)
                {
                    _renamePresetName = _presets[_selectedPresetIndex].Name;
                    _showRenamePopup = true;
                }
            }

            if (SodanenEditorUI.DrawButton(L("preset.delete"), SodanenEditorUI.ButtonGray, 22))
            {
                if (hasSelection)
                {
                    DeleteSelectedPreset();
                }
            }

            EditorGUI.EndDisabledGroup();

            if (SodanenEditorUI.DrawButton(L("preset.export"), SodanenEditorUI.ButtonGray, 22))
            {
                ExportPreset();
            }

            if (SodanenEditorUI.DrawButton(L("preset.import"), SodanenEditorUI.ButtonGray, 22))
            {
                ImportPreset();
            }
            SodanenEditorUI.EndButtonRow();
        }

        private void DrawSavePresetPopup()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(L("preset.save_title"), EditorStyles.boldLabel);

            _newPresetName = EditorGUILayout.TextField(L("preset.name_label"), _newPresetName);
            _newPresetDescription = EditorGUILayout.TextField(L("preset.desc_label"), _newPresetDescription);

            SodanenEditorUI.BeginButtonRow();
            if (SodanenEditorUI.DrawButton(L("dialog.save"), SodanenEditorUI.ButtonGray, 22))
            {
                if (!string.IsNullOrWhiteSpace(_newPresetName))
                {
                    SaveCurrentAsPreset(_newPresetName, _newPresetDescription);
                    _showSavePresetPopup = false;
                }
                else
                {
                    EditorUtility.DisplayDialog(L("dialog.error"), L("preset.name_required"), L("dialog.confirm"));
                }
            }

            if (SodanenEditorUI.DrawButton(L("dialog.cancel"), SodanenEditorUI.ButtonGray, 22))
            {
                _showSavePresetPopup = false;
            }
            SodanenEditorUI.EndButtonRow();

            EditorGUILayout.EndVertical();
        }

        private void DrawRenamePopup()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField(L("preset.rename_title"), EditorStyles.boldLabel);

            _renamePresetName = EditorGUILayout.TextField(L("preset.new_name_label"), _renamePresetName);

            SodanenEditorUI.BeginButtonRow();
            if (SodanenEditorUI.DrawButton(L("dialog.change"), SodanenEditorUI.ButtonGray, 22))
            {
                if (!string.IsNullOrWhiteSpace(_renamePresetName))
                {
                    var oldName = _presets[_selectedPresetIndex].Name;
                    PresetManager.RenamePreset(oldName, _renamePresetName);
                    RefreshPresetList();
                    _showRenamePopup = false;
                }
            }

            if (SodanenEditorUI.DrawButton(L("dialog.cancel"), SodanenEditorUI.ButtonGray, 22))
            {
                _showRenamePopup = false;
            }
            SodanenEditorUI.EndButtonRow();

            EditorGUILayout.EndVertical();
        }

        private void SaveCurrentAsPreset(string name, string description)
        {
            var preset = new BrightnessPreset(name)
            {
                Description = description
            };
            preset.CopyFrom(_unifySettings, _customMaterialEntries);

            if (PresetManager.PresetExists(name))
            {
                if (!EditorUtility.DisplayDialog(L("preset.overwrite_title"),
                    L("preset.overwrite_message", name),
                    L("dialog.overwrite"), L("dialog.cancel")))
                {
                    return;
                }
            }

            PresetManager.SavePreset(preset);
            RefreshPresetList();

            _selectedPresetIndex = _presets.FindIndex(p => p.Name == name);
            EditorUtility.DisplayDialog(L("preset.save_complete_title"), L("preset.save_complete_message", name), L("dialog.confirm"));
        }

        private void ApplySelectedPreset()
        {
            if (_selectedPresetIndex < 0 || _selectedPresetIndex >= _presets.Count) return;

            var preset = _presets[_selectedPresetIndex];
            preset.ApplyTo(_unifySettings, _customMaterialEntries);

            var customCount = preset.CustomMaterials?.Count ?? 0;
            Debug.Log(L("preset.applied_log", preset.Name, customCount));
        }

        private void DeleteSelectedPreset()
        {
            if (_selectedPresetIndex < 0 || _selectedPresetIndex >= _presets.Count) return;

            var preset = _presets[_selectedPresetIndex];
            if (EditorUtility.DisplayDialog(L("preset.delete_title"),
                L("preset.delete_message", preset.Name),
                L("preset.delete"), L("dialog.cancel")))
            {
                PresetManager.DeletePreset(preset.Name);
                RefreshPresetList();
                _selectedPresetIndex = -1;
            }
        }

        private void ExportPreset()
        {
            if (_selectedPresetIndex < 0 || _selectedPresetIndex >= _presets.Count)
            {
                EditorUtility.DisplayDialog(L("preset.export"), L("preset.export_select"), L("dialog.confirm"));
                return;
            }

            var preset = _presets[_selectedPresetIndex];
            var path = EditorUtility.SaveFilePanel(
                L("preset.export_title"),
                "",
                preset.Name + ".json",
                "json"
            );

            if (!string.IsNullOrEmpty(path))
            {
                PresetManager.ExportPreset(preset, path);
                EditorUtility.DisplayDialog(L("preset.export_complete_title"), L("preset.export_complete_message", path), L("dialog.confirm"));
            }
        }

        private void ImportPreset()
        {
            var path = EditorUtility.OpenFilePanel(
                L("preset.import_title"),
                "",
                "json"
            );

            if (!string.IsNullOrEmpty(path))
            {
                var preset = PresetManager.ImportPreset(path);
                if (preset != null)
                {
                    RefreshPresetList();
                    _selectedPresetIndex = _presets.FindIndex(p => p.Name == preset.Name);
                    EditorUtility.DisplayDialog(L("preset.import_complete_title"), L("preset.import_complete_message", preset.Name), L("dialog.confirm"));
                }
            }
        }
    }
}
