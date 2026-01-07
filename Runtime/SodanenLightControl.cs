using System;
using System.Collections.Generic;
using UnityEngine;
using VRC.SDKBase;

namespace Sodanen.LightControl
{
    /// <summary>
    /// 머티리얼별 Shadow 오버라이드 설정
    /// </summary>
    [Serializable]
    public class ShadowOverride
    {
        [Tooltip("오버라이드할 Renderer")]
        public Renderer targetRenderer;

        [Tooltip("Shadow 범위")]
        [MinMaxRange(0f, 1f)]
        public Vector2 shadowRange = new Vector2(0f, 1f);
    }

    /// <summary>
    /// Prefab에 추가하여 NDMF 빌드 시 자동으로 조명 조절 기능을 적용하는 컴포넌트
    /// </summary>
    [AddComponentMenu("Sodanen/Light Control")]
    [DisallowMultipleComponent]
    public class SodanenLightControl : MonoBehaviour, IEditorOnly
    {
        [Header("Max Light")]
        [Tooltip("최대 밝기 조절 기능 활성화")]
        public bool enableMaxLight = true;

        [Tooltip("최대 밝기 범위")]
        [MinMaxRange(0f, 10f)]
        public Vector2 maxLightRange = new Vector2(0f, 1f);

        [Header("Min Light")]
        [Tooltip("최소 밝기 조절 기능 활성화")]
        public bool enableMinLight = true;

        [Tooltip("최소 밝기 범위 (라디얼 0~1 → 이 범위로 매핑)")]
        [MinMaxRange(0f, 1f)]
        public Vector2 minLightRange = new Vector2(0f, 1f);

        [Header("Back Light")]
        [Tooltip("역광 조절 기능 활성화")]
        public bool enableBackLight = true;

        [Tooltip("역광 값 (0~1)")]
        [Range(0f, 1f)]
        public float backLightValue = 0.35f;

        [Header("Shadow")]
        [Tooltip("그림자 강도 조절 기능 활성화")]
        public bool enableShadow = true;

        [Tooltip("그림자 강도 범위 (기본값)")]
        [MinMaxRange(0f, 1f)]
        public Vector2 shadowRange = new Vector2(0f, 1f);

        [Tooltip("특정 Renderer에 다른 Shadow 값 적용")]
        public List<ShadowOverride> shadowOverrides = new List<ShadowOverride>();

        [Header("Shadow Angle")]
        [Tooltip("그림자 X축 각도 조절 (기존 애니메이션 사용)")]
        public bool enableShadowXAngle = true;

        [Tooltip("그림자 Y축 각도 조절 (기존 애니메이션 사용)")]
        public bool enableShadowYAngle = true;

        /// <summary>
        /// 활성화된 기능이 하나라도 있는지 확인
        /// </summary>
        public bool HasAnyFeatureEnabled()
        {
            return enableMinLight || enableMaxLight || enableBackLight ||
                   enableShadow || enableShadowXAngle || enableShadowYAngle;
        }

        /// <summary>
        /// 아바타 루트를 찾아 반환
        /// </summary>
        public GameObject GetAvatarRoot()
        {
            var current = transform;
            while (current != null)
            {
                if (current.GetComponent<VRC_AvatarDescriptor>() != null)
                    return current.gameObject;
                current = current.parent;
            }
            return null;
        }
    }

    /// <summary>
    /// MinMax 범위 속성
    /// </summary>
    public class MinMaxRangeAttribute : PropertyAttribute
    {
        public float Min { get; }
        public float Max { get; }

        public MinMaxRangeAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}
