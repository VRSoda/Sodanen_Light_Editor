using Sodanen.LightControl;
using UnityEditor;
using UnityEngine;

namespace Brightness.Utility
{
    /// <summary>
    /// SodanenLightControl 컴포넌트의 인스펙터 UI
    /// </summary>
    [CustomEditor(typeof(SodanenLightControl))]
    public class SodanenLightControlEditor : Editor
    {
        // Feature toggles
        private SerializedProperty _enableMinLight;
        private SerializedProperty _enableMaxLight;
        private SerializedProperty _enableBackLight;
        private SerializedProperty _enableShadow;
        private SerializedProperty _enableShadowXAngle;
        private SerializedProperty _enableShadowYAngle;

        // Range values
        private SerializedProperty _minLightRange;
        private SerializedProperty _maxLightRange;
        private SerializedProperty _backLightValue;
        private SerializedProperty _shadowRange;
        private SerializedProperty _shadowOverrides;

        // 스타일 캐시
        private GUIStyle _headerStyle;
        private GUIStyle _sectionStyle;
        private GUIStyle _toggleStyle;
        private bool _stylesInitialized;

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
            _backLightValue = serializedObject.FindProperty("backLightValue");
            _shadowRange = serializedObject.FindProperty("shadowRange");
            _shadowOverrides = serializedObject.FindProperty("shadowOverrides");
        }

        private void InitStyles()
        {
            if (_stylesInitialized) return;

            _headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 14,
                alignment = TextAnchor.MiddleCenter,
                padding = new RectOffset(0, 0, 5, 5)
            };

            _sectionStyle = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(8, 8, 6, 6)
            };

            _toggleStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 11
            };

            _stylesInitialized = true;
        }

        public override void OnInspectorGUI()
        {
            InitStyles();
            serializedObject.Update();

            var control = (SodanenLightControl)target;

            // 헤더
            EditorGUILayout.Space(6);
            DrawHeader();
            EditorGUILayout.Space(6);

            // 아바타 확인
            var avatarRoot = control.GetAvatarRoot();
            if (avatarRoot == null)
            {
                EditorGUILayout.HelpBox("VRCAvatarDescriptor를 찾을 수 없습니다.\n아바타 하위에 배치해주세요.", MessageType.Warning);
                EditorGUILayout.Space(6);
            }

            // Light Settings
            DrawSectionHeader("Light Settings");
            DrawFeatureSection("Max Light", _enableMaxLight, _maxLightRange, 0f, 10f);
            DrawFeatureSection("Min Light", _enableMinLight, _minLightRange, 0f, 1f);
            DrawValueSection("Back Light", _enableBackLight, _backLightValue, 0f, 1f);

            EditorGUILayout.Space(4);

            // Shadow Settings
            DrawSectionHeader("Shadow Settings");
            DrawShadowSection();
            DrawToggleSection("Shadow X Angle", _enableShadowXAngle);
            DrawToggleSection("Shadow Y Angle", _enableShadowYAngle);

            EditorGUILayout.Space(8);

            // 상태 표시
            DrawStatusBox(control);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawHeader()
        {
            var rect = EditorGUILayout.GetControlRect(false, 28);
            var bgColor = EditorGUIUtility.isProSkin
                ? new Color(0.22f, 0.22f, 0.22f)
                : new Color(0.76f, 0.76f, 0.76f);
            EditorGUI.DrawRect(rect, bgColor);

            GUI.Label(rect, "Sodanen Light Control", _headerStyle);
        }

        private void DrawSectionHeader(string title)
        {
            EditorGUILayout.Space(2);
            var labelStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize = 11,
                normal = { textColor = EditorGUIUtility.isProSkin
                    ? new Color(0.7f, 0.85f, 1f)
                    : new Color(0.2f, 0.4f, 0.6f) }
            };
            EditorGUILayout.LabelField(title, labelStyle);
            EditorGUILayout.Space(2);
        }

        private void DrawFeatureSection(string label, SerializedProperty enableProp, SerializedProperty rangeProp, float min, float max)
        {
            EditorGUILayout.BeginVertical(_sectionStyle);

            EditorGUILayout.BeginHorizontal();
            enableProp.boolValue = EditorGUILayout.Toggle(enableProp.boolValue, GUILayout.Width(18));
            EditorGUILayout.LabelField(label, _toggleStyle);
            EditorGUILayout.EndHorizontal();

            if (enableProp.boolValue)
            {
                EditorGUILayout.Space(4);
                DrawMinMaxSlider(rangeProp, min, max);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);
        }

        private void DrawValueSection(string label, SerializedProperty enableProp, SerializedProperty valueProp, float min, float max)
        {
            EditorGUILayout.BeginVertical(_sectionStyle);

            EditorGUILayout.BeginHorizontal();
            enableProp.boolValue = EditorGUILayout.Toggle(enableProp.boolValue, GUILayout.Width(18));
            EditorGUILayout.LabelField(label, _toggleStyle);
            EditorGUILayout.EndHorizontal();

            if (enableProp.boolValue)
            {
                EditorGUILayout.Space(4);
                EditorGUILayout.BeginHorizontal();
                valueProp.floatValue = EditorGUILayout.Slider(valueProp.floatValue, min, max);
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);
        }

        private void DrawToggleSection(string label, SerializedProperty enableProp)
        {
            EditorGUILayout.BeginVertical(_sectionStyle);

            EditorGUILayout.BeginHorizontal();
            enableProp.boolValue = EditorGUILayout.Toggle(enableProp.boolValue, GUILayout.Width(18));
            EditorGUILayout.LabelField(label, _toggleStyle);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);
        }

        private void DrawShadowSection()
        {
            EditorGUILayout.BeginVertical(_sectionStyle);

            // 메인 토글
            EditorGUILayout.BeginHorizontal();
            _enableShadow.boolValue = EditorGUILayout.Toggle(_enableShadow.boolValue, GUILayout.Width(18));
            EditorGUILayout.LabelField("Shadow Strength", _toggleStyle);
            EditorGUILayout.EndHorizontal();

            if (_enableShadow.boolValue)
            {
                EditorGUILayout.Space(4);

                // 기본 범위
                EditorGUILayout.LabelField("Default Range", EditorStyles.miniLabel);
                DrawMinMaxSlider(_shadowRange, 0f, 1f);

                EditorGUILayout.Space(6);

                // 오버라이드 리스트
                EditorGUILayout.LabelField("Renderer Overrides", EditorStyles.miniLabel);

                for (int i = 0; i < _shadowOverrides.arraySize; i++)
                {
                    var element = _shadowOverrides.GetArrayElementAtIndex(i);
                    var rendererProp = element.FindPropertyRelative("targetRenderer");
                    var rangeProp = element.FindPropertyRelative("shadowRange");

                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(rendererProp, GUIContent.none);
                    if (GUILayout.Button("X", GUILayout.Width(20)))
                    {
                        _shadowOverrides.DeleteArrayElementAtIndex(i);
                        break;
                    }
                    EditorGUILayout.EndHorizontal();

                    if (rendererProp.objectReferenceValue != null)
                    {
                        DrawMinMaxSlider(rangeProp, 0f, 1f);
                    }

                    EditorGUILayout.EndVertical();
                }

                EditorGUILayout.Space(2);
                if (GUILayout.Button("+ Add Override"))
                {
                    _shadowOverrides.InsertArrayElementAtIndex(_shadowOverrides.arraySize);
                }
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space(2);
        }

        private void DrawMinMaxSlider(SerializedProperty rangeProp, float min, float max)
        {
            var range = rangeProp.vector2Value;
            float minVal = range.x;
            float maxVal = range.y;

            EditorGUILayout.BeginHorizontal();

            // Min 라벨 + 필드
            EditorGUILayout.LabelField("Min", GUILayout.Width(28));
            minVal = EditorGUILayout.FloatField(minVal, GUILayout.Width(40));

            // 슬라이더
            EditorGUILayout.MinMaxSlider(ref minVal, ref maxVal, min, max);

            // Max 라벨 + 필드
            EditorGUILayout.LabelField("Max", GUILayout.Width(28));
            maxVal = EditorGUILayout.FloatField(maxVal, GUILayout.Width(40));

            EditorGUILayout.EndHorizontal();

            // 값 클램프 및 저장
            minVal = Mathf.Clamp(minVal, min, max);
            maxVal = Mathf.Clamp(maxVal, min, max);
            if (minVal > maxVal) minVal = maxVal;

            rangeProp.vector2Value = new Vector2(minVal, maxVal);
        }

        private void DrawStatusBox(SodanenLightControl control)
        {
            if (!control.HasAnyFeatureEnabled())
            {
                EditorGUILayout.HelpBox("활성화된 기능이 없습니다. 최소 하나의 기능을 활성화해주세요.", MessageType.Warning);
            }
            else
            {
                // 활성화된 기능 수 계산
                int count = 0;
                if (control.enableMinLight) count++;
                if (control.enableMaxLight) count++;
                if (control.enableBackLight) count++;
                if (control.enableShadow) count++;
                if (control.enableShadowXAngle) count++;
                if (control.enableShadowYAngle) count++;

                EditorGUILayout.HelpBox($"{count}개 기능 활성화됨 - Play Mode 또는 업로드 시 자동 적용", MessageType.Info);
            }
        }
    }

    /// <summary>
    /// MinMaxRange 속성을 위한 PropertyDrawer
    /// </summary>
    [CustomPropertyDrawer(typeof(MinMaxRangeAttribute))]
    public class MinMaxRangeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = (MinMaxRangeAttribute)attribute;

            if (property.propertyType == SerializedPropertyType.Vector2)
            {
                var range = property.vector2Value;
                float minVal = range.x;
                float maxVal = range.y;

                float fieldWidth = 50f;
                float padding = 5f;

                // 라벨
                var labelRect = new Rect(position.x, position.y, EditorGUIUtility.labelWidth, position.height);
                EditorGUI.LabelField(labelRect, label);

                // Min 필드
                var minRect = new Rect(labelRect.xMax + padding, position.y, fieldWidth, position.height);
                minVal = EditorGUI.FloatField(minRect, minVal);

                // 슬라이더
                var sliderRect = new Rect(minRect.xMax + padding, position.y, position.width - labelRect.width - fieldWidth * 2 - padding * 4, position.height);
                EditorGUI.MinMaxSlider(sliderRect, ref minVal, ref maxVal, attr.Min, attr.Max);

                // Max 필드
                var maxRect = new Rect(sliderRect.xMax + padding, position.y, fieldWidth, position.height);
                maxVal = EditorGUI.FloatField(maxRect, maxVal);

                // 값 클램프
                minVal = Mathf.Clamp(minVal, attr.Min, attr.Max);
                maxVal = Mathf.Clamp(maxVal, attr.Min, attr.Max);
                if (minVal > maxVal) minVal = maxVal;

                property.vector2Value = new Vector2(minVal, maxVal);
            }
            else
            {
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
}
