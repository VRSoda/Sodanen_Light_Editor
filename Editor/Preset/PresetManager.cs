using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Brightness.Utility
{
    public static class PresetManager
    {
        private const string PRESET_FOLDER = "Assets/BRIGHTNESS_CONTROL/Presets";
        private const string PRESET_EXTENSION = ".json";

        private static List<BrightnessPreset> _cache;

        public static List<BrightnessPreset> GetAllPresets()
        {
            LoadIfNeeded();
            return _cache;
        }

        public static void SavePreset(BrightnessPreset preset)
        {
            LoadIfNeeded();
            EnsurePresetFolder();

            var filePath = GetPresetFilePath(preset.Name);
            var json = JsonUtility.ToJson(preset, true);
            File.WriteAllText(filePath, json);

            var existingIndex = _cache.FindIndex(p => p.Name == preset.Name);
            if (existingIndex >= 0)
            {
                _cache[existingIndex] = preset;
            }
            else
            {
                _cache.Add(preset);
            }

            AssetDatabase.Refresh();
            Debug.Log($"[PresetManager] 프리셋 '{preset.Name}' 저장됨: {filePath}");
        }

        public static void DeletePreset(string presetName)
        {
            LoadIfNeeded();

            var filePath = GetPresetFilePath(presetName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                var metaPath = filePath + ".meta";
                if (File.Exists(metaPath))
                {
                    File.Delete(metaPath);
                }
            }

            _cache.RemoveAll(p => p.Name == presetName);
            AssetDatabase.Refresh();
            Debug.Log($"[PresetManager] 프리셋 '{presetName}' 삭제됨");
        }

        public static void RenamePreset(string oldName, string newName)
        {
            LoadIfNeeded();

            var preset = _cache.Find(p => p.Name == oldName);
            if (preset == null) return;

            var oldPath = GetPresetFilePath(oldName);
            var newPath = GetPresetFilePath(newName);

            if (File.Exists(oldPath))
            {
                File.Delete(oldPath);
                var metaPath = oldPath + ".meta";
                if (File.Exists(metaPath))
                {
                    File.Delete(metaPath);
                }
            }

            preset.Name = newName;
            var json = JsonUtility.ToJson(preset, true);
            File.WriteAllText(newPath, json);

            AssetDatabase.Refresh();
            Debug.Log($"[PresetManager] 프리셋 '{oldName}' → '{newName}' 이름 변경됨");
        }

        public static BrightnessPreset GetPreset(string name)
        {
            LoadIfNeeded();
            return _cache.Find(p => p.Name == name);
        }

        public static bool PresetExists(string name)
        {
            LoadIfNeeded();
            return _cache.Exists(p => p.Name == name);
        }

        public static void ExportPreset(BrightnessPreset preset, string filePath)
        {
            var json = JsonUtility.ToJson(preset, true);
            File.WriteAllText(filePath, json);
            Debug.Log($"[PresetManager] 프리셋 '{preset.Name}' 내보내기 완료: {filePath}");
        }

        public static BrightnessPreset ImportPreset(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogError($"[PresetManager] 파일을 찾을 수 없음: {filePath}");
                return null;
            }

            var json = File.ReadAllText(filePath);
            var preset = JsonUtility.FromJson<BrightnessPreset>(json);

            if (preset != null)
            {
                SavePreset(preset);
                Debug.Log($"[PresetManager] 프리셋 '{preset.Name}' 가져오기 완료");
            }

            return preset;
        }

        public static void CreateDefaultPresets()
        {
            LoadIfNeeded();

            if (_cache.Count > 0) return;

            var defaultPresets = new List<BrightnessPreset>
            {
                new BrightnessPreset("기본값")
                {
                    Description = "lilToon 기본 설정",
                    MinLightValue = 0.05f,
                    MaxLightValue = 1.0f,
                    BackLightValue = 0.35f,
                    ShadowStrengthValue = 1.0f,
                    Shadow1stColorValue = new Color32(0xD3, 0xEC, 0xFF, 0xFF),
                    Shadow1stBorderValue = 0.5f,
                    Shadow1stBlurValue = 0.17f
                },
                new BrightnessPreset("밝은 환경")
                {
                    Description = "밝은 월드용 설정",
                    MinLightValue = 0.1f,
                    MaxLightValue = 1.2f,
                    BackLightValue = 0.2f,
                    ShadowStrengthValue = 0.7f,
                    Shadow1stColorValue = new Color32(0xE8, 0xF4, 0xFF, 0xFF),
                    Shadow1stBorderValue = 0.6f,
                    Shadow1stBlurValue = 0.25f
                },
                new BrightnessPreset("어두운 환경")
                {
                    Description = "어두운 월드/클럽용 설정",
                    MinLightValue = 0.15f,
                    MaxLightValue = 0.9f,
                    BackLightValue = 0.5f,
                    ShadowStrengthValue = 0.5f,
                    Shadow1stColorValue = new Color32(0xC0, 0xD8, 0xF0, 0xFF),
                    Shadow1stBorderValue = 0.4f,
                    Shadow1stBlurValue = 0.3f
                },
                new BrightnessPreset("소프트 그림자")
                {
                    Description = "부드러운 그림자 설정",
                    MinLightValue = 0.08f,
                    MaxLightValue = 1.0f,
                    BackLightValue = 0.3f,
                    ShadowStrengthValue = 0.8f,
                    Shadow1stColorValue = new Color32(0xE0, 0xEE, 0xFF, 0xFF),
                    Shadow1stBorderValue = 0.45f,
                    Shadow1stBlurValue = 0.4f,
                    Shadow2ndBlurValue = 0.8f
                }
            };

            foreach (var preset in defaultPresets)
            {
                SavePreset(preset);
            }

            Debug.Log("[PresetManager] 기본 프리셋 생성됨");
        }

        private static void LoadIfNeeded()
        {
            if (_cache != null) return;

            _cache = new List<BrightnessPreset>();
            EnsurePresetFolder();

            var files = Directory.GetFiles(PRESET_FOLDER, "*" + PRESET_EXTENSION);
            foreach (var file in files)
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var preset = JsonUtility.FromJson<BrightnessPreset>(json);
                    if (preset != null)
                    {
                        _cache.Add(preset);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"[PresetManager] 프리셋 로드 실패 ({file}): {e.Message}");
                }
            }

            _cache = _cache.OrderBy(p => p.Name).ToList();
        }

        private static string GetPresetFilePath(string presetName)
        {
            var safeName = string.Join("_", presetName.Split(Path.GetInvalidFileNameChars()));
            return Path.Combine(PRESET_FOLDER, safeName + PRESET_EXTENSION);
        }

        private static void EnsurePresetFolder()
        {
            if (!Directory.Exists(PRESET_FOLDER))
            {
                Directory.CreateDirectory(PRESET_FOLDER);
            }
        }

        public static void RefreshCache()
        {
            _cache = null;
            LoadIfNeeded();
        }
    }
}