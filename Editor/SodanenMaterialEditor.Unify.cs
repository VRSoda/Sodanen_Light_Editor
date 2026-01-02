using System.Linq;
using UnityEditor;
using UnityEngine;
using Brightness.Localization;
using static Brightness.Localization.Loc;

namespace Brightness.Utility
{
    public partial class SodanenMaterialEditor
    {
        #region Fields - Unify

        private Material _sourceMaterial;

        #endregion

        #region Section - Unify Content

        private void DrawUnifySectionContent()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            {
                DrawUnifyHeader();

                if (_showUnifySection)
                {
                    GUILayout.Space(4);
                    DrawLightSettings();

                    GUILayout.Space(4);
                    DrawShadowSettings();

                    GUILayout.Space(4);
                    DrawShadowGroupSection();

                    GUILayout.Space(6);
                    DrawUnifyButtons();
                    DrawMaterialDifferences();

                    EditorGUILayout.LabelField(L("unify.check_before"), SodanenEditorUI.InfoStyle);
                }
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawUnifyHeader()
        {
            _showUnifySection = EditorGUILayout.Foldout(_showUnifySection, L("unify.title"), true);
            DrawSourceMaterialField();
        }

        private void DrawSourceMaterialField()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(L("unify.source_material"), GUILayout.Width(135));

            var newMaterial = (Material)EditorGUILayout.ObjectField(_sourceMaterial, typeof(Material), false);
            if (newMaterial != _sourceMaterial)
            {
                _sourceMaterial = newMaterial;
                if (_sourceMaterial != null)
                    ApplyMaterialToUnifySettings(_sourceMaterial);
            }

            EditorGUILayout.EndHorizontal();
        }

        #endregion

        private void ApplyMaterialToUnifySettings(Material material)
        {
            if (material == null) return;

            // Light
            if (material.HasProperty(BrightnessConstants.ShaderProperties.LIGHT_MIN_LIMIT))
                _unifySettings.MinLightValue = material.GetFloat(BrightnessConstants.ShaderProperties.LIGHT_MIN_LIMIT);
            if (material.HasProperty(BrightnessConstants.ShaderProperties.LIGHT_MAX_LIMIT))
                _unifySettings.MaxLightValue = material.GetFloat(BrightnessConstants.ShaderProperties.LIGHT_MAX_LIMIT);
            if (material.HasProperty(BrightnessConstants.ShaderProperties.BACKLIGHT_BORDER))
                _unifySettings.BackLightValue = 1f - material.GetFloat(BrightnessConstants.ShaderProperties.BACKLIGHT_BORDER);

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
            EditorGUILayout.LabelField(L("feature.light"), EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            DrawUnifySlider(L("feature.minlight"), ref _unifySettings.UnifyMinLight, ref _unifySettings.MinLightValue, 0f, 1f);
            DrawUnifySlider(L("feature.maxlight"), ref _unifySettings.UnifyMaxLight, ref _unifySettings.MaxLightValue, 0f, 10f);
            DrawUnifySlider(L("feature.backlight"), ref _unifySettings.UnifyBackLight, ref _unifySettings.BackLightValue, 0f, 1f);
            EditorGUILayout.EndVertical();
        }

        private void DrawShadowSettings()
        {
            EditorGUILayout.BeginHorizontal();
            var newEnableGroup = EditorGUILayout.Toggle(_unifyEnableShadowGroup, GUILayout.Width(18));
            _shadowUIState.ShowShadowGroup = EditorGUILayout.Foldout(_shadowUIState.ShowShadowGroup, L("unify.shadow"), true);
            EditorGUILayout.EndHorizontal();

            if (_shadowUIState.ShowShadowGroup)
            {
                using (new EditorGUI.DisabledScope(!_unifyEnableShadowGroup))
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    DrawShadowMasterSection();
                    DrawSeparator();
                    DrawShadow1stSection();
                    DrawSeparator();
                    DrawShadow2ndSection();
                    DrawSeparator();
                    DrawShadow3rdSection();
                    DrawSeparator();
                    DrawShadowBorderSettings();
                    DrawSeparator();
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
            _shadowUIState.ShowShadowMaster = EditorGUILayout.Foldout(_shadowUIState.ShowShadowMaster, L("unify.mask_strength"), true);
            if (!_shadowUIState.ShowShadowMaster) return;

            EditorGUI.indentLevel++;
            DrawShadowSlider(L("unify.strength"), ref _unifySettings.ShadowStrengthValue, 0f, 1f);
            DrawShadowSlider("LOD", ref _unifySettings.ShadowStrengthLODValue, 0f, 1f);
            EditorGUI.indentLevel--;
        }

        private void DrawShadow1stSection()
        {
            _shadowUIState.ShowShadow1st = EditorGUILayout.Foldout(_shadowUIState.ShowShadow1st, L("unify.shadow_1"), true);
            if (!_shadowUIState.ShowShadow1st) return;

            EditorGUI.indentLevel++;
            DrawShadowColor(L("unify.color"), ref _unifySettings.Shadow1stColorValue);
            DrawShadowSlider(L("unify.range"), ref _unifySettings.Shadow1stBorderValue, 0f, 1f);
            DrawShadowSlider(L("unify.blur"), ref _unifySettings.Shadow1stBlurValue, 0f, 1f);
            DrawShadowSlider(L("unify.normal_strength"), ref _unifySettings.Shadow1stNormalStrengthValue, 0f, 1f);
            DrawShadowSlider(L("unify.receive_shadow"), ref _unifySettings.Shadow1stReceiveValue, 0f, 1f);
            EditorGUI.indentLevel--;
        }

        private void DrawShadow2ndSection()
        {
            _shadowUIState.ShowShadow2nd = EditorGUILayout.Foldout(_shadowUIState.ShowShadow2nd, L("unify.shadow_2"), true);
            if (!_shadowUIState.ShowShadow2nd) return;

            EditorGUI.indentLevel++;
            DrawShadowColor(L("unify.color"), ref _unifySettings.Shadow2ndColorValue);
            DrawShadowSlider(L("unify.alpha"), ref _unifySettings.Shadow2ndAlphaValue, 0f, 1f);
            DrawShadowSlider(L("unify.range"), ref _unifySettings.Shadow2ndBorderValue, 0f, 1f);
            DrawShadowSlider(L("unify.blur"), ref _unifySettings.Shadow2ndBlurValue, 0f, 1f);
            DrawShadowSlider(L("unify.normal_strength"), ref _unifySettings.Shadow2ndNormalStrengthValue, 0f, 1f);
            DrawShadowSlider(L("unify.receive_shadow"), ref _unifySettings.Shadow2ndReceiveValue, 0f, 1f);
            EditorGUI.indentLevel--;
        }

        private void DrawShadow3rdSection()
        {
            _shadowUIState.ShowShadow3rd = EditorGUILayout.Foldout(_shadowUIState.ShowShadow3rd, L("unify.shadow_3"), true);
            if (!_shadowUIState.ShowShadow3rd) return;

            EditorGUI.indentLevel++;
            DrawShadowColor(L("unify.color"), ref _unifySettings.Shadow3rdColorValue);
            DrawShadowSlider(L("unify.alpha"), ref _unifySettings.Shadow3rdAlphaValue, 0f, 1f);
            EditorGUI.indentLevel--;
        }

        private void DrawShadowBorderSettings()
        {
            DrawShadowColor(L("unify.border_color"), ref _unifySettings.ShadowBorderColorValue);
            DrawShadowSlider(L("unify.border_range"), ref _unifySettings.ShadowBorderRangeValue, 0f, 1f);
            GUILayout.Space(3);
            DrawShadowSlider(L("unify.contrast"), ref _unifySettings.ShadowMainStrengthValue, 0f, 1f);
            DrawShadowSlider(L("unify.env_strength"), ref _unifySettings.ShadowEnvStrengthValue, 0f, 1f);
            DrawShadowSlider("Shadow Caster Bias", ref _unifySettings.ShadowCasterBiasValue, 0f, 1f);
        }

        private void DrawShadowBlurMaskSection()
        {
            _shadowUIState.ShowShadowBlurMask = EditorGUILayout.Foldout(_shadowUIState.ShowShadowBlurMask, L("unify.blur_mask"), true);
            if (!_shadowUIState.ShowShadowBlurMask) return;

            EditorGUI.indentLevel++;
            DrawShadowSlider("LOD", ref _unifySettings.ShadowBlurMaskLODValue, 0f, 1f);
            EditorGUI.indentLevel--;
        }

        private void DrawUnifyButtons()
        {
            SodanenEditorUI.BeginButtonRow();
            if (SodanenEditorUI.DrawButton(L("unify.compare")))
            {
                var excludeMaterials = GetExcludedMaterials();
                _materialDifferences = MaterialUnifyHelper.FindDifferentMaterials(_targetAvatar, _unifySettings, excludeMaterials);
                _showDifferences = true;
                if (_materialDifferences.Count == 0)
                {
                    EditorUtility.DisplayDialog(L("dialog.result"), L("dialog.all_materials_match"), L("dialog.confirm"));
                }
            }

            if (SodanenEditorUI.DrawButton(L("unify.apply_material")))
            {
                if (EditorUtility.DisplayDialog(L("dialog.unify_title"),
                    L("dialog.unify_message"),
                    L("dialog.apply"), L("dialog.cancel")))
                {
                    var excludeMaterials = GetExcludedMaterials();
                    MaterialUnifyHelper.UnifyMaterialProperties(_targetAvatar, _unifySettings, excludeMaterials);
                    AssetDatabase.SaveAssets();
                    _materialDifferences.Clear();
                    _showDifferences = false;
                    EditorUtility.DisplayDialog(L("dialog.complete"), L("dialog.unify_complete"), L("dialog.confirm"));
                }
            }
            SodanenEditorUI.EndButtonRow();

            GUILayout.Space(6);

            DrawNonDestructiveButtons();
        }

        private void DrawNonDestructiveButtons()
        {
            SodanenEditorUI.DrawSubSection(L("nondestructive.title"), L("nondestructive.description"), () =>
            {
                SodanenEditorUI.BeginButtonRow();

                if (SodanenEditorUI.DrawButton(L("nondestructive.duplicate_only")))
                {
                    if (EditorUtility.DisplayDialog(L("dialog.duplicate_title"),
                        L("dialog.duplicate_message"),
                        L("dialog.duplicate"), L("dialog.cancel")))
                    {
                        ExecuteNonDestructiveDuplicate(applyUnify: false);
                    }
                }

                if (SodanenEditorUI.DrawButton(L("nondestructive.duplicate_unify")))
                {
                    if (EditorUtility.DisplayDialog(L("dialog.duplicate_unify_title"),
                        L("dialog.duplicate_unify_message"),
                        L("dialog.duplicate_unify_button"), L("dialog.cancel")))
                    {
                        ExecuteNonDestructiveDuplicate(applyUnify: true);
                    }
                }

                if (SodanenEditorUI.DrawButton(L("nondestructive.delete_all")))
                {
                    if (EditorUtility.DisplayDialog(L("dialog.delete_all_title"),
                        L("dialog.delete_all_message"),
                        L("dialog.delete"), L("dialog.cancel")))
                    {
                        DeleteDuplicatedAvatarAndMaterials();
                    }
                }

                SodanenEditorUI.EndButtonRow();
            });
        }

        private void DeleteDuplicatedAvatarAndMaterials()
        {
            if (_targetAvatar == null) return;

            // _Copy로 끝나는 아바타인지 확인
            if (!_targetAvatar.name.EndsWith("_Copy"))
            {
                EditorUtility.DisplayDialog(L("dialog.warning"),
                    L("dialog.not_duplicated_avatar"), L("dialog.confirm"));
                return;
            }

            string avatarName = _targetAvatar.name;
            string baseName = avatarName.Substring(0, avatarName.Length - 5);
            string folderPath = $"{BrightnessConstants.CREATE_PATH}{baseName}_Materials";

            // 아바타 삭제
            Undo.DestroyObjectImmediate(_targetAvatar);
            _targetAvatar = null;
            _lastAvatar = null;
            RefreshMaterialList();

            // 마테리얼 폴더 삭제
            bool materialDeleted = false;
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.DeleteAsset(folderPath);
                AssetDatabase.Refresh();
                materialDeleted = true;
            }

            string message = materialDeleted
                ? L("dialog.delete_all_complete", avatarName, folderPath)
                : L("dialog.delete_avatar_complete", avatarName);

            EditorUtility.DisplayDialog(L("dialog.complete"), message, L("dialog.confirm"));
        }

        private void ExecuteNonDestructiveDuplicate(bool applyUnify)
        {
            var result = MaterialUnifyHelper.DuplicateMaterialsNonDestructive(
                _targetAvatar,
                applyUnifySettings: applyUnify,
                settings: applyUnify ? _unifySettings : null,
                excludeMaterials: null);

            if (result != null)
            {
                _targetAvatar = result.DuplicatedAvatar;
                RefreshMaterialList();

                ApplyGroupOverridesToDuplicatedAvatar(result.DuplicatedAvatar);
                ApplyCustomMaterialSettingsToDuplicatedAvatar(result.DuplicatedAvatar);

                if (applyUnify)
                {
                    _materialDifferences.Clear();
                    _showDifferences = false;
                }

                EditorUtility.DisplayDialog(L("dialog.complete"),
                    $"{L("dialog.complete")}\n\n{L("avatar.title")}: {result.DuplicatedAvatar.name}\n{L("custom.material")}: {result.MaterialCount}", L("dialog.confirm"));

                Selection.activeGameObject = result.DuplicatedAvatar;
                EditorGUIUtility.PingObject(result.DuplicatedAvatar);
            }
        }

        private void ApplyGroupOverridesToDuplicatedAvatar(GameObject duplicatedAvatar)
        {
            var renderers = duplicatedAvatar.GetComponentsInChildren<Renderer>(true);
            foreach (var group in _shadowGroups)
            {
                var originalMaterials = group.GetSelectedMaterials();
                foreach (var renderer in renderers)
                {
                    if (renderer.sharedMaterials == null) continue;
                    foreach (var mat in renderer.sharedMaterials)
                    {
                        if (mat != null && originalMaterials.Any(m => m.name == mat.name))
                        {
                            group.ApplySettingsToMaterial(mat);
                            EditorUtility.SetDirty(mat);
                        }
                    }
                }
            }
        }

        private void ApplyCustomMaterialSettingsToDuplicatedAvatar(GameObject duplicatedAvatar)
        {
            var renderers = duplicatedAvatar.GetComponentsInChildren<Renderer>(true);
            foreach (var entry in _customMaterialEntries.Where(e => e.Material != null))
            {
                string originalName = entry.Material.name;
                foreach (var renderer in renderers)
                {
                    if (renderer.sharedMaterials == null) continue;
                    foreach (var mat in renderer.sharedMaterials)
                    {
                        if (mat != null && mat.name == originalName)
                        {
                            entry.ApplySettingsToMaterial(mat);
                            EditorUtility.SetDirty(mat);
                        }
                    }
                }
            }
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
            EditorGUILayout.LabelField(L("dialog.different_materials", _materialDifferences.Count), EditorStyles.boldLabel);
            if (GUILayout.Button(L("dialog.close"), GUILayout.Width(40))) _showDifferences = false;
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
            if (GUILayout.Button(L("dialog.select"), GUILayout.Width(40)))
            {
                Selection.activeObject = info.Material;
                EditorGUIUtility.PingObject(info.Material);
            }
            EditorGUILayout.EndHorizontal();

            ShowFloatDiff(L("feature.minlight"), BrightnessConstants.ShaderProperties.LIGHT_MIN_LIMIT,
                _unifySettings.UnifyMinLight, _unifySettings.MinLightValue, info);
            ShowFloatDiff(L("feature.maxlight"), BrightnessConstants.ShaderProperties.LIGHT_MAX_LIMIT,
                _unifySettings.UnifyMaxLight, _unifySettings.MaxLightValue, info);
            ShowFloatDiff(L("feature.backlight"), BrightnessConstants.ShaderProperties.BACKLIGHT_BORDER,
                _unifySettings.UnifyBackLight, _unifySettings.BackLightValue, info);
            ShowFloatDiff(L("unify.strength"), BrightnessConstants.ShaderProperties.SHADOW_STRENGTH,
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
            EditorGUILayout.LabelField(label, GUILayout.Width(155));
            value = EditorGUILayout.Slider(value, min, max);
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawShadowColor(string label, ref Color value)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(155));
            value = EditorGUILayout.ColorField(value);
            EditorGUILayout.EndHorizontal();
        }

        private static void DrawSeparator()
        {
            GUILayout.Space(4);
            var rect = EditorGUILayout.GetControlRect(false, 1);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 0.3f));
            GUILayout.Space(4);
        }
    }
}
