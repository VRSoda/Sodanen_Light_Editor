using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
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

        // 아바타 드롭다운용
        private VRCAvatarDescriptor[] _sceneAvatars = new VRCAvatarDescriptor[0];
        private string[] _avatarNames = new string[0];
        private int _selectedAvatarIndex = -1;

        // 메뉴 위치 드롭다운용
        private List<VRCExpressionsMenu> _avatarMenus = new();
        private string[] _menuNames = new string[0];
        private int _selectedMenuIndex = 0;
        private VRCExpressionsMenu _targetMenu;

        #endregion

        #region Fields - Feature System

        private readonly FeatureToggles _featureToggles = new();
        private readonly FeatureMaterialSelections _materialSelections = new();
        private readonly List<FeatureInfo> _lightFeatures;
        private readonly List<FeatureInfo> _shadowFeatures;

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

        [MenuItem("Sodanen/Light Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<SodanenEditor>("Sodanen Light Editor");
            window.minSize = new Vector2(420, 400);
            window.maxSize = new Vector2(600, 800);
        }

        #endregion

        #region Main GUI

        private void OnGUI()
        {
            LocalizationManager.CheckAndSyncLilToonLanguage();
            SodanenEditorUI.DrawBackground(position);

            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
            {
                GUILayout.Space(6);
                SodanenEditorUI.DrawHeader();
                GUILayout.Space(10);

                DrawAvatarSection();
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
            RefreshMenuList();
        }

        #endregion

        #region Section - Avatar

        private void DrawAvatarSection()
        {
            SodanenEditorUI.DrawSectionBox(L("avatar.title"), () =>
            {
                DrawAvatarField();
                GUILayout.Space(4);
                DrawMenuLocationField();
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

        private void DrawMenuLocationField()
        {
            if (_targetAvatar == null) return;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(L("avatar.menu_location"), EditorStyles.boldLabel, GUILayout.Width(55));

            var newIndex = EditorGUILayout.Popup(_selectedMenuIndex, _menuNames);
            if (newIndex != _selectedMenuIndex)
            {
                _selectedMenuIndex = newIndex;
                _targetMenu = newIndex > 0 ? _avatarMenus[newIndex - 1] : null;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void RefreshMenuList()
        {
            _avatarMenus.Clear();
            _selectedMenuIndex = 0;
            _targetMenu = null;

            if (_targetAvatar == null)
            {
                _menuNames = new[] { L("avatar.menu_root") };
                return;
            }

            var descriptor = _targetAvatar.GetComponent<VRCAvatarDescriptor>();
            if (descriptor == null || descriptor.expressionsMenu == null)
            {
                _menuNames = new[] { L("avatar.menu_root") };
                return;
            }

            // 루트 메뉴와 서브메뉴들 수집
            CollectMenus(descriptor.expressionsMenu);

            // 드롭다운 옵션 생성
            var names = new List<string> { L("avatar.menu_root") };
            foreach (var menu in _avatarMenus)
            {
                names.Add(menu.name);
            }
            _menuNames = names.ToArray();
        }

        private void CollectMenus(VRCExpressionsMenu menu)
        {
            if (menu == null || _avatarMenus.Contains(menu)) return;

            _avatarMenus.Add(menu);

            // 서브메뉴 수집
            foreach (var control in menu.controls)
            {
                if (control.type == VRCExpressionsMenu.Control.ControlType.SubMenu && control.subMenu != null)
                {
                    CollectMenus(control.subMenu);
                }
            }
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
        }

        private void ApplyAllSettings()
        {
            SodanenEditorLogic.ApplyBrightnessControl(new BrightnessControlParameters
            {
                TargetAvatar = _targetAvatar,
                Toggles = _featureToggles,
                Selections = _materialSelections,
                AllMaterialPaths = _allMaterialPaths,
                TargetMenu = _targetMenu
            });
        }

        #endregion
    }
}
