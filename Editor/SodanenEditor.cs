using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace Brightness.Utility
{
    public partial class SodanenEditor : EditorWindow
    {
        private GameObject _targetAvatar;
        private GameObject _lastAvatar;
        private List<string> _allMaterialPaths = new List<string>();
        private Vector2 _scrollPosition;

        private readonly FeatureToggles _featureToggles = new FeatureToggles();
        private readonly FeatureMaterialSelections _materialSelections = new FeatureMaterialSelections();
        private UnifySettings _unifySettings = new UnifySettings();
        private readonly ShadowUnifyUIState _shadowUIState = new ShadowUnifyUIState();
        private readonly List<FeatureInfo> _lightFeatures;
        private readonly List<FeatureInfo> _shadowFeatures;

        private bool _showMaterialSettingsSection = false;
        private bool _showUnifySection = true;
        private bool _unifyEnableShadowGroup = true;
        private List<MaterialPropertyInfo> _materialDifferences = new List<MaterialPropertyInfo>();
        private bool _showDifferences;
        private readonly List<ShadowGroupOverride> _shadowGroups = new List<ShadowGroupOverride>();
        private readonly List<CustomMaterialShadowEntry> _customMaterialEntries = new List<CustomMaterialShadowEntry>();
        private bool _showCustomMaterialSection = true;

        private GUIStyle _labelBoldStyle;
        private GUIStyle _buttonStyle;
        private GUIStyle _diffStyle;
        private GUIStyle _infoStyle;

        public SodanenEditor()
        {
            _lightFeatures = CreateLightFeatures();
            _shadowFeatures = CreateShadowFeatures();
        }

        private void OnEnable()
        {
            InitPresets();
        }

        [MenuItem("Sodanen/밝기 조절 에디터")]
        public static void ShowWindow()
        {
            var window = GetWindow<SodanenEditor>("Sodanen 에디터");
            window.minSize = new Vector2(420, 500);
            window.maxSize = new Vector2(600, 1400);
        }

        private void OnGUI()
        {
            InitStyles();
            SodanenEditorUI.DrawBackground(position);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);

            GUILayout.Space(10);
            SodanenEditorUI.DrawHeader();
            GUILayout.Space(15);
            DrawAvatarSection();
            GUILayout.Space(10);
            DrawPresetSection();
            GUILayout.Space(10);
            DrawMaterialSettingsSection();
            GUILayout.Space(10);
            DrawFeatureSection();
            GUILayout.Space(15);
            DrawApplyButton();
            GUILayout.Space(10);

            EditorGUILayout.EndScrollView();

            if (_targetAvatar != _lastAvatar)
            {
                _lastAvatar = _targetAvatar;
                RefreshMaterialList();
            }
        }

        private void InitStyles()
        {
            if (_labelBoldStyle == null)
                _labelBoldStyle = new GUIStyle(EditorStyles.label) { fontStyle = FontStyle.Bold };

            if (_buttonStyle == null)
                _buttonStyle = new GUIStyle(GUI.skin.button)
                {
                    fontSize = 14,
                    fontStyle = FontStyle.Bold,
                    fixedHeight = 45
                };

            if (_diffStyle == null)
                _diffStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    normal = { textColor = Color.yellow }
                };

            if (_infoStyle == null)
                _infoStyle = new GUIStyle(EditorStyles.miniLabel)
                {
                    wordWrap = true,
                    normal = { textColor = Color.gray }
                };
        }

        private void RefreshMaterialList()
        {
            _allMaterialPaths.Clear();
            _materialSelections.Clear();

            if (_targetAvatar == null) return;

            _allMaterialPaths = ShaderHelper.GetCompatibleShaderPaths(_targetAvatar);
            _materialSelections.SetAll(_allMaterialPaths, true);

            foreach (var group in _shadowGroups)
            {
                group.RefreshAvailableMaterials(_targetAvatar);
            }
        }

        private void DrawAvatarSection()
        {
            SodanenEditorUI.DrawSectionBox("아바타", () =>
            {
                GUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("대상", _labelBoldStyle, GUILayout.Width(50));
                _targetAvatar = (GameObject)EditorGUILayout.ObjectField(_targetAvatar, typeof(GameObject), true);
                EditorGUILayout.EndHorizontal();
                GUILayout.Space(5);

                if (_targetAvatar != null)
                {
                    var hasDescriptor = _targetAvatar.GetComponent<VRCAvatarDescriptor>() != null;
                    var text = hasDescriptor
                        ? $"lilToon 오브젝트: {_allMaterialPaths.Count}개 발견"
                        : "VRCAvatarDescriptor 없음";
                    var color = hasDescriptor ? SodanenEditorUI.AccentColor : SodanenEditorUI.WarningColor;
                    SodanenEditorUI.DrawStatusBox(text, color);
                }
                else
                {
                    SodanenEditorUI.DrawStatusBox("아바타를 선택하세요", Color.gray);
                }
            });
        }

        private void DrawMaterialSettingsSection()
        {
            if (_targetAvatar == null || _allMaterialPaths.Count == 0) return;

            SodanenEditorUI.DrawSectionBox("마테리얼 설정", () =>
            {
                GUILayout.Space(5);
                _showMaterialSettingsSection = EditorGUILayout.Foldout(_showMaterialSettingsSection, "마테리얼 속성 통일 / 개별 조절", true);

                if (!_showMaterialSettingsSection) return;

                GUILayout.Space(5);
                DrawUnifySectionContent();
                GUILayout.Space(10);
                DrawCustomMaterialSectionContent();
            });
        }

        private void DrawApplyButton()
        {
            var canApply = _targetAvatar != null
                           && _targetAvatar.GetComponent<VRCAvatarDescriptor>() != null
                           && _featureToggles.HasAnyFeatureSelected();

            EditorGUI.BeginDisabledGroup(!canApply);
            GUI.backgroundColor = canApply ? SodanenEditorUI.AccentColor : Color.gray;

            if (GUILayout.Button("적용하기", _buttonStyle))
            {
                ApplyAllSettings();
            }

            GUI.backgroundColor = Color.white;
            EditorGUI.EndDisabledGroup();

            if (canApply) return;

            var msgStyle = new GUIStyle(EditorStyles.centeredGreyMiniLabel);
            var message = _targetAvatar == null ? "아바타를 선택해주세요" : "최소 하나의 기능을 선택해주세요";
            EditorGUILayout.LabelField(message, msgStyle);
        }

        private void ApplyAllSettings()
        {
            var excludeMaterials = new HashSet<Material>(_shadowGroups.SelectMany(g => g.GetSelectedMaterials()));
            foreach (var entry in _customMaterialEntries.Where(e => e.Material != null))
            {
                excludeMaterials.Add(entry.Material);
            }

            MaterialUnifyHelper.UnifyMaterialProperties(_targetAvatar, _unifySettings, excludeMaterials);

            foreach (var group in _shadowGroups)
            {
                group.ApplyToSelectedMaterials();
            }

            foreach (var entry in _customMaterialEntries.Where(e => e.Material != null))
            {
                entry.ApplyToMaterial();
            }

            AssetDatabase.SaveAssets();

            SodanenEditorLogic.ApplyBrightnessControl(new BrightnessControlParameters
            {
                TargetAvatar = _targetAvatar,
                Toggles = _featureToggles,
                Selections = _materialSelections,
                AllMaterialPaths = _allMaterialPaths
            });
        }
    }
}
