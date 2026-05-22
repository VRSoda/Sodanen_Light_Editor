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

        // 쉐이더 설정
        public const string SHADER_SHORT_NAME = "lil";

        // 에셋 경로
        public static string BRIGHTNESS_CONTROLLER_PATH => ANIMATION_PATH + "BrightnessController.controller";
        public static string DUMMY_ANIM_PATH => ANIMATION_PATH + "Dummy.anim";

        // 애니메이션 클립 경로 (그림자 각도는 기존 클립을 복사해서 사용)
        public static string SHADOW_XANGLE_ANIM => ANIMATION_PATH + "Shadow_XAngle.anim";
        public static string SHADOW_YANGLE_ANIM => ANIMATION_PATH + "Shadow_YAngle.anim";

        // 파라미터 이름
        public static class Parameters
        {
            public const string MAX_LIGHT = "MaxLight";
            public const string MIN_LIGHT = "MinLight";
            public const string BACK_LIGHT = "BackLight";
            public const string BACK_LIGHT_HUE = "BackLightHue";
            public const string SHADOW = "Shadow";
            public const string TOGGLE_ANGLE = "Toggle_Angle";
            public const string SHADOW_XANGLE = "Shadow_XAngle";
            public const string SHADOW_YANGLE = "Shadow_YAngle";
        }

        // 기본값
        public static class Defaults
        {
            public const float PARAMETER_DEFAULT = 0.5f;
        }
    }
}
