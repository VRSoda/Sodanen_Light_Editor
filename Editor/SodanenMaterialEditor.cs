using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using Brightness.Localization;
using static Brightness.Localization.Loc;

namespace Brightness.Utility
{
    public partial class SodanenMaterialEditor : EditorWindow
    {
        #region Fields - Core

        private GameObject _targetAvatar;
        private GameObject _lastAvatar;
        private List<string> _allMaterialPaths = new();
        private Vector2 _scrollPosition;

        // 아바타 드롭다운용
        private VRCAvatarDescriptor[] _sceneAvatars = new VRCAvatarDescriptor[0];
        private string[] _avatarNames = new string[0];
        private int _selectedAvatarIndex = -1;

        #endregion

        #region Fields - Unify System

        private UnifySettings _unifySettings = new();
        private readonly ShadowUnifyUIState _shadowUIState = new();
        private List<MaterialPropertyInfo> _materialDifferences = new();
        private readonly List<ShadowGroupOverride> _shadowGroups = new();
        private readonly List<CustomMaterialShadowEntry> _customMaterialEntries = new();

        #endregion

        #region Fields - UI State

        private bool _showUnifySection = true;
        private bool _unifyEnableShadowGroup = true;
        private bool _showDifferences;
        private bool _showCustomMaterialSection = true;
        private GUIStyle _diffStyle;

        #endregion

        #region Constants

        private const float SectionSpacing = 6f;

        #endregion

        #region Initialization

        private void OnEnable()
        {
            LocalizationManager.CheckAndSyncLilToonLanguage();
            InitPresets();
            RefreshSceneAvatars();
        }

        private void OnFocus()
        {
            LocalizationManager.CheckAndSyncLilToonLanguage();
            RefreshSceneAvatars();
            Repaint();
        }

        private void OnProjectChange()
        {
            LocalizationManager.CheckAndSyncLilToonLanguage();
            RefreshSceneAvatars();
            Repaint();
        }

        private void OnHierarchyChange()
        {
            RefreshSceneAvatars();
            Repaint();
        }

        private void RefreshSceneAvatars()
        {
            _sceneAvatars = AvatarHelper.GetSceneAvatars();
            _avatarNames = AvatarHelper.GetAvatarNames(_sceneAvatars);
            _selectedAvatarIndex = AvatarHelper.FindAvatarIndex(_sceneAvatars, _targetAvatar);
        }

        [MenuItem("Sodanen/Material Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<SodanenMaterialEditor>("Sodanen Material Editor");
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
                SodanenEditorUI.DrawHeader("Sodanen Material Editor");
                GUILayout.Space(10);

                DrawAvatarSection();
                GUILayout.Space(SectionSpacing);

                DrawPresetSection();
                GUILayout.Space(SectionSpacing);

                DrawMaterialSettingsContent();
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
            // 드롭다운
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(L("avatar.target"), EditorStyles.boldLabel, GUILayout.Width(55));

            var newIndex = EditorGUILayout.Popup(_selectedAvatarIndex, _avatarNames);
            if (newIndex != _selectedAvatarIndex)
            {
                _selectedAvatarIndex = newIndex;
                _targetAvatar = newIndex > 0 ? _sceneAvatars[newIndex - 1].gameObject : null;
            }
            EditorGUILayout.EndHorizontal();

            // ObjectField (드래그 앤 드롭용)
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(59);
            var newAvatar = (GameObject)EditorGUILayout.ObjectField(_targetAvatar, typeof(GameObject), true);
            if (newAvatar != _targetAvatar)
            {
                _targetAvatar = newAvatar;
                RefreshSceneAvatars();
            }
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

        private void DrawMaterialSettingsContent()
        {
            if (_targetAvatar == null || _allMaterialPaths.Count == 0)
            {
                SodanenEditorUI.DrawStatusBox(L("avatar.select"), SodanenEditorUI.SubtleGray);
                return;
            }

            DrawUnifySectionContent();
            GUILayout.Space(SectionSpacing);
            DrawCustomMaterialSectionContent();
        }

        #endregion

        #region Logic - Material Management

        private void RefreshMaterialList()
        {
            _allMaterialPaths.Clear();

            if (_targetAvatar == null) return;

            _allMaterialPaths = ShaderHelper.GetCompatibleShaderPaths(_targetAvatar);

            foreach (var group in _shadowGroups)
                group.RefreshAvailableMaterials(_targetAvatar);
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
