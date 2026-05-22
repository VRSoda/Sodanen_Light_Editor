using Sodanen.LightControl;
using UnityEditor;
using UnityEngine;

namespace Brightness.Utility
{
    [CustomEditor(typeof(SodanenLightControl))]
    public class SodanenLightControlEditor : Editor
    {
        private const string LogoPath = "Packages/com.sodanen.sodanenlighteditor/Editor/Images/SodanenLightControl_Logo.png";
        private const string BacklightDetailsPrefKey = "Sodanen.LightControl.ShowBacklightDetails";

        private SerializedProperty _enableMinLight;
        private SerializedProperty _enableMaxLight;
        private SerializedProperty _enableBackLight;
        private SerializedProperty _enableShadow;
        private SerializedProperty _enableShadowXAngle;
        private SerializedProperty _enableShadowYAngle;

        private SerializedProperty _minLightRange;
        private SerializedProperty _maxLightRange;
        private SerializedProperty _backLightColor;
        private SerializedProperty _enableBackLightColorChange;
        private SerializedProperty _backLightAlpha;
        private SerializedProperty _backLightMainStrength;
        private SerializedProperty _backLightReceiveShadow;
        private SerializedProperty _backLightBackfaceMask;
        private SerializedProperty _backLightNormalStrength;
        private SerializedProperty _backLightBorder;
        private SerializedProperty _backLightBlur;
        private SerializedProperty _backLightDirectivity;
        private SerializedProperty _backLightViewStrength;
        private SerializedProperty _shadowRange;
        private SerializedProperty _shadowOverrides;

        private GUIStyle _sectionStyle;
        private GUIStyle _sectionTitleStyle;
        private GUIStyle _featureTitleStyle;
        private GUIStyle _mutedStyle;
        private Texture2D _logoTexture;
        private bool _stylesInitialized;
        private bool _showBacklightDetails;

        private void OnEnable()
        {
            _enableMinLight = serializedObject.FindProperty("enableMinLight");
            _enableMaxLight = serializedObject.FindProperty("enableMaxLight");
            _enableBackLight = serializedObject.FindProperty("enableBackLight");
            _enableShadow = serializedObject.FindProperty("enableShadow");
            _enableShadowXAngle = serializedObject.FindProperty("enableShadowXAngle");
            _enableShadowYAngle = serializedObject.FindProperty("enableShadowYAngle");

            _minLightRange = serializedObject.FindProperty("minLightRange");
            _maxLightRange = serializedObject.FindProperty("maxLightRange");
            _backLightColor = serializedObject.FindProperty("backLightColor");
            _enableBackLightColorChange = serializedObject.FindProperty("enableBackLightColorChange");
            _backLightAlpha = serializedObject.FindProperty("backLightAlpha");
            _backLightMainStrength = serializedObject.FindProperty("backLightMainStrength");
            _backLightReceiveShadow = serializedObject.FindProperty("backLightReceiveShadow");
            _backLightBackfaceMask = serializedObject.FindProperty("backLightBackfaceMask");
            _backLightNormalStrength = serializedObject.FindProperty("backLightNormalStrength");
            _backLightBorder = serializedObject.FindProperty("backLightBorder");
            _backLightBlur = serializedObject.FindProperty("backLightBlur");
            _backLightDirectivity = serializedObject.FindProperty("backLightDirectivity");
            _backLightViewStrength = serializedObject.FindProperty("backLightViewStrength");
            _shadowRange = serializedObject.FindProperty("shadowRange");
            _shadowOverrides = serializedObject.FindProperty("shadowOverrides");
            _logoTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(LogoPath);
            _showBacklightDetails = EditorPrefs.GetBool(BacklightDetailsPrefKey, false);
        }

        private void InitStyles()
        {
            if (_stylesInitialized) return;

            _sectionStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(10, 10, 8, 9),
                margin = new RectOffset(0, 0, 4, 6)
            };

            _sectionTitleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 12
            };

            _featureTitleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 11
            };

            _mutedStyle = new GUIStyle(EditorStyles.miniLabel)
            {
                wordWrap = true
            };

            _stylesInitialized = true;
        }

        public override void OnInspectorGUI()
        {
            InitStyles();
            serializedObject.Update();

            var control = (SodanenLightControl)target;
            var avatarRoot = control.GetAvatarRoot();

            DrawLogo();

            DrawSection("라이팅", () =>
            {
                DrawRangeFeature(
                    "밝기 상한",
                    "밝은 월드에서 아바타가 과하게 하얗게 뜨는 것을 제한합니다.",
                    _enableMaxLight,
                    _maxLightRange,
                    0f,
                    10f);

                DrawRangeFeature(
                    "밝기 하한",
                    "어두운 월드에서 아바타가 너무 묻히지 않도록 최소 밝기를 확보합니다.",
                    _enableMinLight,
                    _minLightRange,
                    0f,
                    1f);

                DrawBacklightFeature();
            });

            DrawSection("그림자", () =>
            {
                DrawShadowFeature();
                DrawToggleFeature(
                    "그림자 X 각도",
                    "기존 X축 그림자 방향 애니메이션을 EX 메뉴에 추가합니다.",
                    _enableShadowXAngle);
                DrawToggleFeature(
                    "그림자 Y 각도",
                    "기존 Y축 그림자 방향 애니메이션을 EX 메뉴에 추가합니다.",
                    _enableShadowYAngle);
            });

            DrawBuildInfo(control, avatarRoot);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawLogo()
        {
            if (_logoTexture == null) return;

            EditorGUILayout.Space(4);
            var availableWidth = Mathf.Max(1f, EditorGUIUtility.currentViewWidth - 36f);
            var aspect = (float)_logoTexture.height / _logoTexture.width;
            var height = Mathf.Clamp(availableWidth * aspect, 38f, 76f);
            var rect = GUILayoutUtility.GetRect(availableWidth, height, GUILayout.ExpandWidth(true));

            var imageWidth = Mathf.Min(rect.width, height / aspect);
            var imageRect = new Rect(rect.x + (rect.width - imageWidth) / 2f, rect.y, imageWidth, height);

            GUI.DrawTexture(imageRect, _logoTexture, ScaleMode.ScaleToFit, true);
            EditorGUILayout.Space(2);
        }

        private void DrawSection(string title, System.Action content)
        {
            EditorGUILayout.BeginVertical(_sectionStyle);
            EditorGUILayout.LabelField(title, _sectionTitleStyle);
            EditorGUILayout.Space(3);
            content.Invoke();
            EditorGUILayout.EndVertical();
        }

        private void DrawRangeFeature(
            string title,
            string description,
            SerializedProperty enabledProp,
            SerializedProperty rangeProp,
            float min,
            float max)
        {
            DrawFeatureHeader(title, description, enabledProp);
            using (new EditorGUI.DisabledScope(!enabledProp.boolValue))
            {
                DrawMinMaxSlider(rangeProp, min, max);
            }
            EditorGUILayout.Space(5);
        }

        private void DrawBacklightFeature()
        {
            DrawFeatureHeader(
                "백라이트",
                "lilToon 백라이트 값을 EX 메뉴용 애니메이션에 적용합니다.",
                _enableBackLight);

            using (new EditorGUI.DisabledScope(!_enableBackLight.boolValue))
            {
                using (new EditorGUI.DisabledScope(_enableBackLightColorChange.boolValue))
                {
                    DrawColorRow("색상", _backLightColor);
                }
                _enableBackLightColorChange.boolValue = EditorGUILayout.ToggleLeft(
                    "색상 변경 (EX 메뉴 무지개)", _enableBackLightColorChange.boolValue);
                if (_enableBackLightColorChange.boolValue)
                {
                    EditorGUILayout.LabelField("EX 메뉴 라디얼로 색을 조절합니다. 0=흰색 → 무지개. (위 색상값은 무시)", _mutedStyle);
                }

                DrawSliderRow("투명도", _backLightAlpha, 0f, 1f);
                DrawSliderRow("메인컬러 강도", _backLightMainStrength, 0f, 1f);

                EditorGUILayout.PropertyField(_backLightReceiveShadow, new GUIContent("그림자를 받는"));
                EditorGUILayout.PropertyField(_backLightBackfaceMask, new GUIContent("후면 무효화"));

                EditorGUILayout.Space(2);
                EditorGUI.indentLevel++;
                EditorGUI.BeginChangeCheck();
                _showBacklightDetails = EditorGUILayout.Foldout(_showBacklightDetails, "상세 설정", true);
                if (EditorGUI.EndChangeCheck())
                    EditorPrefs.SetBool(BacklightDetailsPrefKey, _showBacklightDetails);
                EditorGUI.indentLevel--;
                if (_showBacklightDetails)
                {
                    DrawSliderRow("노멀 맵 강도", _backLightNormalStrength, 0f, 1f);
                    DrawSliderRow("범위", _backLightBorder, 0f, 1f);
                    DrawSliderRow("흐리게", _backLightBlur, 0f, 1f);
                    DrawFloatFieldRow("지향성", _backLightDirectivity);
                    DrawSliderRow("뷰 방향의 영향도", _backLightViewStrength, 0f, 1f);
                }
            }

            EditorGUILayout.Space(5);
        }

        private void DrawSliderRow(string label, SerializedProperty property, float min, float max)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(104));
            property.floatValue = EditorGUILayout.Slider(property.floatValue, min, max);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawFloatFieldRow(string label, SerializedProperty property)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(104));
            property.floatValue = EditorGUILayout.FloatField(property.floatValue);
            EditorGUILayout.EndHorizontal();
        }

        private void DrawColorRow(string label, SerializedProperty property)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.Width(104));
            property.colorValue = EditorGUILayout.ColorField(GUIContent.none, property.colorValue, true, false, false);

            var currentHex = "#" + ColorUtility.ToHtmlStringRGB(property.colorValue);
            var newHex = EditorGUILayout.DelayedTextField(currentHex, GUILayout.Width(76));
            if (newHex != currentHex && ColorUtility.TryParseHtmlString(newHex, out var parsed))
            {
                parsed.a = property.colorValue.a;
                property.colorValue = parsed;
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawToggleFeature(string title, string description, SerializedProperty enabledProp)
        {
            DrawFeatureHeader(title, description, enabledProp);
            EditorGUILayout.Space(3);
        }

        private void DrawShadowFeature()
        {
            DrawFeatureHeader(
                "그림자 강도",
                "lilToon 그림자 강도를 EX 메뉴에서 조절합니다. 필요하면 렌더러별 범위를 따로 줄 수 있습니다.",
                _enableShadow);

            using (new EditorGUI.DisabledScope(!_enableShadow.boolValue))
            {
                EditorGUILayout.LabelField("기본 범위", _mutedStyle);
                DrawMinMaxSlider(_shadowRange, 0f, 1f);

                EditorGUILayout.Space(4);
                DrawOverrideList();
            }

            EditorGUILayout.Space(5);
        }

        private void DrawFeatureHeader(string title, string description, SerializedProperty enabledProp)
        {
            EditorGUILayout.BeginHorizontal();
            enabledProp.boolValue = EditorGUILayout.Toggle(enabledProp.boolValue, GUILayout.Width(18));
            EditorGUILayout.LabelField(title, _featureTitleStyle);
            EditorGUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(description))
            {
                EditorGUILayout.LabelField(description, _mutedStyle);
            }
        }

        private void DrawOverrideList()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("렌더러별 오버라이드", _mutedStyle);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("추가", EditorStyles.miniButton, GUILayout.Width(52)))
            {
                _shadowOverrides.InsertArrayElementAtIndex(_shadowOverrides.arraySize);
                var element = _shadowOverrides.GetArrayElementAtIndex(_shadowOverrides.arraySize - 1);
                element.FindPropertyRelative("targetRenderer").objectReferenceValue = null;
                element.FindPropertyRelative("shadowRange").vector2Value = _shadowRange.vector2Value;
            }
            EditorGUILayout.EndHorizontal();

            if (_shadowOverrides.arraySize == 0)
            {
                EditorGUILayout.LabelField("개별 렌더러 설정 없음", _mutedStyle);
                return;
            }

            for (int i = 0; i < _shadowOverrides.arraySize; i++)
            {
                var element = _shadowOverrides.GetArrayElementAtIndex(i);
                var rendererProp = element.FindPropertyRelative("targetRenderer");
                var rangeProp = element.FindPropertyRelative("shadowRange");

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(rendererProp, GUIContent.none);
                if (GUILayout.Button("삭제", EditorStyles.miniButton, GUILayout.Width(48)))
                {
                    _shadowOverrides.DeleteArrayElementAtIndex(i);
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    break;
                }
                EditorGUILayout.EndHorizontal();

                if (rendererProp.objectReferenceValue == null)
                {
                    EditorGUILayout.LabelField("렌더러를 지정하면 개별 범위를 설정할 수 있습니다.", _mutedStyle);
                }
                else
                {
                    DrawMinMaxSlider(rangeProp, 0f, 1f);
                }
                EditorGUILayout.EndVertical();
            }
        }

        private void DrawMinMaxSlider(SerializedProperty rangeProp, float min, float max)
        {
            var range = rangeProp.vector2Value;
            var minVal = range.x;
            var maxVal = range.y;

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Min", GUILayout.Width(28));
            minVal = EditorGUILayout.FloatField(minVal, GUILayout.Width(54));
            EditorGUILayout.MinMaxSlider(ref minVal, ref maxVal, min, max);
            EditorGUILayout.LabelField("Max", GUILayout.Width(30));
            maxVal = EditorGUILayout.FloatField(maxVal, GUILayout.Width(54));
            EditorGUILayout.EndHorizontal();

            minVal = Mathf.Clamp(minVal, min, max);
            maxVal = Mathf.Clamp(maxVal, min, max);
            if (minVal > maxVal) minVal = maxVal;

            rangeProp.vector2Value = new Vector2(minVal, maxVal);
        }

        private void DrawBuildInfo(SodanenLightControl control, GameObject avatarRoot)
        {
            if (avatarRoot == null)
            {
                EditorGUILayout.HelpBox("VRCAvatarDescriptor를 찾을 수 없습니다. 이 오브젝트를 아바타 루트 하위에 배치해주세요.", MessageType.Warning);
                return;
            }

            if (!control.HasAnyFeatureEnabled())
            {
                EditorGUILayout.HelpBox("활성화된 기능이 없습니다. 최소 하나 이상 켜야 빌드 시 메뉴가 생성됩니다.", MessageType.Warning);
                return;
            }

            EditorGUILayout.HelpBox($"대상 아바타: {avatarRoot.name}\n활성화된 기능 {GetEnabledCount(control)}개가 업로드 또는 NDMF 빌드 시 자동 적용됩니다.", MessageType.Info);
        }

        private int GetEnabledCount(SodanenLightControl control)
        {
            var count = 0;
            if (control.enableMaxLight) count++;
            if (control.enableMinLight) count++;
            if (control.enableBackLight) count++;
            if (control.enableShadow) count++;
            if (control.enableShadowXAngle) count++;
            if (control.enableShadowYAngle) count++;
            return count;
        }
    }

    [CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
    public class MinMaxRangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = (MinMaxRangeAttribute)attribute;
            if (property.propertyType != SerializedPropertyType.Vector2)
            {
                EditorGUI.PropertyField(position, property, label);
                return;
            }

            var range = property.vector2Value;
            var minVal = range.x;
            var maxVal = range.y;

            var fieldWidth = 50f;
            var padding = 5f;
            var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
            var minRect = new Rect(labelRect.xMax + padding, position.y, fieldWidth, position.height);
            var sliderRect = new Rect(minRect.xMax + padding, position.y, position.width - labelRect.width - fieldWidth * 2 - padding * 4, position.height);
            var maxRect = new Rect(sliderRect.xMax + padding, position.y, fieldWidth, position.height);

            EditorGUI.LabelField(labelRect, label);
            minVal = EditorGUI.FloatField(minRect, minVal);
            EditorGUI.MinMaxSlider(sliderRect, ref minVal, ref maxVal, attr.Min, attr.Max);
            maxVal = EditorGUI.FloatField(maxRect, maxVal);

            minVal = Mathf.Clamp(minVal, attr.Min, attr.Max);
            maxVal = Mathf.Clamp(maxVal, attr.Min, attr.Max);
            if (minVal > maxVal) minVal = maxVal;

            property.vector2Value = new Vector2(minVal, maxVal);
        }
    }
}
