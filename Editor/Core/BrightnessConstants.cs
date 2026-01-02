using UnityEditor;

namespace Brightness.Utility
{
    /// <summary>
    /// 밝기 조절 시스템에서 사용되는 모든 경로 및 상수 정의
    /// </summary>
    public static class BrightnessConstants
    {
        // 기본 경로 (Assets 또는 Packages 자동 감지)
        private const string ASSETS_PATH = "Assets/SodanenLightEditor/";
        private const string PACKAGES_PATH = "Packages/com.sodanen.sodanenlighteditor/";

        private static string _basePath;
        public static string BASE_PATH
        {
            get
            {
                if (_basePath == null)
                {
                    // Packages 경로에 있는지 먼저 확인
                    if (AssetDatabase.IsValidFolder(PACKAGES_PATH.TrimEnd('/')))
                        _basePath = PACKAGES_PATH;
                    else
                        _basePath = ASSETS_PATH;
                }
                return _basePath;
            }
        }

        public static string ANIMATION_PATH => BASE_PATH + "Animation/";
        public const string CREATE_PATH = "Assets/SodanenLightEditor/Created/";

        // 오브젝트 및 쉐이더 설정
        public const string OBJECT_NAME = "BrightnessControl";
        public const string SHADER_SHORT_NAME = "lil";

        // 에셋 경로
        public static string SETTINGS_ASSET_PATH => BASE_PATH + "BrightnessSetting.asset";
        public static string BRIGHTNESS_CONTROLLER_PATH => ANIMATION_PATH + "BrightnessController.controller";
        public static string DUMMY_ANIM_PATH => ANIMATION_PATH + "Dummy.anim";

        // 애니메이션 클립 경로
        public static string MIN_LIGHT_ANIM => ANIMATION_PATH + "MinLight.anim";
        public static string MAX_LIGHT_ANIM => ANIMATION_PATH + "MaxLight.anim";
        public static string BACK_LIGHT_ANIM => ANIMATION_PATH + "BackLight.anim";
        public static string SHADOW_ANGLE_ANIM => ANIMATION_PATH + "Shadow_Angle.anim";
        public static string SHADOW_XANGLE_ANIM => ANIMATION_PATH + "Shadow_XAngle.anim";
        public static string SHADOW_YANGLE_ANIM => ANIMATION_PATH + "Shadow_YAngle.anim";

        // 쉐이더 프로퍼티 이름
        public static class ShaderProperties
        {
            // Light
            public const string LIGHT_MIN_LIMIT = "_LightMinLimit";
            public const string LIGHT_MAX_LIMIT = "_LightMaxLimit";
            public const string BACKLIGHT_BORDER = "_BacklightBorder";
            public const string LIGHT_DIRECTION = "_LightDirection";

            // Shadow Master
            public const string USE_SHADOW = "_UseShadow";
            public const string SHADOW_MASK_TYPE = "_ShadowMaskType";  // 마스크 타입 (0=강도)
            public const string SHADOW_STRENGTH = "_ShadowStrength";   // 마스크와 강도
            public const string SHADOW_STRENGTH_MASK = "_ShadowStrengthMask";
            public const string SHADOW_STRENGTH_MASK_LOD = "_ShadowStrengthMaskLOD";  // LOD
            public const string SHADOW_COLOR_TYPE = "_ShadowColorType";  // Color Type (0=Normal, 1=LUT)

            // Shadow 1st
            public const string SHADOW_COLOR = "_ShadowColor";  // 그림자 색 1 색상
            public const string SHADOW_BORDER = "_ShadowBorder";
            public const string SHADOW_BLUR = "_ShadowBlur";
            public const string SHADOW_NORMAL_STRENGTH = "_ShadowNormalStrength";
            public const string SHADOW_RECEIVE = "_ShadowReceive";

            // Shadow 2nd
            public const string SHADOW_2ND_COLOR = "_Shadow2ndColor";  // 그림자 색 2 (Color + Alpha)
            public const string SHADOW_2ND_BORDER = "_Shadow2ndBorder";
            public const string SHADOW_2ND_BLUR = "_Shadow2ndBlur";
            public const string SHADOW_2ND_NORMAL_STRENGTH = "_Shadow2ndNormalStrength";
            public const string SHADOW_2ND_RECEIVE = "_Shadow2ndReceive";

            // Shadow 3rd
            public const string SHADOW_3RD_COLOR = "_Shadow3rdColor";  // 그림자 색 3 (Color + Alpha)

            // Shadow Additional
            public const string SHADOW_BORDER_COLOR = "_ShadowBorderColor";
            public const string SHADOW_BORDER_RANGE = "_ShadowBorderRange";
            public const string SHADOW_MAIN_STRENGTH = "_ShadowMainStrength";
            public const string SHADOW_ENV_STRENGTH = "_ShadowEnvStrength";
            public const string SHADOW_CASTER_BIAS = "_lilShadowCasterBias";

            // Shadow Blur Mask (흐림 효과 마스크)
            public const string SHADOW_BLUR_MASK = "_ShadowBlurMask";
            public const string SHADOW_BLUR_MASK_LOD = "_ShadowBlurMaskLOD";

            // Shadow AO Map
            public const string SHADOW_AO_MAP = "_ShadowAOMask";
            public const string SHADOW_AO_MAP_LOD = "_ShadowAOMaskLOD";
            public const string SHADOW_AO_MAP_2ND = "_ShadowAOMask2nd";
            public const string SHADOW_AO_MAP_2ND_LOD = "_ShadowAOMask2ndLOD";
        }

        // 파라미터 이름
        public static class Parameters
        {
            public const string MAX_LIGHT = "MaxLight";
            public const string MIN_LIGHT = "MinLight";
            public const string BACK_LIGHT = "BackLight";
            public const string SHADOW = "Shadow";
            public const string TOGGLE_ANGLE = "Toggle_Angle";
            public const string SHADOW_XANGLE = "Shadow_XAngle";
            public const string SHADOW_YANGLE = "Shadow_YAngle";
        }

        // 레이어 이름
        public static class Layers
        {
            public const string MIN_LIGHT = "MinLight";
            public const string MAX_LIGHT = "MaxLight";
            public const string BACK_LIGHT = "BackLight";
            public const string SHADOW = "Shadow";
            public const string SHADOW_XANGLE = "Shadow_XAngle";
            public const string SHADOW_YANGLE = "Shadow_YAngle";
        }

        // 기본값
        public static class Defaults
        {
            public const float PARAMETER_DEFAULT = 0.5f;
            public const float LOWER_BRIGHTNESS_LIMIT = 0.0f;
            public const float UPPER_BRIGHTNESS_LIMIT = 3.0f;
            public const float BACKLIGHT_STRENGTH = 0.5f;
            public const float SHADOW_ANGLE = 0.0f;
        }
    }
}
