using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Brightness.Utility
{
    public class FeatureToggles
    {
        public bool MinLight { get; set; } = true;
        public bool MaxLight { get; set; } = true;
        public bool BackLight { get; set; } = true;
        public bool Shadow { get; set; } = true;
        public bool ShadowXAngle { get; set; } = true;
        public bool ShadowYAngle { get; set; } = true;

        public bool HasAnyFeatureSelected() =>
            MinLight || MaxLight || BackLight || Shadow || ShadowXAngle || ShadowYAngle;
    }

    public class FeatureMaterialSelections
    {
        public Dictionary<string, bool> MinLight { get; } = new();
        public Dictionary<string, bool> MaxLight { get; } = new();
        public Dictionary<string, bool> BackLight { get; } = new();
        public Dictionary<string, bool> Shadow { get; } = new();
        public Dictionary<string, bool> ShadowX { get; } = new();
        public Dictionary<string, bool> ShadowY { get; } = new();

        public bool ShowMinLight { get; set; }
        public bool ShowMaxLight { get; set; }
        public bool ShowBackLight { get; set; }
        public bool ShowShadow { get; set; }
        public bool ShowShadowX { get; set; }
        public bool ShowShadowY { get; set; }

        public void Clear()
        {
            MinLight.Clear();
            MaxLight.Clear();
            BackLight.Clear();
            Shadow.Clear();
            ShadowX.Clear();
            ShadowY.Clear();
        }

        public void SetAll(IEnumerable<string> paths, bool value)
        {
            foreach (var path in paths)
            {
                MinLight[path] = value;
                MaxLight[path] = value;
                BackLight[path] = value;
                Shadow[path] = value;
                ShadowX[path] = value;
                ShadowY[path] = value;
            }
        }
    }

    public class ShadowUnifyUIState
    {
        public bool ShowShadowMaster { get; set; } = true;
        public bool ShowShadow1st { get; set; } = true;
        public bool ShowShadow2nd { get; set; }
        public bool ShowShadow3rd { get; set; }
        public bool ShowShadowBlurMask { get; set; }
        public bool ShowShadowGroup { get; set; } = true;
    }

    internal class FeatureInfo
    {
        public string Label { get; set; }
        public string Tooltip { get; set; }
        public Func<FeatureToggles, bool> IsEnabled { get; set; }
        public Action<FeatureToggles, bool> SetEnabled { get; set; }
        public Func<FeatureMaterialSelections, bool> IsExpanded { get; set; }
        public Action<FeatureMaterialSelections, bool> SetExpanded { get; set; }
        public Func<FeatureMaterialSelections, Dictionary<string, bool>> GetMaterials { get; set; }
    }

    public class MaterialSelections
    {
        public List<string> MinLight { get; set; } = new();
        public List<string> MaxLight { get; set; } = new();
        public List<string> BackLight { get; set; } = new();
        public List<string> Shadow { get; set; } = new();
        public List<string> ShadowX { get; set; } = new();
        public List<string> ShadowY { get; set; } = new();
    }

    public class CustomMaterialShadowEntry
    {
        public Material Material;
        public bool IsExpanded = true;

        public float ShadowStrength = 0.5f;
        public Color Shadow1stColor = new Color32(0xF9, 0xE5, 0xE7, 0xFF);
        public float Shadow1stBorder = 0.5f;
        public float Shadow1stBlur = 0.1f;
        public Color Shadow2ndColor = new Color32(0xF5, 0xE9, 0xF2, 0xFF);
        public float Shadow2ndAlpha = 1.0f;
        public float Shadow2ndBorder = 0.45f;
        public float Shadow2ndBlur = 0.3f;
        public Color Shadow3rdColor = new Color32(0x00, 0x00, 0x00, 0xFF);
        public float Shadow3rdAlpha = 0.0f;

        public void ReadFromMaterial()
        {
            if (Material == null) return;

            ReadFloat(BrightnessConstants.ShaderProperties.SHADOW_STRENGTH, ref ShadowStrength);
            ReadColor(BrightnessConstants.ShaderProperties.SHADOW_COLOR, ref Shadow1stColor);
            ReadFloat(BrightnessConstants.ShaderProperties.SHADOW_BORDER, ref Shadow1stBorder);
            ReadFloat(BrightnessConstants.ShaderProperties.SHADOW_BLUR, ref Shadow1stBlur);

            if (Material.HasProperty(BrightnessConstants.ShaderProperties.SHADOW_2ND_COLOR))
            {
                var col = Material.GetColor(BrightnessConstants.ShaderProperties.SHADOW_2ND_COLOR);
                Shadow2ndColor = new Color(col.r, col.g, col.b, 1f);
                Shadow2ndAlpha = col.a;
            }
            ReadFloat(BrightnessConstants.ShaderProperties.SHADOW_2ND_BORDER, ref Shadow2ndBorder);
            ReadFloat(BrightnessConstants.ShaderProperties.SHADOW_2ND_BLUR, ref Shadow2ndBlur);

            if (Material.HasProperty(BrightnessConstants.ShaderProperties.SHADOW_3RD_COLOR))
            {
                var col = Material.GetColor(BrightnessConstants.ShaderProperties.SHADOW_3RD_COLOR);
                Shadow3rdColor = new Color(col.r, col.g, col.b, 1f);
                Shadow3rdAlpha = col.a;
            }
        }

        public void ApplyToMaterial()
        {
            if (Material == null) return;

            SetFloat(BrightnessConstants.ShaderProperties.SHADOW_STRENGTH, ShadowStrength);
            SetColor(BrightnessConstants.ShaderProperties.SHADOW_COLOR, Shadow1stColor);
            SetFloat(BrightnessConstants.ShaderProperties.SHADOW_BORDER, Shadow1stBorder);
            SetFloat(BrightnessConstants.ShaderProperties.SHADOW_BLUR, Shadow1stBlur);
            SetColor(BrightnessConstants.ShaderProperties.SHADOW_2ND_COLOR,
                new Color(Shadow2ndColor.r, Shadow2ndColor.g, Shadow2ndColor.b, Shadow2ndAlpha));
            SetFloat(BrightnessConstants.ShaderProperties.SHADOW_2ND_BORDER, Shadow2ndBorder);
            SetFloat(BrightnessConstants.ShaderProperties.SHADOW_2ND_BLUR, Shadow2ndBlur);
            SetColor(BrightnessConstants.ShaderProperties.SHADOW_3RD_COLOR,
                new Color(Shadow3rdColor.r, Shadow3rdColor.g, Shadow3rdColor.b, Shadow3rdAlpha));

            EditorUtility.SetDirty(Material);
        }

        private void ReadFloat(string prop, ref float value)
        {
            if (Material.HasProperty(prop)) value = Material.GetFloat(prop);
        }

        private void ReadColor(string prop, ref Color value)
        {
            if (Material.HasProperty(prop)) value = Material.GetColor(prop);
        }

        private void SetFloat(string prop, float value)
        {
            if (Material.HasProperty(prop)) Material.SetFloat(prop, value);
        }

        private void SetColor(string prop, Color value)
        {
            if (Material.HasProperty(prop)) Material.SetColor(prop, value);
        }
    }

    public class ShadowGroupOverride
    {
        public string GroupName { get; }
        public bool IsExpanded { get; set; } = true;
        public bool ShowMaterialPopup { get; set; }
        public Dictionary<Material, bool> AvailableMaterials { get; } = new();

        public Color Shadow1stColor = new Color32(0xF9, 0xE5, 0xE7, 0xFF);
        public float Shadow1stBorder = 0.5f;
        public float Shadow1stBlur = 0.1f;
        public Color Shadow2ndColor = new Color32(0xF5, 0xE9, 0xF2, 0xFF);
        public float Shadow2ndAlpha = 1.0f;
        public float Shadow2ndBorder = 0.45f;
        public float Shadow2ndBlur = 0.3f;
        public Color Shadow3rdColor = new Color32(0x00, 0x00, 0x00, 0xFF);
        public float Shadow3rdAlpha = 0.0f;

        public ShadowGroupOverride(string name) => GroupName = name;

        public void RefreshAvailableMaterials(GameObject avatar)
        {
            var oldSelections = new Dictionary<Material, bool>(AvailableMaterials);
            AvailableMaterials.Clear();

            if (avatar == null) return;

            var processedMaterials = new HashSet<Material>();
            foreach (var renderer in avatar.GetComponentsInChildren<Renderer>(true))
            {
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material == null) continue;
                    if (!material.shader.name.Contains(BrightnessConstants.SHADER_SHORT_NAME)) continue;
                    if (!processedMaterials.Add(material)) continue;

                    AvailableMaterials[material] = oldSelections.TryGetValue(material, out var selected) && selected;
                }
            }
        }

        public int GetSelectedMaterialCount() => AvailableMaterials.Count(kvp => kvp.Value);

        public IEnumerable<Material> GetSelectedMaterials() =>
            AvailableMaterials.Where(kvp => kvp.Value).Select(kvp => kvp.Key);

        public void SelectAllMaterials(bool selected)
        {
            foreach (var key in AvailableMaterials.Keys.ToList())
            {
                AvailableMaterials[key] = selected;
            }
        }

        public void ApplyToSelectedMaterials()
        {
            var selectedMaterials = GetSelectedMaterials().ToList();

            foreach (var material in selectedMaterials)
            {
                material.SetColor(BrightnessConstants.ShaderProperties.SHADOW_COLOR, Shadow1stColor);
                material.SetFloat(BrightnessConstants.ShaderProperties.SHADOW_BORDER, Shadow1stBorder);
                material.SetFloat(BrightnessConstants.ShaderProperties.SHADOW_BLUR, Shadow1stBlur);

                material.SetColor(BrightnessConstants.ShaderProperties.SHADOW_2ND_COLOR,
                    new Color(Shadow2ndColor.r, Shadow2ndColor.g, Shadow2ndColor.b, Shadow2ndAlpha));
                material.SetFloat(BrightnessConstants.ShaderProperties.SHADOW_2ND_BORDER, Shadow2ndBorder);
                material.SetFloat(BrightnessConstants.ShaderProperties.SHADOW_2ND_BLUR, Shadow2ndBlur);

                material.SetColor(BrightnessConstants.ShaderProperties.SHADOW_3RD_COLOR,
                    new Color(Shadow3rdColor.r, Shadow3rdColor.g, Shadow3rdColor.b, Shadow3rdAlpha));

                EditorUtility.SetDirty(material);
            }

            if (selectedMaterials.Count > 0)
            {
                Debug.Log($"[ShadowGroup] '{GroupName}' 그룹: {selectedMaterials.Count}개 마테리얼에 그림자 설정 적용됨");
            }
        }
    }
}
