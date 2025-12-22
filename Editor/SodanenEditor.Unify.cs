using UnityEditor;
using UnityEngine;

namespace Brightness.Utility
{
    public partial class SodanenEditor
    {
        private Material _sourceMaterial;

        private void DrawUnifySectionContent()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawUnifyHeader();

            if (_showUnifySection)
            {
                GUILayout.Space(5);
                DrawLightSettings();
                GUILayout.Space(5);
                DrawShadowSettings();
                GUILayout.Space(5);
                DrawShadowGroupSection();
                GUILayout.Space(8);
                DrawUnifyButtons();
                DrawMaterialDifferences();

                EditorGUILayout.LabelField("적용 전에 마테리얼 초기값이 동일한지 확인", _infoStyle);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawUnifyHeader()
        {
            _showUnifySection = EditorGUILayout.Foldout(_showUnifySection, "마테리얼 속성 통일", true);
            DrawSourceMaterialSection();
        }

        private void DrawSourceMaterialSection()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("마테리얼에서 가져오기", GUILayout.Width(120));
            var newMaterial = (Material)EditorGUILayout.ObjectField(_sourceMaterial, typeof(Material), false);

            if (newMaterial != _sourceMaterial)
            {
                _sourceMaterial = newMaterial;
                if (_sourceMaterial != null)
                {
                    ApplyMaterialToUnifySettings(_sourceMaterial);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void ApplyMaterialToUnifySettings(Material material)
        {
            if (material == null) return;

            // Light
            if (material.HasProperty(BrightnessConstants.ShaderProperties.LIGHT_MIN_LIMIT))
                _unifySettings.MinLightValue = material.GetFloat(BrightnessConstants.ShaderProperties.LIGHT_MIN_LIMIT);
            if (material.HasProperty(BrightnessConstants.ShaderProperties.LIGHT_MAX_LIMIT))
                _unifySettings.MaxLightValue = material.GetFloat(BrightnessConstants.ShaderProperties.LIGHT_MAX_LIMIT);
            if (material.HasProperty(BrightnessConstants.ShaderProperties.BACKLIGHT_BORDER))
                _unifySettings.BackLightValue = material.GetFloat(BrightnessConstants.ShaderProperties.BACKLIGHT_BORDER);

            // Shadow Master
            if (material.HasProperty(BrightnessConstants.ShaderProperties.SHADOW_STRENGTH))
                _unifySettings.ShadowStrengthValue = material.GetFloat(BrightnessConstants.ShaderProperties.SHADOW_STRENGTH);
            if (material.HasProperty(BrightnessConstants.ShaderProperties.SHADOW_STRENGTH_MASK_LOD))
                _unifySettings.ShadowStrengthLODValue = material.GetFloat(BrightnessConstants.ShaderProperties.SHADOW_STRENGTH_MASK_LOD);

            // Shadow 1st
            if (material.HasProperty(BrightnessConstants.ShaderProperties.SHADOW_COLOR))
                _unifySettings.Shadow1stColorValue = material.GetColor(BrightnessConstants.ShaderProperties.SHADOW_COLOR);
            if (material.HasProperty(BrightnessConstants.ShaderProperties.SHADOW_BORDER))
                _unifySettings.Shadow1stBorderValue = material.GetFloat(BrightnessConstants.ShaderProperties.SHADOW_BORDER);
            if (material.HasProperty(BrightnessConstants.ShaderProperties.SHADOW_BLUR))
                _unifySettings.Shadow1stBlurValue = material.GetFloat(BrightnessConstants.ShaderProperties.SHADOW_BLUR);
            if (material.HasProperty(BrightnessConstants.ShaderProperties.SHADOW_NORMAL_STRENGTH))
                _unifySettings.Shadow1stNormalStrengthValue = material.GetFloat(BrightnessConstants.ShaderProperties.SHADOW_NORMAL_STRENGTH);
            if (material.HasProperty(BrightnessConstants.ShaderProperties.SHADOW_RECEIVE))
                _unifySettings.Shadow1stReceiveValue = material.GetFloat(BrightnessConstants.ShaderProperties.SHADOW_RECEIVE);

            // Shadow 2nd
            if (material.HasProperty(BrightnessConstants.ShaderProperties.SHADOW_2ND_COLOR))
            {
                var col = material.GetColor(BrightnessConstants.ShaderProperties.SHADOW_2ND_COLOR);
                _unifySettings.Shadow2ndColorValue = new Color(col.r, col.g, col.b, 1f);
                _unifySettings.Shadow2ndAlphaValue = col.a;
            }
            if (material.HasProperty(BrightnessConstants.ShaderProperties.SHADOW_2ND_BORDER))
                _unifySettings.Shadow2ndBorderValue = material.GetFloat(BrightnessConstants.ShaderProperties.SHADOW_2ND_BORDER);
            if (material.HasProperty(BrightnessConstants.ShaderProperties.SHADOW_2ND_BLUR))
                _unifySettings.Shadow2ndBlurValue = material.GetFloat(BrightnessConstants.ShaderProperties.SHADOW_2ND_BLUR);
            if (material.HasProperty(BrightnessConstants.ShaderProperties.SHADOW_2ND_NORMAL_STRENGTH))
                _unifySettings.Shadow2ndNormalStrengthValue = material.GetFloat(BrightnessConstants.ShaderProperties.SHADOW_2ND_NORMAL_STRENGTH);
            if (material.HasProperty(BrightnessConstants.ShaderProperties.SHADOW_2ND_RECEIVE))
                _unifySettings.Shadow2ndReceiveValue = material.GetFloat(BrightnessConstants.ShaderProperties.SHADOW_2ND_RECEIVE);

            // Shadow 3rd
            if (material.HasProperty(BrightnessConstants.ShaderProperties.SHADOW_3RD_COLOR))
            {
                var col = material.GetColor(BrightnessConstants.ShaderProperties.SHADOW_3RD_COLOR);
                _unifySettings.Shadow3rdColorValue = new Color(col.r, col.g, col.b, 1f);
                _unifySettings.Shadow3rdAlphaValue = col.a;
            }

            // Shadow Border
            if (material.HasProperty(BrightnessConstants.ShaderProperties.SHADOW_BORDER_COLOR))
                _unifySettings.ShadowBorderColorValue = material.GetColor(BrightnessConstants.ShaderProperties.SHADOW_BORDER_COLOR);
            if (material.HasProperty(BrightnessConstants.ShaderProperties.SHADOW_BORDER_RANGE))
                _unifySettings.ShadowBorderRangeValue = material.GetFloat(BrightnessConstants.ShaderProperties.SHADOW_BORDER_RANGE);
            if (material.HasProperty(BrightnessConstants.ShaderProperties.SHADOW_MAIN_STRENGTH))
                _unifySettings.ShadowMainStrengthValue = material.GetFloat(BrightnessConstants.ShaderProperties.SHADOW_MAIN_STRENGTH);
            if (material.HasProperty(BrightnessConstants.ShaderProperties.SHADOW_ENV_STRENGTH))
                _unifySettings.ShadowEnvStrengthValue = material.GetFloat(BrightnessConstants.ShaderProperties.SHADOW_ENV_STRENGTH);
            if (material.HasProperty(BrightnessConstants.ShaderProperties.SHADOW_CASTER_BIAS))
                _unifySettings.ShadowCasterBiasValue = material.GetFloat(BrightnessConstants.ShaderProperties.SHADOW_CASTER_BIAS);

            // Shadow Blur Mask
            if (material.HasProperty(BrightnessConstants.ShaderProperties.SHADOW_BLUR_MASK_LOD))
                _unifySettings.ShadowBlurMaskLODValue = material.GetFloat(BrightnessConstants.ShaderProperties.SHADOW_BLUR_MASK_LOD);
        }

        private void DrawLightSettings()
        {
            EditorGUILayout.LabelField("Light", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawUnifySlider("Min Light", ref _unifySettings.UnifyMinLight, ref _unifySettings.MinLightValue, 0f, 1f);
            DrawUnifySlider("Max Light", ref _unifySettings.UnifyMaxLight, ref _unifySettings.MaxLightValue, 0f, 10f);
            DrawUnifySlider("Back Light", ref _unifySettings.UnifyBackLight, ref _unifySettings.BackLightValue, 0f, 1f);
            EditorGUILayout.EndVertical();
        }

        private void DrawShadowSettings()
        {
            EditorGUILayout.BeginHorizontal();
            var newEnableGroup = EditorGUILayout.Toggle(_unifyEnableShadowGroup, GUILayout.Width(18));
            _shadowUIState.ShowShadowGroup = EditorGUILayout.Foldout(_shadowUIState.ShowShadowGroup, "그림자", true);
            EditorGUILayout.EndHorizontal();

            if (_shadowUIState.ShowShadowGroup)
            {
                using (new EditorGUI.DisabledScope(!_unifyEnableShadowGroup))
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    DrawShadowMasterSection();
                    GUILayout.Space(3);
                    DrawShadow1stSection();
                    GUILayout.Space(3);
                    DrawShadow2ndSection();
                    GUILayout.Space(3);
                    DrawShadow3rdSection();
                    GUILayout.Space(3);
                    DrawShadowBorderSettings();
                    GUILayout.Space(3);
                    DrawShadowBlurMaskSection();
                    EditorGUILayout.EndVertical();
                }
            }

            if (newEnableGroup != _unifyEnableShadowGroup)
            {
                _unifyEnableShadowGroup = newEnableGroup;
                SetAllShadowUnifyFlags(_unifyEnableShadowGroup);
            }
        }

        private void DrawShadowMasterSection()
        {
            _shadowUIState.ShowShadowMaster = EditorGUILayout.Foldout(_shadowUIState.ShowShadowMaster, "마스크와 강도", true);
            if (!_shadowUIState.ShowShadowMaster) return;

            EditorGUI.indentLevel++;
            DrawShadowSlider("강도", ref _unifySettings.ShadowStrengthValue, 0f, 1f);
            DrawShadowSlider("LOD", ref _unifySettings.ShadowStrengthLODValue, 0f, 1f);
            EditorGUI.indentLevel--;
        }

        private void DrawShadow1stSection()
        {
            _shadowUIState.ShowShadow1st = EditorGUILayout.Foldout(_shadowUIState.ShowShadow1st, "그림자 색 1", true);
            if (!_shadowUIState.ShowShadow1st) return;

            EditorGUI.indentLevel++;
            DrawShadowColor("색상", ref _unifySettings.Shadow1stColorValue);
            DrawShadowSlider("범위", ref _unifySettings.Shadow1stBorderValue, 0f, 1f);
            DrawShadowSlider("흐리게", ref _unifySettings.Shadow1stBlurValue, 0f, 1f);
            DrawShadowSlider("노멀 맵 강도", ref _unifySettings.Shadow1stNormalStrengthValue, 0f, 1f);
            DrawShadowSlider("그림자를받는", ref _unifySettings.Shadow1stReceiveValue, 0f, 1f);
            EditorGUI.indentLevel--;
        }

        private void DrawShadow2ndSection()
        {
            _shadowUIState.ShowShadow2nd = EditorGUILayout.Foldout(_shadowUIState.ShowShadow2nd, "그림자 색 2", true);
            if (!_shadowUIState.ShowShadow2nd) return;

            EditorGUI.indentLevel++;
            DrawShadowColor("색상", ref _unifySettings.Shadow2ndColorValue);
            DrawShadowSlider("투명도", ref _unifySettings.Shadow2ndAlphaValue, 0f, 1f);
            DrawShadowSlider("범위", ref _unifySettings.Shadow2ndBorderValue, 0f, 1f);
            DrawShadowSlider("흐리게", ref _unifySettings.Shadow2ndBlurValue, 0f, 1f);
            DrawShadowSlider("노멀 맵 강도", ref _unifySettings.Shadow2ndNormalStrengthValue, 0f, 1f);
            DrawShadowSlider("그림자를받는", ref _unifySettings.Shadow2ndReceiveValue, 0f, 1f);
            EditorGUI.indentLevel--;
        }

        private void DrawShadow3rdSection()
        {
            _shadowUIState.ShowShadow3rd = EditorGUILayout.Foldout(_shadowUIState.ShowShadow3rd, "그림자 색 3", true);
            if (!_shadowUIState.ShowShadow3rd) return;

            EditorGUI.indentLevel++;
            DrawShadowColor("색상", ref _unifySettings.Shadow3rdColorValue);
            DrawShadowSlider("투명도", ref _unifySettings.Shadow3rdAlphaValue, 0f, 1f);
            EditorGUI.indentLevel--;
        }

        private void DrawShadowBorderSettings()
        {
            DrawShadowColor("경계의 색", ref _unifySettings.ShadowBorderColorValue);
            DrawShadowSlider("경계의 폭", ref _unifySettings.ShadowBorderRangeValue, 0f, 1f);
            GUILayout.Space(3);
            DrawShadowSlider("콘트라스트", ref _unifySettings.ShadowMainStrengthValue, 0f, 1f);
            DrawShadowSlider("그림자 색에 환경 광의 영향", ref _unifySettings.ShadowEnvStrengthValue, 0f, 1f);
            DrawShadowSlider("Shadow Caster Bias", ref _unifySettings.ShadowCasterBiasValue, 0f, 1f);
        }

        private void DrawShadowBlurMaskSection()
        {
            _shadowUIState.ShowShadowBlurMask = EditorGUILayout.Foldout(_shadowUIState.ShowShadowBlurMask, "흐림 효과 마스크", true);
            if (!_shadowUIState.ShowShadowBlurMask) return;

            EditorGUI.indentLevel++;
            DrawShadowSlider("LOD", ref _unifySettings.ShadowBlurMaskLODValue, 0f, 1f);
            EditorGUI.indentLevel--;
        }

        private void DrawUnifyButtons()
        {
            EditorGUILayout.BeginHorizontal();

            GUI.backgroundColor = new Color(1f, 0.7f, 0.3f);
            if (GUILayout.Button("수치 다른 마테리얼 찾기", GUILayout.Height(30)))
            {
                var excludeMaterials = GetExcludedMaterials();
                _materialDifferences = MaterialUnifyHelper.FindDifferentMaterials(_targetAvatar, _unifySettings, excludeMaterials);
                _showDifferences = true;
                if (_materialDifferences.Count == 0)
                {
                    EditorUtility.DisplayDialog("결과", "모든 마테리얼의 수치가 동일합니다.", "확인");
                }
            }

            GUI.backgroundColor = new Color(0.3f, 0.6f, 1f);
            if (GUILayout.Button("마테리얼 속성 통일하기", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog("마테리얼 통일",
                    "선택된 속성값을 모든 lilToon 마테리얼에 적용합니다.\n이 작업은 되돌릴 수 없습니다.\n\n계속하시겠습니까?",
                    "적용", "취소"))
                {
                    var excludeMaterials = GetExcludedMaterials();
                    MaterialUnifyHelper.UnifyMaterialProperties(_targetAvatar, _unifySettings, excludeMaterials);
                    AssetDatabase.SaveAssets();
                    _materialDifferences.Clear();
                    _showDifferences = false;
                    EditorUtility.DisplayDialog("완료", "마테리얼 속성이 통일되었습니다.", "확인");
                }
            }

            GUI.backgroundColor = Color.white;
            EditorGUILayout.EndHorizontal();
        }

        private System.Collections.Generic.HashSet<Material> GetExcludedMaterials()
        {
            var excludeMaterials = new System.Collections.Generic.HashSet<Material>();
            foreach (var group in _shadowGroups)
            {
                foreach (var mat in group.GetSelectedMaterials())
                {
                    excludeMaterials.Add(mat);
                }
            }
            foreach (var entry in _customMaterialEntries)
            {
                if (entry.Material != null)
                {
                    excludeMaterials.Add(entry.Material);
                }
            }
            return excludeMaterials;
        }

        private void DrawMaterialDifferences()
        {
            if (!_showDifferences || _materialDifferences.Count == 0) return;

            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"수치가 다른 마테리얼: {_materialDifferences.Count}개", EditorStyles.boldLabel);
            if (GUILayout.Button("닫기", GUILayout.Width(40))) _showDifferences = false;
            EditorGUILayout.EndHorizontal();

            foreach (var info in _materialDifferences)
            {
                DrawMaterialDifferenceItem(info);
            }

            EditorGUILayout.EndVertical();
        }

        private void DrawMaterialDifferenceItem(MaterialPropertyInfo info)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(info.MaterialName, EditorStyles.boldLabel);
            if (GUILayout.Button("선택", GUILayout.Width(40)))
            {
                Selection.activeObject = info.Material;
                EditorGUIUtility.PingObject(info.Material);
            }
            EditorGUILayout.EndHorizontal();

            ShowFloatDiff("Min Light", BrightnessConstants.ShaderProperties.LIGHT_MIN_LIMIT,
                _unifySettings.UnifyMinLight, _unifySettings.MinLightValue, info);
            ShowFloatDiff("Max Light", BrightnessConstants.ShaderProperties.LIGHT_MAX_LIMIT,
                _unifySettings.UnifyMaxLight, _unifySettings.MaxLightValue, info);
            ShowFloatDiff("Back Light", BrightnessConstants.ShaderProperties.BACKLIGHT_BORDER,
                _unifySettings.UnifyBackLight, _unifySettings.BackLightValue, info);
            ShowFloatDiff("강도", BrightnessConstants.ShaderProperties.SHADOW_STRENGTH,
                _unifySettings.UnifyShadowStrength, _unifySettings.ShadowStrengthValue, info);

            EditorGUILayout.EndVertical();
        }

        private void ShowFloatDiff(string label, string key, bool unify, float unifyValue, MaterialPropertyInfo info)
        {
            if (!unify) return;
            if (!info.Values.TryGetValue(key, out var val)) return;
            if (!(val is float fVal)) return;
            if (Mathf.Approximately(fVal, unifyValue)) return;

            EditorGUILayout.LabelField($"  {label}: {fVal:F3} → {unifyValue:F3}", _diffStyle);
        }

        private void DrawShadowGroupSection() { }

        private void SetAllShadowUnifyFlags(bool enabled)
        {
            _unifySettings.UnifyShadowStrength = enabled;
            _unifySettings.UnifyShadowStrengthLOD = enabled;
            _unifySettings.UnifyShadow1stColor = enabled;
            _unifySettings.UnifyShadow1stBorder = enabled;
            _unifySettings.UnifyShadow1stBlur = enabled;
            _unifySettings.UnifyShadow1stNormalStrength = enabled;
            _unifySettings.UnifyShadow1stReceive = enabled;
            _unifySettings.UnifyShadow2ndColor = enabled;
            _unifySettings.UnifyShadow2ndAlpha = enabled;
            _unifySettings.UnifyShadow2ndBorder = enabled;
            _unifySettings.UnifyShadow2ndBlur = enabled;
            _unifySettings.UnifyShadow2ndNormalStrength = enabled;
            _unifySettings.UnifyShadow2ndReceive = enabled;
            _unifySettings.UnifyShadow3rdColor = enabled;
            _unifySettings.UnifyShadow3rdAlpha = enabled;
            _unifySettings.UnifyShadowBorderColor = enabled;
            _unifySettings.UnifyShadowBorderRange = enabled;
            _unifySettings.UnifyShadowMainStrength = enabled;
            _unifySettings.UnifyShadowEnvStrength = enabled;
            _unifySettings.UnifyShadowCasterBias = enabled;
            _unifySettings.UnifyShadowBlurMaskLOD = enabled;
        }

        private static void DrawUnifySlider(string label, ref bool enabled, ref float value, float min, float max)
        {
            EditorGUILayout.BeginHorizontal();
            enabled = EditorGUILayout.Toggle(enabled, GUILayout.Width(20));
            EditorGUILayout.LabelField(label, GUILayout.Width(85));
            using (new EditorGUI.DisabledScope(!enabled))
            {
                value = EditorGUILayout.Slider(value, min, max);
            }
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawShadowSlider(string label, ref float value, float min, float max)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(140));
            value = EditorGUILayout.Slider(value, min, max);
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawShadowColor(string label, ref Color value)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(140));
            value = EditorGUILayout.ColorField(value);
            EditorGUILayout.EndHorizontal();
        }
    }
}
