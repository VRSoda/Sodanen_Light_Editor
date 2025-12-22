using UnityEditor.Animations;
using UnityEngine;
using System;
using System.Collections.Generic;

namespace Brightness.Utility
{
    /// <summary>
    /// Brightness Control 시스템의 메인 파사드 클래스
    /// 각 Helper 클래스에 위임하여 기능 제공
    /// </summary>
    public static class BrightnessUtils
    {
        /// <summary>
        /// 새 GUID 생성
        /// </summary>
        public static Guid CreateGUID() => Guid.NewGuid();

        /// <summary>
        /// 아바타에 MA 오브젝트 추가
        /// </summary>
        public static void AddMAObject(GameObject avatar, AnimatorController controller)
        {
            ModularAvatarHelper.SetupBrightnessObject(avatar, controller);
        }

        /// <summary>
        /// Transform의 계층 경로 반환
        /// </summary>
        public static string GetPartialPath(Transform transform)
        {
            return PathHelper.GetHierarchyPath(transform);
        }

        /// <summary>
        /// 아바타에서 호환 가능한 쉐이더 경로 목록 반환
        /// </summary>
        public static List<string> CheckShader(GameObject avatar)
        {
            return ShaderHelper.GetCompatibleShaderPaths(avatar);
        }

        /// <summary>
        /// 애니메이션 클립 생성 및 복사
        /// </summary>
        public static (AnimationClip, AnimationClip, AnimationClip, AnimationClip, AnimationClip, AnimationClip)
            CreateAndCopyAnimation(GameObject avatar, List<string> pathList, Guid guid)
        {
            string outputPath = $"{BrightnessConstants.CREATE_PATH}{avatar.name}_{guid}/";
            AnimationClipSet clipSet = AnimationClipHelper.CreateAnimationClips(avatar, pathList, outputPath);

            return (
                clipSet.MinLight,
                clipSet.MaxLight,
                clipSet.BackLight,
                clipSet.Shadow,
                clipSet.ShadowXAngle,
                clipSet.ShadowYAngle
            );
        }

        /// <summary>
        /// 컨트롤러 수정 및 저장
        /// </summary>
        public static AnimatorController ModifyAndSaveController(
            GameObject avatar,
            AnimationClip minLightClip,
            AnimationClip maxLightClip,
            AnimationClip backLightClip,
            AnimationClip shadowClip,
            AnimationClip shadowXAngleClip,
            AnimationClip shadowYAngleClip,
            Guid guid)
        {
            var clipSet = new AnimationClipSet
            {
                MinLight = minLightClip,
                MaxLight = maxLightClip,
                BackLight = backLightClip,
                Shadow = shadowClip,
                ShadowXAngle = shadowXAngleClip,
                ShadowYAngle = shadowYAngleClip
            };

            return AnimatorControllerHelper.CreateController(avatar, clipSet, guid);
        }

        /// <summary>
        /// 애니메이션에서 밝기 값 읽기
        /// </summary>
        public static float ReadBrightnessValueFromAnimation(string animPath, string propertyName)
        {
            return AnimationClipHelper.ReadValue(animPath, propertyName);
        }

        /// <summary>
        /// 애니메이션에 밝기 값 설정
        /// </summary>
        public static void SetBrightnessValueInAnimation(string animPath, string propertyName, float value)
        {
            AnimationClipHelper.SetValue(animPath, propertyName, value);
        }

        /// <summary>
        /// Anchor Override 및 Bounds 재귀적 설정
        /// </summary>
        public static void SetAnchorOverrideAndBoundsRecursively(Transform transform, Transform chestBone)
        {
            RendererHelper.SetAnchorOverrideRecursively(transform, chestBone);
        }
    }
}
