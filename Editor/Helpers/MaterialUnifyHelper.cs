using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Brightness.Utility
{
    public static class MaterialUnifyHelper
    {
        #region Non-Destructive Material Duplication

        /// <summary>
        /// 비파괴적 마테리얼 복제 결과
        /// </summary>
        public class DuplicateResult
        {
            public GameObject DuplicatedAvatar;
            public string MaterialFolderPath;
            public int MaterialCount;
        }

        /// <summary>
        /// 아바타를 복제하고, 복제된 아바타에 복제된 마테리얼을 적용합니다.
        /// 원본 아바타와 원본 마테리얼은 변경되지 않습니다.
        /// </summary>
        /// <param name="avatar">대상 아바타</param>
        /// <param name="applyUnifySettings">복제된 마테리얼에 통일 설정을 적용할지 여부</param>
        /// <param name="settings">통일 설정 (applyUnifySettings가 true일 때 사용)</param>
        /// <param name="excludeMaterials">제외할 마테리얼 목록</param>
        /// <returns>복제 결과 (복제된 아바타, 마테리얼 폴더 경로, 마테리얼 개수)</returns>
        public static DuplicateResult DuplicateMaterialsNonDestructive(
            GameObject avatar,
            bool applyUnifySettings = false,
            UnifySettings settings = null,
            HashSet<Material> excludeMaterials = null)
        {
            if (avatar == null)
            {
                Debug.LogError("[MaterialUnifyHelper] 아바타가 null입니다.");
                return null;
            }

            // 1. 아바타 복제
            GameObject duplicatedAvatar = UnityEngine.Object.Instantiate(avatar);
            duplicatedAvatar.name = avatar.name + "_Copy";
            duplicatedAvatar.transform.SetParent(avatar.transform.parent);
            duplicatedAvatar.transform.SetSiblingIndex(avatar.transform.GetSiblingIndex() + 1);

            // 원본 아바타 비활성화
            avatar.SetActive(false);

            Undo.RegisterCreatedObjectUndo(duplicatedAvatar, "Duplicate Avatar Non-Destructive");

            // 2. 마테리얼 폴더 생성
            string folderPath = CreateMaterialFolder(avatar.name);
            if (string.IsNullOrEmpty(folderPath))
            {
                Debug.LogError("[MaterialUnifyHelper] 폴더 생성에 실패했습니다.");
                UnityEngine.Object.DestroyImmediate(duplicatedAvatar);
                avatar.SetActive(true);
                return null;
            }

            // 3. 원본 마테리얼 -> 복제 마테리얼 매핑 (원본 아바타 기준으로 수집)
            var materialMapping = new Dictionary<Material, Material>();
            var originalRenderers = avatar.GetComponentsInChildren<Renderer>(true);

            foreach (var renderer in originalRenderers)
            {
                if (renderer.sharedMaterials == null) continue;

                foreach (var material in renderer.sharedMaterials)
                {
                    if (material == null || material.shader == null) continue;
                    if (!material.shader.name.Contains(BrightnessConstants.SHADER_SHORT_NAME)) continue;
                    if (materialMapping.ContainsKey(material)) continue;
                    if (excludeMaterials != null && excludeMaterials.Contains(material)) continue;

                    // 마테리얼 복제
                    Material duplicatedMaterial = DuplicateMaterial(material, folderPath);
                    if (duplicatedMaterial != null)
                    {
                        materialMapping[material] = duplicatedMaterial;

                        // 통일 설정 적용
                        if (applyUnifySettings && settings != null)
                        {
                            foreach (var handler in _allProperties)
                            {
                                handler.Apply(duplicatedMaterial, settings);
                            }
                            EditorUtility.SetDirty(duplicatedMaterial);
                        }
                    }
                }
            }

            // 4. 복제된 아바타의 Renderer에 복제된 마테리얼 적용
            var duplicatedRenderers = duplicatedAvatar.GetComponentsInChildren<Renderer>(true);

            foreach (var renderer in duplicatedRenderers)
            {
                if (renderer.sharedMaterials == null) continue;

                var materials = renderer.sharedMaterials;
                bool changed = false;

                for (int i = 0; i < materials.Length; i++)
                {
                    if (materials[i] != null && materialMapping.TryGetValue(materials[i], out var duplicated))
                    {
                        materials[i] = duplicated;
                        changed = true;
                    }
                }

                if (changed)
                {
                    renderer.sharedMaterials = materials;
                }
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"[MaterialUnifyHelper] 아바타 '{duplicatedAvatar.name}' 생성, {materialMapping.Count}개 마테리얼을 '{folderPath}'에 복제했습니다.");

            return new DuplicateResult
            {
                DuplicatedAvatar = duplicatedAvatar,
                MaterialFolderPath = folderPath,
                MaterialCount = materialMapping.Count
            };
        }

        /// <summary>
        /// 마테리얼 저장용 폴더 생성
        /// </summary>
        private static string CreateMaterialFolder(string avatarName)
        {
            string basePath = BrightnessConstants.CREATE_PATH;

            // 기본 폴더가 없으면 생성
            if (!AssetDatabase.IsValidFolder(basePath.TrimEnd('/')))
            {
                string parentPath = Path.GetDirectoryName(basePath.TrimEnd('/'));
                string folderName = Path.GetFileName(basePath.TrimEnd('/'));
                AssetDatabase.CreateFolder(parentPath, folderName);
            }

            // 아바타별 마테리얼 폴더 생성
            string safeName = SanitizeFileName(avatarName);
            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string folderName2 = $"{safeName}_Materials_{timestamp}";
            string fullPath = $"{basePath}{folderName2}";

            // 폴더 생성
            string guid = AssetDatabase.CreateFolder(basePath.TrimEnd('/'), folderName2);
            if (string.IsNullOrEmpty(guid))
            {
                Debug.LogError($"[MaterialUnifyHelper] 폴더 생성 실패: {fullPath}");
                return null;
            }

            return fullPath;
        }

        /// <summary>
        /// 마테리얼 복제 및 저장
        /// </summary>
        private static Material DuplicateMaterial(Material original, string folderPath)
        {
            if (original == null) return null;

            // 새 마테리얼 생성 (원본 복제)
            Material duplicated = new Material(original);
            duplicated.name = original.name;

            // 고유한 파일 경로 생성
            string assetPath = GetUniqueAssetPath(folderPath, original.name);

            // 에셋으로 저장
            AssetDatabase.CreateAsset(duplicated, assetPath);

            return duplicated;
        }

        /// <summary>
        /// 중복되지 않는 에셋 경로 생성
        /// </summary>
        private static string GetUniqueAssetPath(string folderPath, string materialName)
        {
            string safeName = SanitizeFileName(materialName);
            string basePath = $"{folderPath}/{safeName}.mat";

            if (!File.Exists(basePath))
                return basePath;

            int counter = 1;
            while (File.Exists($"{folderPath}/{safeName}_{counter}.mat"))
            {
                counter++;
            }

            return $"{folderPath}/{safeName}_{counter}.mat";
        }

        /// <summary>
        /// 파일명에서 사용할 수 없는 문자 제거
        /// </summary>
        private static string SanitizeFileName(string name)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars();
            string result = name;
            foreach (char c in invalidChars)
            {
                result = result.Replace(c, '_');
            }
            return result;
        }

        #endregion

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

            public override void Apply(Material mat, UnifySettings s)
            {
                if (_unifyFlag(s) && mat.HasProperty(PropName))
                {
                    float valueToApply = _unifyValue(s);
                    Debug.Log($"[MaterialUnify] Applying {PropName} = {valueToApply} to {mat.name}");
                    mat.SetFloat(PropName, valueToApply);
                }
            }
            public override void Read(Material mat, UnifySettings s) { if (mat.HasProperty(PropName)) s.Values[PropName] = mat.GetFloat(PropName); }
            public override bool IsDifferent(Material mat, UnifySettings s, MaterialPropertyInfo info)
            {
                if (!_unifyFlag(s) || !mat.HasProperty(PropName)) return false;
                float matValue = mat.GetFloat(PropName);
                info.Values[PropName] = matValue;
                return !Mathf.Approximately(matValue, _unifyValue(s));
            }
        }

        /// <summary>
        /// lilToon의 BacklightBorder처럼 UI에서 1-value로 표시되는 프로퍼티용 핸들러
        /// </summary>
        private class InvertedFloatHandler : PropertyHandler
        {
            private readonly Func<UnifySettings, bool> _unifyFlag;
            private readonly Func<UnifySettings, float> _unifyValue;

            public InvertedFloatHandler(string propName, Func<UnifySettings, bool> unifyFlag, Func<UnifySettings, float> unifyValue) : base(propName)
            {
                _unifyFlag = unifyFlag;
                _unifyValue = unifyValue;
            }

            public override void Apply(Material mat, UnifySettings s)
            {
                if (_unifyFlag(s) && mat.HasProperty(PropName))
                {
                    // UI 값을 셰이더 값으로 변환 (1 - value)
                    float valueToApply = 1f - _unifyValue(s);
                    Debug.Log($"[MaterialUnify] Applying {PropName} = {valueToApply} (UI: {_unifyValue(s)}) to {mat.name}");
                    mat.SetFloat(PropName, valueToApply);
                }
            }

            public override void Read(Material mat, UnifySettings s)
            {
                // 셰이더 값을 UI 값으로 변환 (1 - value)
                if (mat.HasProperty(PropName))
                    s.Values[PropName] = 1f - mat.GetFloat(PropName);
            }

            public override bool IsDifferent(Material mat, UnifySettings s, MaterialPropertyInfo info)
            {
                if (!_unifyFlag(s) || !mat.HasProperty(PropName)) return false;
                float shaderValue = mat.GetFloat(PropName);
                float uiValue = 1f - shaderValue; // 셰이더 값을 UI 값으로 변환
                info.Values[PropName] = uiValue;
                return !Mathf.Approximately(uiValue, _unifyValue(s));
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
            new InvertedFloatHandler(BrightnessConstants.ShaderProperties.BACKLIGHT_BORDER, s => s.UnifyBackLight, s => s.BackLightValue),
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

