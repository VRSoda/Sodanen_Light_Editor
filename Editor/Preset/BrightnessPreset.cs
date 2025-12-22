using System;
using System.Collections.Generic;
using UnityEngine;

namespace Brightness.Utility
{
    [Serializable]
    public class CustomMaterialPresetEntry
    {
        public string MaterialPath = "";
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

        public CustomMaterialPresetEntry() { }

        public CustomMaterialPresetEntry(CustomMaterialShadowEntry entry)
        {
            if (entry.Material != null)
            {
                MaterialPath = UnityEditor.AssetDatabase.GetAssetPath(entry.Material);
            }
            ShadowStrength = entry.ShadowStrength;
            Shadow1stColor = entry.Shadow1stColor;
            Shadow1stBorder = entry.Shadow1stBorder;
            Shadow1stBlur = entry.Shadow1stBlur;
            Shadow2ndColor = entry.Shadow2ndColor;
            Shadow2ndAlpha = entry.Shadow2ndAlpha;
            Shadow2ndBorder = entry.Shadow2ndBorder;
            Shadow2ndBlur = entry.Shadow2ndBlur;
            Shadow3rdColor = entry.Shadow3rdColor;
            Shadow3rdAlpha = entry.Shadow3rdAlpha;
        }

        public CustomMaterialShadowEntry ToEntry()
        {
            var entry = new CustomMaterialShadowEntry
            {
                ShadowStrength = ShadowStrength,
                Shadow1stColor = Shadow1stColor,
                Shadow1stBorder = Shadow1stBorder,
                Shadow1stBlur = Shadow1stBlur,
                Shadow2ndColor = Shadow2ndColor,
                Shadow2ndAlpha = Shadow2ndAlpha,
                Shadow2ndBorder = Shadow2ndBorder,
                Shadow2ndBlur = Shadow2ndBlur,
                Shadow3rdColor = Shadow3rdColor,
                Shadow3rdAlpha = Shadow3rdAlpha
            };

            if (!string.IsNullOrEmpty(MaterialPath))
            {
                entry.Material = UnityEditor.AssetDatabase.LoadAssetAtPath<Material>(MaterialPath);
            }

            return entry;
        }
    }

    [Serializable]
    public class BrightnessPreset
    {
        public string Name = "New Preset";
        public string Description = "";

        // 개별 마테리얼 설정
        public List<CustomMaterialPresetEntry> CustomMaterials = new List<CustomMaterialPresetEntry>();

        // Light Settings
        public float MinLightValue = 0.05f;
        public float MaxLightValue = 1.0f;
        public float BackLightValue = 0.35f;

        // Shadow Master
        public float ShadowStrengthValue = 1.0f;
        public float ShadowStrengthLODValue = 0.0f;

        // Shadow 1st
        public Color Shadow1stColorValue = new Color32(0xD3, 0xEC, 0xFF, 0xFF);
        public float Shadow1stBorderValue = 0.5f;
        public float Shadow1stBlurValue = 0.17f;
        public float Shadow1stNormalStrengthValue = 1.0f;
        public float Shadow1stReceiveValue = 1.0f;

        // Shadow 2nd
        public Color Shadow2ndColorValue = new Color32(0xCE, 0xE9, 0xFF, 0xFF);
        public float Shadow2ndAlphaValue = 1.0f;
        public float Shadow2ndBorderValue = 0.45f;
        public float Shadow2ndBlurValue = 0.7f;
        public float Shadow2ndNormalStrengthValue = 1.0f;
        public float Shadow2ndReceiveValue = 0.0f;

        // Shadow 3rd
        public Color Shadow3rdColorValue = new Color32(0x00, 0x00, 0x00, 0x00);
        public float Shadow3rdAlphaValue = 0.0f;

        // Shadow Border
        public Color ShadowBorderColorValue = new Color32(0xD2, 0xDD, 0xF7, 0xFF);
        public float ShadowBorderRangeValue = 0.0f;

        // Shadow Additional
        public float ShadowMainStrengthValue = 0.0f;
        public float ShadowEnvStrengthValue = 0.0f;
        public float ShadowCasterBiasValue = 0.0f;
        public float ShadowBlurMaskLODValue = 0.0f;

        public BrightnessPreset() { }

        public BrightnessPreset(string name)
        {
            Name = name;
        }

        public void CopyFrom(UnifySettings settings, List<CustomMaterialShadowEntry> customEntries = null)
        {
            MinLightValue = settings.MinLightValue;
            MaxLightValue = settings.MaxLightValue;
            BackLightValue = settings.BackLightValue;
            ShadowStrengthValue = settings.ShadowStrengthValue;
            ShadowStrengthLODValue = settings.ShadowStrengthLODValue;
            Shadow1stColorValue = settings.Shadow1stColorValue;
            Shadow1stBorderValue = settings.Shadow1stBorderValue;
            Shadow1stBlurValue = settings.Shadow1stBlurValue;
            Shadow1stNormalStrengthValue = settings.Shadow1stNormalStrengthValue;
            Shadow1stReceiveValue = settings.Shadow1stReceiveValue;
            Shadow2ndColorValue = settings.Shadow2ndColorValue;
            Shadow2ndAlphaValue = settings.Shadow2ndAlphaValue;
            Shadow2ndBorderValue = settings.Shadow2ndBorderValue;
            Shadow2ndBlurValue = settings.Shadow2ndBlurValue;
            Shadow2ndNormalStrengthValue = settings.Shadow2ndNormalStrengthValue;
            Shadow2ndReceiveValue = settings.Shadow2ndReceiveValue;
            Shadow3rdColorValue = settings.Shadow3rdColorValue;
            Shadow3rdAlphaValue = settings.Shadow3rdAlphaValue;
            ShadowBorderColorValue = settings.ShadowBorderColorValue;
            ShadowBorderRangeValue = settings.ShadowBorderRangeValue;
            ShadowMainStrengthValue = settings.ShadowMainStrengthValue;
            ShadowEnvStrengthValue = settings.ShadowEnvStrengthValue;
            ShadowCasterBiasValue = settings.ShadowCasterBiasValue;
            ShadowBlurMaskLODValue = settings.ShadowBlurMaskLODValue;

            // 개별 마테리얼 설정 저장
            CustomMaterials.Clear();
            if (customEntries != null)
            {
                foreach (var entry in customEntries)
                {
                    if (entry.Material != null)
                    {
                        CustomMaterials.Add(new CustomMaterialPresetEntry(entry));
                    }
                }
            }
        }

        public void ApplyTo(UnifySettings settings, List<CustomMaterialShadowEntry> customEntries = null)
        {
            settings.MinLightValue = MinLightValue;
            settings.MaxLightValue = MaxLightValue;
            settings.BackLightValue = BackLightValue;
            settings.ShadowStrengthValue = ShadowStrengthValue;
            settings.ShadowStrengthLODValue = ShadowStrengthLODValue;
            settings.Shadow1stColorValue = Shadow1stColorValue;
            settings.Shadow1stBorderValue = Shadow1stBorderValue;
            settings.Shadow1stBlurValue = Shadow1stBlurValue;
            settings.Shadow1stNormalStrengthValue = Shadow1stNormalStrengthValue;
            settings.Shadow1stReceiveValue = Shadow1stReceiveValue;
            settings.Shadow2ndColorValue = Shadow2ndColorValue;
            settings.Shadow2ndAlphaValue = Shadow2ndAlphaValue;
            settings.Shadow2ndBorderValue = Shadow2ndBorderValue;
            settings.Shadow2ndBlurValue = Shadow2ndBlurValue;
            settings.Shadow2ndNormalStrengthValue = Shadow2ndNormalStrengthValue;
            settings.Shadow2ndReceiveValue = Shadow2ndReceiveValue;
            settings.Shadow3rdColorValue = Shadow3rdColorValue;
            settings.Shadow3rdAlphaValue = Shadow3rdAlphaValue;
            settings.ShadowBorderColorValue = ShadowBorderColorValue;
            settings.ShadowBorderRangeValue = ShadowBorderRangeValue;
            settings.ShadowMainStrengthValue = ShadowMainStrengthValue;
            settings.ShadowEnvStrengthValue = ShadowEnvStrengthValue;
            settings.ShadowCasterBiasValue = ShadowCasterBiasValue;
            settings.ShadowBlurMaskLODValue = ShadowBlurMaskLODValue;

            // 개별 마테리얼 설정 적용
            if (customEntries != null)
            {
                customEntries.Clear();
                foreach (var presetEntry in CustomMaterials)
                {
                    var entry = presetEntry.ToEntry();
                    if (entry.Material != null)
                    {
                        customEntries.Add(entry);
                    }
                }
            }
        }
    }

}