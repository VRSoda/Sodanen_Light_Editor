using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using Brightness.Localization;
using static Brightness.Localization.Loc;

namespace Brightness.Utility
{
    public partial class SodanenEditor : EditorWindow
    {
        #region Fields - Core

        private GameObject _targetAvatar;
        private GameObject _lastAvatar;
        private List<string> _allMaterialPaths = new();
        private Vector2 _scrollPosition;

        #endregion

        #region Fields - Feature System

        private readonly FeatureToggles _featureToggles = new();
        private readonly FeatureMaterialSelections _materialSelections = new();
        private readonly List<FeatureInfo> _lightFeatures;
        private readonly List<FeatureInfo> _shadowFeatures;

        #endregion

        #region Fields - Unify System

        private UnifySettings _unifySettings = new();
        private readonly ShadowUnifyUIState _shadowUIState = new();
        private List<MaterialPropertyInfo> _materialDifferences = new();
        private readonly List<ShadowGroupOverride> _shadowGroups = new();
        private readonly List<CustomMaterialShadowEntry> _customMaterialEntries = new();

        #endregion

        #region Fields - UI State

        private bool _showMaterialSettingsSection;
        private bool _showUnifySection = true;
        private bool _unifyEnableShadowGroup = true;
        private bool _showDifferences;
        private bool _showCustomMaterialSection = true;
        private GUIStyle _diffStyle;

        #endregion

        #region Constants

        private const float SectionSpacing = 6f;
        private const float ApplyButtonHeight = 36f;

        #endregion

        #region Initialization

        public SodanenEditor()
        {
            _lightFeatures = CreateLightFeatures();
            _shadowFeatures = CreateShadowFeatures();
        }

        private void OnEnable()
        {
            LocalizationManager.CheckAndSyncLilToonLanguage();
            InitPresets();
        }

        [MenuItem("Sodanen/Light Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<SodanenEditor>("Sodanen Light Editor");
            window.minSize = new Vector2(420, 500);
            window.maxSize = new Vector2(600, 1200);
        }

        private void InitStyles()
        {
            _diffStyle ??= new GUIStyle(EditorStyles.miniLabel)
            {
                normal = { textColor = new Color(1f, 0.9f, 0.4f) }
            };
        }

        #endregion

        #region Main GUI

        private void OnGUI()
        {
            InitStyles();
            LocalizationManager.CheckAndSyncLilToonLanguage();
            SodanenEditorUI.DrawBackground(position);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            {
                GUILayout.Space(6);
                SodanenEditorUI.DrawHeader();
                GUILayout.Space(10);

                DrawAvatarSection();
                GUILayout.Space(SectionSpacing);

                DrawPresetSection();
                GUILayout.Space(SectionSpacing);

                DrawMaterialSettingsSection();
                GUILayout.Space(SectionSpacing);

                DrawFeatureSection();
                GUILayout.Space(10);

                DrawApplyButton();
                GUILayout.Space(8);
            }
            EditorGUILayout.EndScrollView();

            HandleAvatarChange();
        }

        private void HandleAvatarChange()
        {
            if (_targetAvatar == _lastAvatar) return;

            _lastAvatar = _targetAvatar;
            RefreshMaterialList();
        }

        #endregion

        #region Section - Avatar

        private void DrawAvatarSection()
        {
            SodanenEditorUI.DrawSectionBox(L("avatar.title"), () =>
            {
                DrawAvatarField();
                GUILayout.Space(4);
                DrawAvatarStatus();
            });
        }

        private void DrawAvatarField()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(L("avatar.target"), EditorStyles.boldLabel, GUILayout.Width(55));
            _targetAvatar = (GameObject)EditorGUILayout.ObjectField(_targetAvatar, typeof(GameObject), true);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawAvatarStatus()
        {
            if (_targetAvatar == null)
            {
                SodanenEditorUI.DrawStatusBox(L("avatar.select"), SodanenEditorUI.SubtleGray);
                return;
            }

            var hasDescriptor = _targetAvatar.GetComponent<VRCAvatarDescriptor>() != null;
            var (text, color) = hasDescriptor
                ? (L("avatar.material_count", _allMaterialPaths.Count), SodanenEditorUI.AccentColor)
                : (L("avatar.no_descriptor"), SodanenEditorUI.WarningColor);

            SodanenEditorUI.DrawStatusBox(text, color);
        }

        #endregion

        #region Section - Material Settings

        private void DrawMaterialSettingsSection()
        {
            if (_targetAvatar == null || _allMaterialPaths.Count == 0) return;

            SodanenEditorUI.DrawSectionBox(L("material.title"), () =>
            {
                _showMaterialSettingsSection = EditorGUILayout.Foldout(
                    _showMaterialSettingsSection, L("material.unify_toggle"), true);

                if (!_showMaterialSettingsSection) return;

                GUILayout.Space(4);
                DrawUnifySectionContent();
                GUILayout.Space(SectionSpacing);
                DrawCustomMaterialSectionContent();
            });
        }

        #endregion

        #region Section - Apply Button

        private void DrawApplyButton()
        {
            var canApply = CanApplySettings();

            using (new EditorGUI.DisabledScope(!canApply))
            {
                if (SodanenEditorUI.DrawButton(L("button.apply"), SodanenEditorUI.AccentColor, ApplyButtonHeight))
                {
                    ApplyAllSettings();
                }
            }

            if (!canApply)
            {
                var message = _targetAvatar == null
                    ? L("button.select_avatar")
                    : L("button.select_feature");
                EditorGUILayout.LabelField(message, SodanenEditorUI.InfoStyle);
            }
        }

        private bool CanApplySettings()
        {
            return _targetAvatar != null
                   && _targetAvatar.GetComponent<VRCAvatarDescriptor>() != null
                   && _featureToggles.HasAnyFeatureSelected();
        }

        #endregion

        #region Logic - Material Management

        private void RefreshMaterialList()
        {
            _allMaterialPaths.Clear();
            _materialSelections.Clear();

            if (_targetAvatar == null) return;

            _allMaterialPaths = ShaderHelper.GetCompatibleShaderPaths(_targetAvatar);
            _materialSelections.SetAll(_allMaterialPaths, true);

            foreach (var group in _shadowGroups)
                group.RefreshAvailableMaterials(_targetAvatar);
        }

        private void ApplyAllSettings()
        {
            var excludeMaterials = CollectExcludedMaterials();

            MaterialUnifyHelper.UnifyMaterialProperties(_targetAvatar, _unifySettings, excludeMaterials);
            ApplyGroupOverrides();
            ApplyCustomMaterialSettings();

            AssetDatabase.SaveAssets();

            SodanenEditorLogic.ApplyBrightnessControl(new BrightnessControlParameters
            {
                TargetAvatar = _targetAvatar,
                Toggles = _featureToggles,
                Selections = _materialSelections,
                AllMaterialPaths = _allMaterialPaths
            });
        }

        private HashSet<Material> CollectExcludedMaterials()
        {
            var excludeMaterials = new HashSet<Material>(_shadowGroups.SelectMany(g => g.GetSelectedMaterials()));

            foreach (var entry in _customMaterialEntries.Where(e => e.Material != null))
                excludeMaterials.Add(entry.Material);

            return excludeMaterials;
        }

        private void ApplyGroupOverrides()
        {
            foreach (var group in _shadowGroups)
                group.ApplyToSelectedMaterials();
        }

        private void ApplyCustomMaterialSettings()
        {
            foreach (var entry in _customMaterialEntries.Where(e => e.Material != null))
                entry.ApplyToMaterial();
        }

        #endregion
    }
}
