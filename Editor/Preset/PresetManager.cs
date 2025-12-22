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
        private const string PRESET_EXTENSION = ".json";

        private static string GetPresetFolder()
        {
            // 패키지 내부 Presets 폴더 경로 찾기
            var guids = AssetDatabase.FindAssets("t:Folder Presets");
            foreach (var guid in guids)
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                if (path.Contains("com.sodanen.sodanenlighteditor") || path.Contains("SodanenLightEditor"))
                {
                    return path;
                }
            }

            // Packages 폴더에서 찾기
            var packagePath = "Packages/com.sodanen.sodanenlighteditor/Presets";
            if (Directory.Exists(packagePath))
            {
                return packagePath;
            }

            return "Assets/Presets";
        }

        private static List<BrightnessPreset> _cache;

        public static List<BrightnessPreset> GetAllPresets()
        {
            LoadIfNeeded();
            return _cache;
        }

        public static void SavePreset(BrightnessPreset preset)
        {
            LoadIfNeeded();
            var presetFolder = GetPresetFolder();
            EnsurePresetFolder(presetFolder);

            var filePath = GetPresetFilePath(preset.Name, presetFolder);
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
            var presetFolder = GetPresetFolder();

            var filePath = GetPresetFilePath(presetName, presetFolder);
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
            var presetFolder = GetPresetFolder();

            var preset = _cache.Find(p => p.Name == oldName);
            if (preset == null) return;

            var oldPath = GetPresetFilePath(oldName, presetFolder);
            var newPath = GetPresetFilePath(newName, presetFolder);

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

        private static void LoadIfNeeded()
        {
            if (_cache != null) return;

            _cache = new List<BrightnessPreset>();
            var presetFolder = GetPresetFolder();
            EnsurePresetFolder(presetFolder);

            var files = Directory.GetFiles(presetFolder, "*" + PRESET_EXTENSION);
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

        private static string GetPresetFilePath(string presetName, string presetFolder)
        {
            var safeName = string.Join("_", presetName.Split(Path.GetInvalidFileNameChars()));
            return Path.Combine(presetFolder, safeName + PRESET_EXTENSION);
        }

        private static void EnsurePresetFolder(string presetFolder)
        {
            if (!Directory.Exists(presetFolder))
            {
                Directory.CreateDirectory(presetFolder);
            }
        }

        public static void RefreshCache()
        {
            _cache = null;
            LoadIfNeeded();
        }

        public static void CreateDefaultPresets()
        {
            var defaultPreset = new BrightnessPreset("기본 설정")
            {
                Description = "lilToon 기본 설정값"
            };
            SavePreset(defaultPreset);
        }
    }
}