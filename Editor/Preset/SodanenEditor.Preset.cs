using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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
            SodanenEditorUI.DrawSectionBox("프리셋", () =>
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
            _showPresetSection = EditorGUILayout.Foldout(_showPresetSection, "프리셋 관리", true);

            if (GUILayout.Button("새로고침", GUILayout.Width(60)))
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
                EditorGUILayout.HelpBox("저장된 프리셋이 없습니다.", MessageType.Info);
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
            EditorGUILayout.BeginHorizontal();

            GUI.backgroundColor = new Color(0.3f, 0.8f, 0.5f);
            if (GUILayout.Button("현재 설정 저장", GUILayout.Height(25)))
            {
                _newPresetName = "새 프리셋 " + (_presets.Count + 1);
                _newPresetDescription = "";
                _showSavePresetPopup = true;
            }

            var hasSelection = _selectedPresetIndex >= 0 && _selectedPresetIndex < _presets.Count;

            GUI.backgroundColor = hasSelection ? new Color(0.4f, 0.7f, 1f) : Color.gray;
            EditorGUI.BeginDisabledGroup(!hasSelection);
            if (GUILayout.Button("적용", GUILayout.Height(25)))
            {
                ApplySelectedPreset();
            }
            EditorGUI.EndDisabledGroup();

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(3);

            EditorGUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(!hasSelection);

            if (GUILayout.Button("이름 변경", GUILayout.Height(22)))
            {
                if (hasSelection)
                {
                    _renamePresetName = _presets[_selectedPresetIndex].Name;
                    _showRenamePopup = true;
                }
            }

            GUI.backgroundColor = new Color(1f, 0.4f, 0.4f);
            if (GUILayout.Button("삭제", GUILayout.Height(22)))
            {
                if (hasSelection)
                {
                    DeleteSelectedPreset();
                }
            }
            GUI.backgroundColor = Color.white;

            EditorGUI.EndDisabledGroup();

            if (GUILayout.Button("내보내기", GUILayout.Height(22)))
            {
                ExportPreset();
            }

            if (GUILayout.Button("가져오기", GUILayout.Height(22)))
            {
                ImportPreset();
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawSavePresetPopup()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("새 프리셋 저장", EditorStyles.boldLabel);

            _newPresetName = EditorGUILayout.TextField("이름", _newPresetName);
            _newPresetDescription = EditorGUILayout.TextField("설명", _newPresetDescription);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("저장"))
            {
                if (!string.IsNullOrWhiteSpace(_newPresetName))
                {
                    SaveCurrentAsPreset(_newPresetName, _newPresetDescription);
                    _showSavePresetPopup = false;
                }
                else
                {
                    EditorUtility.DisplayDialog("오류", "프리셋 이름을 입력해주세요.", "확인");
                }
            }

            if (GUILayout.Button("취소"))
            {
                _showSavePresetPopup = false;
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void DrawRenamePopup()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField("프리셋 이름 변경", EditorStyles.boldLabel);

            _renamePresetName = EditorGUILayout.TextField("새 이름", _renamePresetName);

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("변경"))
            {
                if (!string.IsNullOrWhiteSpace(_renamePresetName))
                {
                    var oldName = _presets[_selectedPresetIndex].Name;
                    PresetManager.RenamePreset(oldName, _renamePresetName);
                    RefreshPresetList();
                    _showRenamePopup = false;
                }
            }

            if (GUILayout.Button("취소"))
            {
                _showRenamePopup = false;
            }

            EditorGUILayout.EndHorizontal();
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
                if (!EditorUtility.DisplayDialog("프리셋 덮어쓰기",
                    $"'{name}' 프리셋이 이미 존재합니다.\n덮어쓰시겠습니까?",
                    "덮어쓰기", "취소"))
                {
                    return;
                }
            }

            PresetManager.SavePreset(preset);
            RefreshPresetList();

            _selectedPresetIndex = _presets.FindIndex(p => p.Name == name);
            EditorUtility.DisplayDialog("저장 완료", $"프리셋 '{name}'이(가) 저장되었습니다.", "확인");
        }

        private void ApplySelectedPreset()
        {
            if (_selectedPresetIndex < 0 || _selectedPresetIndex >= _presets.Count) return;

            var preset = _presets[_selectedPresetIndex];
            preset.ApplyTo(_unifySettings, _customMaterialEntries);

            var customCount = preset.CustomMaterials?.Count ?? 0;
            Debug.Log($"[Preset] '{preset.Name}' 프리셋 적용됨 (개별 마테리얼: {customCount}개)");
        }

        private void DeleteSelectedPreset()
        {
            if (_selectedPresetIndex < 0 || _selectedPresetIndex >= _presets.Count) return;

            var preset = _presets[_selectedPresetIndex];
            if (EditorUtility.DisplayDialog("프리셋 삭제",
                $"'{preset.Name}' 프리셋을 삭제하시겠습니까?\n이 작업은 되돌릴 수 없습니다.",
                "삭제", "취소"))
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
                EditorUtility.DisplayDialog("내보내기", "먼저 내보낼 프리셋을 선택해주세요.", "확인");
                return;
            }

            var preset = _presets[_selectedPresetIndex];
            var path = EditorUtility.SaveFilePanel(
                "프리셋 내보내기",
                "",
                preset.Name + ".json",
                "json"
            );

            if (!string.IsNullOrEmpty(path))
            {
                PresetManager.ExportPreset(preset, path);
                EditorUtility.DisplayDialog("내보내기 완료", $"프리셋이 저장되었습니다:\n{path}", "확인");
            }
        }

        private void ImportPreset()
        {
            var path = EditorUtility.OpenFilePanel(
                "프리셋 가져오기",
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
                    EditorUtility.DisplayDialog("가져오기 완료", $"프리셋 '{preset.Name}'이(가) 가져왔습니다.", "확인");
                }
            }
        }
    }
}