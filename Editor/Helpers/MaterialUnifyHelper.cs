using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Brightness.Utility
{
    public static class MaterialUnifyHelper
    {
        #region Property Handler Architecture
        private abstract class PropertyHandler
        {
            public string PropName { get; }
            protected PropertyHandler(string propName) { PropName = propName; }

            public abstract void Apply(Material mat, UnifySettings settings);
            public abstract void Read(Material mat, UnifySettings settings);
            public abstract bool IsDifferent(Material mat, UnifySettings settings, MaterialPropertyInfo info);
        }

        private class FloatHandler : PropertyHandler
        {
            private readonly Func<UnifySettings, bool> _unifyFlag;
            private readonly Func<UnifySettings, float> _unifyValue;

            public FloatHandler(string propName, Func<UnifySettings, bool> unifyFlag, Func<UnifySettings, float> unifyValue) : base(propName)
            {
                _unifyFlag = unifyFlag;
                _unifyValue = unifyValue;
            }

            public override void Apply(Material mat, UnifySettings s) { if (_unifyFlag(s) && mat.HasProperty(PropName)) mat.SetFloat(PropName, _unifyValue(s)); }
            public override void Read(Material mat, UnifySettings s) { if (mat.HasProperty(PropName)) s.Values[PropName] = mat.GetFloat(PropName); }
            public override bool IsDifferent(Material mat, UnifySettings s, MaterialPropertyInfo info)
            {
                if (!_unifyFlag(s) || !mat.HasProperty(PropName)) return false;
                float matValue = mat.GetFloat(PropName);
                info.Values[PropName] = matValue;
                return !Mathf.Approximately(matValue, _unifyValue(s));
            }
        }

        private class ColorHandler : PropertyHandler
        {
            private readonly Func<UnifySettings, bool> _unifyFlag;
            private readonly Func<UnifySettings, Color> _unifyValue;
            private readonly Func<UnifySettings, bool> _unifyAlphaFlag;
            private readonly Func<UnifySettings, float> _unifyAlphaValue;

            public ColorHandler(string propName, Func<UnifySettings, bool> unifyFlag, Func<UnifySettings, Color> unifyValue, Func<UnifySettings, bool> unifyAlphaFlag = null, Func<UnifySettings, float> unifyAlphaValue = null) : base(propName)
            {
                _unifyFlag = unifyFlag;
                _unifyValue = unifyValue;
                _unifyAlphaFlag = unifyAlphaFlag;
                _unifyAlphaValue = unifyAlphaValue;
            }

            public override void Apply(Material mat, UnifySettings s)
            {
                if (!mat.HasProperty(PropName)) return;
                bool unifyColor = _unifyFlag(s);
                bool unifyAlpha = _unifyAlphaFlag != null && _unifyAlphaFlag(s);
                if (!unifyColor && !unifyAlpha) return;

                Color c = mat.GetColor(PropName);
                if (unifyColor) c = new Color(_unifyValue(s).r, _unifyValue(s).g, _unifyValue(s).b, c.a);
                if (unifyAlpha) c.a = _unifyAlphaValue(s);
                mat.SetColor(PropName, c);
            }
            public override void Read(Material mat, UnifySettings s) 
            { 
                if (!mat.HasProperty(PropName)) return;
                s.Values[PropName] = mat.GetColor(PropName); 
                if(_unifyAlphaValue != null) s.Values[PropName + "_alpha"] = mat.GetColor(PropName).a;
            }
            public override bool IsDifferent(Material mat, UnifySettings s, MaterialPropertyInfo info)
            {
                if (!mat.HasProperty(PropName)) return false;
                Color matValue = mat.GetColor(PropName);
                info.Values[PropName] = matValue;
                
                bool colorDiff = _unifyFlag(s) && (Color)(_unifyValue(s)) != matValue;
                bool alphaDiff = _unifyAlphaFlag != null && _unifyAlphaFlag(s) && !Mathf.Approximately(_unifyAlphaValue(s), matValue.a);
                return colorDiff || alphaDiff;
            }
        }

        private static readonly List<PropertyHandler> _allProperties = new List<PropertyHandler>
        {
            new FloatHandler(BrightnessConstants.ShaderProperties.LIGHT_MIN_LIMIT, s => s.UnifyMinLight, s => s.MinLightValue),
            new FloatHandler(BrightnessConstants.ShaderProperties.LIGHT_MAX_LIMIT, s => s.UnifyMaxLight, s => s.MaxLightValue),
            new FloatHandler(BrightnessConstants.ShaderProperties.BACKLIGHT_BORDER, s => s.UnifyBackLight, s => s.BackLightValue),
            new FloatHandler(BrightnessConstants.ShaderProperties.SHADOW_STRENGTH, s => s.UnifyShadowStrength, s => s.ShadowStrengthValue),
            new FloatHandler(BrightnessConstants.ShaderProperties.SHADOW_STRENGTH_MASK_LOD, s => s.UnifyShadowStrengthLOD, s => s.ShadowStrengthLODValue),
            new ColorHandler(BrightnessConstants.ShaderProperties.SHADOW_COLOR, s => s.UnifyShadow1stColor, s => s.Shadow1stColorValue),
            new FloatHandler(BrightnessConstants.ShaderProperties.SHADOW_BORDER, s => s.UnifyShadow1stBorder, s => s.Shadow1stBorderValue),
            new FloatHandler(BrightnessConstants.ShaderProperties.SHADOW_BLUR, s => s.UnifyShadow1stBlur, s => s.Shadow1stBlurValue),
            new FloatHandler(BrightnessConstants.ShaderProperties.SHADOW_NORMAL_STRENGTH, s => s.UnifyShadow1stNormalStrength, s => s.Shadow1stNormalStrengthValue),
            new FloatHandler(BrightnessConstants.ShaderProperties.SHADOW_RECEIVE, s => s.UnifyShadow1stReceive, s => s.Shadow1stReceiveValue),
            new ColorHandler(BrightnessConstants.ShaderProperties.SHADOW_2ND_COLOR, s => s.UnifyShadow2ndColor, s => s.Shadow2ndColorValue, s => s.UnifyShadow2ndAlpha, s => s.Shadow2ndAlphaValue),
            new FloatHandler(BrightnessConstants.ShaderProperties.SHADOW_2ND_BORDER, s => s.UnifyShadow2ndBorder, s => s.Shadow2ndBorderValue),
            new FloatHandler(BrightnessConstants.ShaderProperties.SHADOW_2ND_BLUR, s => s.UnifyShadow2ndBlur, s => s.Shadow2ndBlurValue),
            new FloatHandler(BrightnessConstants.ShaderProperties.SHADOW_2ND_NORMAL_STRENGTH, s => s.UnifyShadow2ndNormalStrength, s => s.Shadow2ndNormalStrengthValue),
            new FloatHandler(BrightnessConstants.ShaderProperties.SHADOW_2ND_RECEIVE, s => s.UnifyShadow2ndReceive, s => s.Shadow2ndReceiveValue),
            new ColorHandler(BrightnessConstants.ShaderProperties.SHADOW_3RD_COLOR, s => s.UnifyShadow3rdColor, s => s.Shadow3rdColorValue, s => s.UnifyShadow3rdAlpha, s => s.Shadow3rdAlphaValue),
            new ColorHandler(BrightnessConstants.ShaderProperties.SHADOW_BORDER_COLOR, s => s.UnifyShadowBorderColor, s => s.ShadowBorderColorValue),
            new FloatHandler(BrightnessConstants.ShaderProperties.SHADOW_BORDER_RANGE, s => s.UnifyShadowBorderRange, s => s.ShadowBorderRangeValue),
            new FloatHandler(BrightnessConstants.ShaderProperties.SHADOW_MAIN_STRENGTH, s => s.UnifyShadowMainStrength, s => s.ShadowMainStrengthValue),
            new FloatHandler(BrightnessConstants.ShaderProperties.SHADOW_ENV_STRENGTH, s => s.UnifyShadowEnvStrength, s => s.ShadowEnvStrengthValue),
            new FloatHandler(BrightnessConstants.ShaderProperties.SHADOW_CASTER_BIAS, s => s.UnifyShadowCasterBias, s => s.ShadowCasterBiasValue),
            new FloatHandler(BrightnessConstants.ShaderProperties.SHADOW_BLUR_MASK_LOD, s => s.UnifyShadowBlurMaskLOD, s => s.ShadowBlurMaskLODValue)
        };
        #endregion

        private static IEnumerable<Material> GetLilToonMaterials(GameObject avatar)
        {
            if (avatar == null) yield break;
            
            var processedMaterials = new HashSet<Material>();
            var renderers = avatar.GetComponentsInChildren<Renderer>(true);

            foreach (var renderer in renderers)
            {
                if (renderer.sharedMaterials == null) continue;
                foreach (var material in renderer.sharedMaterials)
                {
                    if (material != null && material.shader != null &&
                        material.shader.name.Contains(BrightnessConstants.SHADER_SHORT_NAME) &&
                        processedMaterials.Add(material))
                    {
                        yield return material;
                    }
                }
            }
        }

        public static void UnifyMaterialProperties(GameObject avatar, UnifySettings settings, HashSet<Material> excludeMaterials = null)
        {
            int count = 0;
            foreach (var material in GetLilToonMaterials(avatar))
            {
                if (excludeMaterials != null && excludeMaterials.Contains(material)) continue;

                foreach (var handler in _allProperties) handler.Apply(material, settings);
                
                EditorUtility.SetDirty(material);
                count++;
            }

            if (count > 0)
            {
                AssetDatabase.SaveAssets();
                Debug.Log($"[MaterialUnifyHelper] {count}개 마테리얼에 속성값을 통일했습니다.");
            }
        }

        public static UnifySettings ReadCurrentValues(GameObject avatar)
        {
            var settings = new UnifySettings();
            var firstMaterial = GetLilToonMaterials(avatar).FirstOrDefault();

            if (firstMaterial != null)
            {
                foreach (var handler in _allProperties) handler.Read(firstMaterial, settings);
            }

            return settings;
        }

        public static List<MaterialPropertyInfo> FindDifferentMaterials(GameObject avatar, UnifySettings settings, HashSet<Material> excludeMaterials = null)
        {
            var result = new List<MaterialPropertyInfo>();
            foreach (var material in GetLilToonMaterials(avatar))
            {
                if (excludeMaterials != null && excludeMaterials.Contains(material)) continue;

                var info = new MaterialPropertyInfo { MaterialName = material.name, Material = material };
                bool isDifferent = false;

                foreach (var handler in _allProperties)
                {
                    if (handler.IsDifferent(material, settings, info))
                    {
                        isDifferent = true;
                    }
                }

                if (isDifferent)
                {
                    result.Add(info);
                }
            }
            return result;
        }
    }

    public class UnifySettings
    {
        public Dictionary<string, object> Values = new Dictionary<string, object>();
        
        public bool UnifyMinLight = true;
        public bool UnifyMaxLight = true;
        public bool UnifyBackLight = true;
        public bool UnifyShadowStrength = true;
        public bool UnifyShadowStrengthLOD = true;
        public bool UnifyShadow1stColor = true;
        public bool UnifyShadow1stBorder = true;
        public bool UnifyShadow1stBlur = true;
        public bool UnifyShadow1stNormalStrength = true;
        public bool UnifyShadow1stReceive = true;
        public bool UnifyShadow2ndColor = true;
        public bool UnifyShadow2ndAlpha = true;
        public bool UnifyShadow2ndBorder = true;
        public bool UnifyShadow2ndBlur = true;
        public bool UnifyShadow2ndNormalStrength = true;
        public bool UnifyShadow2ndReceive = true;
        public bool UnifyShadow3rdColor = true;
        public bool UnifyShadow3rdAlpha = true;
        public bool UnifyShadowBorderColor = true;
        public bool UnifyShadowBorderRange = true;
        public bool UnifyShadowMainStrength = true;
        public bool UnifyShadowEnvStrength = true;
        public bool UnifyShadowCasterBias = true;
        public bool UnifyShadowBlurMaskLOD = true;

        public float MinLightValue = 0.05f;
        public float MaxLightValue = 1.0f;
        public float BackLightValue = 0.35f;
        public float ShadowStrengthValue = 1.0f;
        public float ShadowStrengthLODValue = 0.0f;
        public Color Shadow1stColorValue = new Color32(0xD3, 0xEC, 0xFF, 0xFF);
        public float Shadow1stBorderValue = 0.5f;
        public float Shadow1stBlurValue = 0.17f;
        public float Shadow1stNormalStrengthValue = 1.0f;
        public float Shadow1stReceiveValue = 1.0f;
        public Color Shadow2ndColorValue = new Color32(0xCE, 0xE9, 0xFF, 0xFF);
        public float Shadow2ndAlphaValue = 1.0f;
        public float Shadow2ndBorderValue = 0.45f;
        public float Shadow2ndBlurValue = 0.7f;
        public float Shadow2ndNormalStrengthValue = 1.0f;
        public float Shadow2ndReceiveValue = 0.0f;
        public Color Shadow3rdColorValue = new Color32(0x00, 0x00, 0x00, 0x00);
        public float Shadow3rdAlphaValue = 0.0f;
        public Color ShadowBorderColorValue = new Color32(0xD2, 0xDD, 0xF7, 0xFF);
        public float ShadowBorderRangeValue = 0.0f;
        public float ShadowMainStrengthValue = 0.0f;
        public float ShadowEnvStrengthValue = 0.0f;
        public float ShadowCasterBiasValue = 0.0f;
        public float ShadowBlurMaskLODValue = 0.0f;
    }
    
    public class MaterialPropertyInfo
    {
        public string MaterialName;
        public Material Material;
        public Dictionary<string, object> Values = new Dictionary<string, object>();
    }
}

