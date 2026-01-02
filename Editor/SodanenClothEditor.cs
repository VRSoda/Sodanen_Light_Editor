using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using Brightness.Localization;
using static Brightness.Localization.Loc;
using AnimatorController = UnityEditor.Animations.AnimatorController;
using AnimatorControllerLayer = UnityEditor.Animations.AnimatorControllerLayer;
using AnimatorControllerParameterType = UnityEngine.AnimatorControllerParameterType;

namespace Brightness.Utility
{
    public class SodanenClothEditor : EditorWindow
    {
        #region Fields - Core

        private GameObject _targetAvatar;
        private GameObject _lastAvatar;
        private Vector2 _scrollPosition;

        // 아바타 드롭다운용
        private VRCAvatarDescriptor[] _sceneAvatars = new VRCAvatarDescriptor[0];
        private string[] _avatarNames = new string[0];
        private int _selectedAvatarIndex = -1;

        #endregion

        #region Fields - Toggle System

        private List<GameObject> _clothItems = new();
        private ReorderableList _reorderableList;
        private string _toggleName = "Clothes";
        private int _defaultIndex = 0;
        private bool _savedState = true;
        private bool _addAllOff = true;
        private VRCExpressionsMenu _targetMenu;
        private string _saveFolderPath = "";

        // 메뉴 드롭다운용
        private List<VRCExpressionsMenu> _avatarMenus = new();
        private string[] _menuNames = new string[0];
        private int _selectedMenuIndex = 0;

        // 설정 파일 경로 (VPM 업데이트 후에도 유지되도록 Assets/Sodanen/Settings에 저장)
        private const string SettingsFolder = "Assets/Sodanen/Settings";
        private const string SettingsFileName = "ClothEditorSettings.json";
        private string _savedTargetMenuPath = "";

        #endregion

        #region Fields - Styles

        private static GUIStyle s_dropAreaStyle;
        private static GUIStyle s_warningStyle;

        #endregion

        #region Constants

        private const float SectionSpacing = 6f;
        private const float ApplyButtonHeight = 36f;

        #endregion

        #region Initialization

        private void OnEnable()
        {
            LocalizationManager.CheckAndSyncLilToonLanguage();
            RefreshSceneAvatars();
            SetupReorderableList();
            LoadSettings();
        }

        private void LoadSettings()
        {
            var settingsPath = GetSettingsFilePath();
            if (File.Exists(settingsPath))
            {
                try
                {
                    var json = File.ReadAllText(settingsPath);
                    var settings = JsonUtility.FromJson<ClothEditorSettings>(json);
                    if (settings != null)
                    {
                        _saveFolderPath = settings.saveFolderPath ?? "";
                        _toggleName = settings.toggleName ?? "Clothes";
                        _savedState = settings.savedState;
                        _addAllOff = settings.addAllOff;
                        // 대상 메뉴는 아바타 선택 후 복원
                        _savedTargetMenuPath = settings.targetMenuPath ?? "";
                    }
                }
                catch
                {
                    _saveFolderPath = "";
                }
            }
        }

        private void SaveSettings()
        {
            EnsureDirectoryExists(SettingsFolder);
            var settingsPath = GetSettingsFilePath();
            var settings = new ClothEditorSettings
            {
                saveFolderPath = _saveFolderPath,
                toggleName = _toggleName,
                savedState = _savedState,
                addAllOff = _addAllOff,
                targetMenuPath = _targetMenu != null ? AssetDatabase.GetAssetPath(_targetMenu) : ""
            };
            var json = JsonUtility.ToJson(settings, true);
            File.WriteAllText(settingsPath, json);
        }

        private string GetSettingsFilePath()
        {
            return Path.Combine(Application.dataPath.Replace("Assets", ""), SettingsFolder, SettingsFileName);
        }

        [System.Serializable]
        private class ClothEditorSettings
        {
            public string saveFolderPath = "";
            public string toggleName = "Clothes";
            public bool savedState = true;
            public bool addAllOff = true;
            public string targetMenuPath = "";
        }

        private void SetupReorderableList()
        {
            _reorderableList = new ReorderableList(_clothItems, typeof(GameObject), true, false, false, false);

            _reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (index >= _clothItems.Count) return;

                var item = _clothItems[index];
                var labelRect = new Rect(rect.x, rect.y + 2, 25, EditorGUIUtility.singleLineHeight);
                var fieldRect = new Rect(rect.x + 28, rect.y + 2, rect.width - 53, EditorGUIUtility.singleLineHeight);
                var delRect = new Rect(rect.x + rect.width - 20, rect.y + 2, 20, EditorGUIUtility.singleLineHeight);

                // 인덱스
                EditorGUI.LabelField(labelRect, $"{index}.");

                // GameObject 필드
                var newItem = (GameObject)EditorGUI.ObjectField(fieldRect, item, typeof(GameObject), true);
                if (newItem != item)
                {
                    if (newItem != null && _targetAvatar != null && !IsChildOf(newItem.transform, _targetAvatar.transform))
                    {
                        Debug.LogWarning($"[Cloth Editor] '{newItem.name}' is not a child of the selected avatar.");
                    }
                    else
                    {
                        _clothItems[index] = newItem;
                    }
                }

                // 삭제 버튼
                if (GUI.Button(delRect, "×"))
                {
                    _clothItems.RemoveAt(index);
                    if (_defaultIndex >= _clothItems.Count)
                        _defaultIndex = Mathf.Max(0, _clothItems.Count - 1);
                }
            };

            _reorderableList.elementHeight = EditorGUIUtility.singleLineHeight + 6;
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

        [MenuItem("Sodanen/Cloth Editor")]
        public static void ShowWindow()
        {
            var window = GetWindow<SodanenClothEditor>("Sodanen Cloth Editor");
            window.minSize = new Vector2(420, 500);
            window.maxSize = new Vector2(600, 900);
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
                SodanenEditorUI.DrawHeader("Sodanen Cloth Editor");
                GUILayout.Space(10);

                DrawAvatarSection();
                GUILayout.Space(SectionSpacing);

                DrawClothItemsSection();
                GUILayout.Space(SectionSpacing);

                DrawSettingsSection();
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
            RefreshAvatarMenus();
        }

        private void RefreshAvatarMenus()
        {
            _avatarMenus.Clear();
            _selectedMenuIndex = 0;
            _targetMenu = null;

            if (_targetAvatar == null)
            {
                _menuNames = new[] { L("cloth.new_menu") };
                return;
            }

            var descriptor = _targetAvatar.GetComponent<VRCAvatarDescriptor>();
            if (descriptor == null || descriptor.expressionsMenu == null)
            {
                _menuNames = new[] { L("cloth.new_menu") };
                return;
            }

            // 루트 메뉴와 서브메뉴들 수집
            CollectMenus(descriptor.expressionsMenu, "");

            // 드롭다운 옵션 생성
            var names = new List<string> { L("cloth.new_menu") };
            foreach (var menu in _avatarMenus)
            {
                names.Add(menu.name);
            }
            _menuNames = names.ToArray();

            // 저장된 대상 메뉴 복원
            if (!string.IsNullOrEmpty(_savedTargetMenuPath))
            {
                var savedMenu = AssetDatabase.LoadAssetAtPath<VRCExpressionsMenu>(_savedTargetMenuPath);
                if (savedMenu != null)
                {
                    var index = _avatarMenus.IndexOf(savedMenu);
                    if (index >= 0)
                    {
                        _selectedMenuIndex = index + 1;
                        _targetMenu = savedMenu;
                    }
                }
            }
        }

        private void CollectMenus(VRCExpressionsMenu menu, string prefix)
        {
            if (menu == null || _avatarMenus.Contains(menu)) return;

            _avatarMenus.Add(menu);

            // 서브메뉴 수집
            foreach (var control in menu.controls)
            {
                if (control.type == VRCExpressionsMenu.Control.ControlType.SubMenu && control.subMenu != null)
                {
                    CollectMenus(control.subMenu, prefix + menu.name + "/");
                }
            }
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
            var totalItems = _clothItems.Count(i => i != null);
            var (text, color) = hasDescriptor
                ? (L("cloth.selected_count", totalItems), SodanenEditorUI.AccentColor)
                : (L("avatar.no_descriptor"), SodanenEditorUI.WarningColor);

            SodanenEditorUI.DrawStatusBox(text, color);
        }

        #endregion

        #region Section - Cloth Items

        private void DrawClothItemsSection()
        {
            if (_targetAvatar == null) return;

            SodanenEditorUI.DrawSectionBox(L("cloth.title"), () =>
            {
                // 파라미터 이름
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(L("cloth.toggle_name"), GUILayout.Width(80));
                EditorGUI.BeginChangeCheck();
                _toggleName = EditorGUILayout.TextField(_toggleName);
                if (EditorGUI.EndChangeCheck()) SaveSettings();
                EditorGUILayout.EndHorizontal();

                GUILayout.Space(4);

                // 드래그 가능한 리스트
                if (_reorderableList != null)
                {
                    _reorderableList.DoLayoutList();
                }

                // 드래그 앤 드롭 영역
                DrawDropArea();

                GUILayout.Space(4);

                // 기본값 선택
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(L("cloth.default_index"), GUILayout.Width(80));

                var validItems = _clothItems.Where(i => i != null).ToList();
                var optionCount = _addAllOff ? validItems.Count + 1 : validItems.Count;
                var options = new string[optionCount];
                for (int i = 0; i < validItems.Count; i++)
                {
                    options[i] = validItems[i].name;
                }
                if (_addAllOff)
                {
                    options[validItems.Count] = "All OFF";
                }

                if (optionCount > 0)
                {
                    _defaultIndex = Mathf.Clamp(_defaultIndex, 0, optionCount - 1);
                    var newDefaultIndex = EditorGUILayout.Popup(_defaultIndex, options);

                    // 기본값이 변경되면 해당 아이템을 첫 번째로 이동
                    if (newDefaultIndex != _defaultIndex && newDefaultIndex < validItems.Count)
                    {
                        var selectedItem = validItems[newDefaultIndex];
                        var originalIndex = _clothItems.IndexOf(selectedItem);
                        if (originalIndex > 0)
                        {
                            _clothItems.RemoveAt(originalIndex);
                            _clothItems.Insert(0, selectedItem);
                        }
                        _defaultIndex = 0;
                    }
                    else
                    {
                        _defaultIndex = newDefaultIndex;
                    }
                }
                EditorGUILayout.EndHorizontal();

                if (_clothItems.Count == 0)
                {
                    EditorGUILayout.LabelField(L("cloth.no_items"), EditorStyles.centeredGreyMiniLabel);
                }
            });
        }

        private void DrawDropArea()
        {
            // 드롭 영역 스타일 (캐싱)
            s_dropAreaStyle ??= new GUIStyle(EditorStyles.helpBox)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Italic,
                normal = { textColor = new Color(0.6f, 0.6f, 0.6f) }
            };

            var dropRect = GUILayoutUtility.GetRect(0, 32, GUILayout.ExpandWidth(true));
            GUI.Box(dropRect, L("cloth.drop_here"), s_dropAreaStyle);

            // 드래그 앤 드롭 처리
            var evt = Event.current;
            if (dropRect.Contains(evt.mousePosition))
            {
                switch (evt.type)
                {
                    case EventType.DragUpdated:
                        if (DragAndDrop.objectReferences.Length > 0)
                        {
                            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                            evt.Use();
                        }
                        break;

                    case EventType.DragPerform:
                        DragAndDrop.AcceptDrag();

                        foreach (var obj in DragAndDrop.objectReferences)
                        {
                            if (obj is GameObject go)
                            {
                                if (_targetAvatar != null && IsChildOf(go.transform, _targetAvatar.transform))
                                {
                                    if (!_clothItems.Contains(go))
                                    {
                                        _clothItems.Add(go);
                                    }
                                }
                                else
                                {
                                    Debug.LogWarning($"[Cloth Editor] '{go.name}' is not a child of the selected avatar.");
                                }
                            }
                        }

                        evt.Use();
                        Repaint();
                        break;
                }
            }
        }

        private bool IsChildOf(Transform child, Transform parent)
        {
            var current = child;
            while (current != null)
            {
                if (current == parent) return true;
                current = current.parent;
            }
            return false;
        }

        #endregion

        #region Section - Settings

        private void DrawSettingsSection()
        {
            if (_targetAvatar == null) return;

            SodanenEditorUI.DrawSectionBox(L("cloth.settings"), () =>
            {
                // 저장 폴더 선택
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(L("cloth.save_folder"), GUILayout.Width(100));

                var displayPath = string.IsNullOrEmpty(_saveFolderPath)
                    ? L("cloth.default_folder")
                    : _saveFolderPath;
                EditorGUILayout.TextField(displayPath);

                if (GUILayout.Button("...", GUILayout.Width(30)))
                {
                    var selectedPath = EditorUtility.OpenFolderPanel(
                        L("cloth.select_folder"),
                        string.IsNullOrEmpty(_saveFolderPath) ? "Assets" : _saveFolderPath,
                        "");

                    if (!string.IsNullOrEmpty(selectedPath))
                    {
                        // Assets 폴더 내부인지 확인
                        var dataPath = Application.dataPath;
                        if (selectedPath.StartsWith(dataPath))
                        {
                            _saveFolderPath = "Assets" + selectedPath.Substring(dataPath.Length);
                            SaveSettings();
                            Repaint();
                        }
                        else
                        {
                            EditorUtility.DisplayDialog(
                                L("dialog.error"),
                                L("cloth.folder_error"),
                                L("dialog.confirm"));
                        }
                    }
                }

                EditorGUILayout.EndHorizontal();

                // 대상 메뉴 선택 (드롭다운)
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(L("cloth.target_menu"), GUILayout.Width(100));

                var newMenuIndex = EditorGUILayout.Popup(_selectedMenuIndex, _menuNames);
                if (newMenuIndex != _selectedMenuIndex)
                {
                    _selectedMenuIndex = newMenuIndex;
                    _targetMenu = newMenuIndex > 0 && newMenuIndex <= _avatarMenus.Count
                        ? _avatarMenus[newMenuIndex - 1]
                        : null;
                    SaveSettings();
                }
                EditorGUILayout.EndHorizontal();

                // Saved 옵션
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(L("cloth.saved_state"), GUILayout.Width(100));
                EditorGUI.BeginChangeCheck();
                _savedState = EditorGUILayout.Toggle(_savedState);
                if (EditorGUI.EndChangeCheck()) SaveSettings();
                EditorGUILayout.EndHorizontal();

                // All OFF 옵션 추가 여부
                EditorGUILayout.BeginHorizontal();
                s_warningStyle ??= new GUIStyle(EditorStyles.label) { normal = { textColor = SodanenEditorUI.WarningColor } };
                EditorGUILayout.LabelField(L("cloth.add_all_off"), s_warningStyle, GUILayout.Width(100));
                EditorGUI.BeginChangeCheck();
                _addAllOff = EditorGUILayout.Toggle(_addAllOff);
                if (EditorGUI.EndChangeCheck()) SaveSettings();
                EditorGUILayout.EndHorizontal();
            });
        }

        #endregion

        #region Section - Apply Button

        private void DrawApplyButton()
        {
            var hasValidItems = _clothItems.Any(i => i != null);
            var canApply = _targetAvatar != null
                           && _targetAvatar.GetComponent<VRCAvatarDescriptor>() != null
                           && hasValidItems;

            using (new EditorGUI.DisabledScope(!canApply))
            {
                if (SodanenEditorUI.DrawButton(L("cloth.apply"), SodanenEditorUI.AccentColor, ApplyButtonHeight))
                {
                    ApplyClothToggle();
                }
            }

            if (!canApply)
            {
                var message = _targetAvatar == null
                    ? L("button.select_avatar")
                    : !hasValidItems
                        ? L("cloth.select_items")
                        : L("avatar.no_descriptor");
                EditorGUILayout.LabelField(message, SodanenEditorUI.InfoStyle);
            }
        }

        #endregion

        #region Logic - Apply Cloth Toggle

        private void ApplyClothToggle()
        {
            var descriptor = _targetAvatar.GetComponent<VRCAvatarDescriptor>();
            if (descriptor == null) return;

            var validItems = _clothItems.Where(i => i != null).ToList();
            if (validItems.Count == 0) return;

            // 기본 저장 경로 (FX, Parameters, Menu 등)
            var avatarPath = $"Assets/Sodanen/Generated/{_targetAvatar.name}";
            EnsureDirectoryExists(avatarPath);

            // 애니메이션 클립 저장 경로 (사용자 지정 폴더 또는 기본 경로)
            var clipPath = string.IsNullOrEmpty(_saveFolderPath)
                ? $"{avatarPath}/Clips"
                : _saveFolderPath;
            EnsureDirectoryExists(clipPath);

            // FX Controller 가져오기 또는 생성
            var fxController = GetOrCreateFXController(descriptor, avatarPath);

            // Parameters 가져오기 또는 생성
            var parameters = GetOrCreateParameters(descriptor, avatarPath);

            // 대상 메뉴 결정
            VRCExpressionsMenu finalMenu = _targetMenu != null
                ? _targetMenu
                : GetOrCreateMenu(descriptor, avatarPath);

            var paramName = $"Cloth/{_toggleName}";

            // Int 파라미터 추가 (0 = Item1, 1 = Item2, ... N = All OFF)
            AddIntParameter(parameters, paramName, _defaultIndex);
            AddAnimatorIntParameter(fxController, paramName, _defaultIndex);

            // 애니메이션 클립 생성
            var clips = CreateClothClips(validItems, clipPath);

            // 레이어 생성
            CreateIntToggleLayer(fxController, paramName, clips, _defaultIndex, _addAllOff);

            // 메뉴 아이템 추가
            AddClothMenuItems(finalMenu, validItems, paramName);

            // 저장
            EditorUtility.SetDirty(fxController);
            EditorUtility.SetDirty(parameters);
            EditorUtility.SetDirty(finalMenu);
            AssetDatabase.SaveAssets();

            EditorUtility.DisplayDialog(
                L("dialog.complete"),
                L("cloth.apply_complete", validItems.Count),
                L("dialog.confirm"));
        }

        private void EnsureDirectoryExists(string path)
        {
            var folders = path.Split('/');
            var currentPath = folders[0];

            for (int i = 1; i < folders.Length; i++)
            {
                var nextPath = $"{currentPath}/{folders[i]}";
                if (!AssetDatabase.IsValidFolder(nextPath))
                {
                    AssetDatabase.CreateFolder(currentPath, folders[i]);
                }
                currentPath = nextPath;
            }
        }

        private AnimatorController GetOrCreateFXController(VRCAvatarDescriptor descriptor, string basePath)
        {
            var fxLayer = descriptor.baseAnimationLayers
                .FirstOrDefault(l => l.type == VRCAvatarDescriptor.AnimLayerType.FX);

            if (fxLayer.animatorController != null)
            {
                return fxLayer.animatorController as AnimatorController;
            }

            var path = $"{basePath}/FX_{_targetAvatar.name}.controller";
            var controller = AnimatorController.CreateAnimatorControllerAtPath(path);

            for (int i = 0; i < descriptor.baseAnimationLayers.Length; i++)
            {
                if (descriptor.baseAnimationLayers[i].type == VRCAvatarDescriptor.AnimLayerType.FX)
                {
                    descriptor.baseAnimationLayers[i].animatorController = controller;
                    descriptor.baseAnimationLayers[i].isDefault = false;
                    break;
                }
            }

            EditorUtility.SetDirty(descriptor);
            return controller;
        }

        private VRCExpressionParameters GetOrCreateParameters(VRCAvatarDescriptor descriptor, string basePath)
        {
            if (descriptor.expressionParameters != null)
            {
                return descriptor.expressionParameters;
            }

            var path = $"{basePath}/Parameters_{_targetAvatar.name}.asset";
            var parameters = ScriptableObject.CreateInstance<VRCExpressionParameters>();
            parameters.parameters = new VRCExpressionParameters.Parameter[0];
            AssetDatabase.CreateAsset(parameters, path);

            descriptor.expressionParameters = parameters;
            EditorUtility.SetDirty(descriptor);

            return parameters;
        }

        private VRCExpressionsMenu GetOrCreateMenu(VRCAvatarDescriptor descriptor, string basePath)
        {
            if (descriptor.expressionsMenu != null)
            {
                return descriptor.expressionsMenu;
            }

            var path = $"{basePath}/Menu_{_targetAvatar.name}.asset";
            var menu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
            menu.controls = new List<VRCExpressionsMenu.Control>();
            AssetDatabase.CreateAsset(menu, path);

            descriptor.expressionsMenu = menu;
            EditorUtility.SetDirty(descriptor);

            return menu;
        }

        private void AddIntParameter(VRCExpressionParameters parameters, string name, int defaultValue)
        {
            if (parameters.parameters.Any(p => p.name == name)) return;

            var list = parameters.parameters.ToList();
            list.Add(new VRCExpressionParameters.Parameter
            {
                name = name,
                valueType = VRCExpressionParameters.ValueType.Int,
                defaultValue = defaultValue,
                saved = _savedState,
                networkSynced = true
            });
            parameters.parameters = list.ToArray();
        }

        private void AddAnimatorIntParameter(AnimatorController controller, string name, int defaultValue)
        {
            if (controller.parameters.Any(p => p.name == name)) return;

            controller.AddParameter(name, AnimatorControllerParameterType.Int);

            var param = controller.parameters.First(p => p.name == name);
            param.defaultInt = defaultValue;
        }

        private List<AnimationClip> CreateClothClips(List<GameObject> items, string clipPath)
        {
            var clips = new List<AnimationClip>();

            // State 0~N-1: Each item ON (others OFF)
            for (int i = 0; i < items.Count; i++)
            {
                var clip = new AnimationClip { name = $"{_toggleName}_{items[i].name}" };

                for (int j = 0; j < items.Count; j++)
                {
                    var relativePath = GetRelativePath(_targetAvatar.transform, items[j].transform);
                    var value = (i == j) ? 1f : 0f;
                    var curve = new AnimationCurve(new Keyframe(0, value));
                    clip.SetCurve(relativePath, typeof(GameObject), "m_IsActive", curve);
                }

                AssetDatabase.CreateAsset(clip, $"{clipPath}/{_toggleName}_{items[i].name}.anim");
                clips.Add(clip);
            }

            // State N (마지막): All OFF (옵션이 켜져 있을 때만)
            if (_addAllOff)
            {
                var allOffClip = new AnimationClip { name = $"{_toggleName}_AllOFF" };
                foreach (var item in items)
                {
                    var relativePath = GetRelativePath(_targetAvatar.transform, item.transform);
                    var curve = new AnimationCurve(new Keyframe(0, 0));
                    allOffClip.SetCurve(relativePath, typeof(GameObject), "m_IsActive", curve);
                }

                AssetDatabase.CreateAsset(allOffClip, $"{clipPath}/{_toggleName}_AllOFF.anim");
                clips.Add(allOffClip);
            }

            return clips;
        }

        private string GetRelativePath(Transform root, Transform target)
        {
            var path = new List<string>();
            var current = target;

            while (current != null && current != root)
            {
                path.Insert(0, current.name);
                current = current.parent;
            }

            return string.Join("/", path);
        }

        private void CreateIntToggleLayer(AnimatorController controller, string paramName, List<AnimationClip> clips, int defaultIndex, bool addAllOff)
        {
            var layerName = paramName.Replace("/", "_");

            // 기존 레이어 제거
            var existingIndex = System.Array.FindIndex(controller.layers, l => l.name == layerName);
            if (existingIndex >= 0)
            {
                controller.RemoveLayer(existingIndex);
            }

            var layer = new AnimatorControllerLayer
            {
                name = layerName,
                defaultWeight = 1f,
                stateMachine = new AnimatorStateMachine
                {
                    name = layerName,
                    hideFlags = HideFlags.HideInHierarchy
                }
            };

            AssetDatabase.AddObjectToAsset(layer.stateMachine, controller);

            // Entry/Exit/Any State 위치 설정
            layer.stateMachine.entryPosition = new Vector3(-100, 0, 0);
            layer.stateMachine.exitPosition = new Vector3(-100, 60, 0);
            layer.stateMachine.anyStatePosition = new Vector3(150, 120, 0);

            // 상태 생성 - 세로 정렬
            var states = new List<AnimatorState>();
            var stateCount = clips.Count;

            for (int i = 0; i < stateCount; i++)
            {
                // All OFF 옵션이 켜져있고 마지막 클립인 경우
                var isAllOffState = addAllOff && (i == stateCount - 1);
                var stateName = isAllOffState ? "All OFF" : clips[i].name.Replace($"{_toggleName}_", "");

                // 기본 상태(0)는 Entry 옆에, 나머지는 Any State 오른쪽에 세로 배치
                float x, y;
                if (i == 0)
                {
                    x = 150;
                    y = 0;
                }
                else
                {
                    x = 400;
                    y = 60 + (i - 1) * 60;
                }

                var state = layer.stateMachine.AddState(stateName, new Vector3(x, y, 0));
                state.motion = clips[i];
                state.writeDefaultValues = true;
                states.Add(state);
            }

            // 기본 상태 설정
            if (defaultIndex >= 0 && defaultIndex < states.Count)
            {
                layer.stateMachine.defaultState = states[defaultIndex];
            }

            // Any State에서 각 상태로 트랜지션
            for (int i = 0; i < states.Count; i++)
            {
                var transition = layer.stateMachine.AddAnyStateTransition(states[i]);
                transition.hasExitTime = false;
                transition.duration = 0;
                transition.canTransitionToSelf = false;
                transition.AddCondition(AnimatorConditionMode.Equals, i, paramName);
            }

            controller.AddLayer(layer);
        }

        private void AddClothMenuItems(VRCExpressionsMenu menu, List<GameObject> items, string paramName)
        {
            // 기존 관련 메뉴 아이템 삭제 (같은 파라미터를 사용하는 것들)
            menu.controls.RemoveAll(c =>
                c.parameter != null && c.parameter.name == paramName);

            // 애니메이터 순서와 동일하게 메뉴 추가 (i=0은 기본 의상이므로 제외)
            for (int i = 1; i < items.Count; i++)
            {
                if (menu.controls.Count >= 8) break;

                menu.controls.Add(new VRCExpressionsMenu.Control
                {
                    name = items[i].name,
                    type = VRCExpressionsMenu.Control.ControlType.Toggle,
                    parameter = new VRCExpressionsMenu.Control.Parameter { name = paramName },
                    value = i
                });
            }

            // All OFF 옵션
            if (_addAllOff && menu.controls.Count < 8)
            {
                menu.controls.Add(new VRCExpressionsMenu.Control
                {
                    name = "All OFF",
                    type = VRCExpressionsMenu.Control.ControlType.Toggle,
                    parameter = new VRCExpressionsMenu.Control.Parameter { name = paramName },
                    value = items.Count
                });
            }
        }

        #endregion
    }
}
