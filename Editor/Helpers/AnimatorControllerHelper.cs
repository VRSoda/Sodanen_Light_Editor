using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using System;

namespace Brightness.Utility
{
    /// <summary>
    /// 애니메이터 컨트롤러 생성 및 수정 관련 유틸리티
    /// </summary>
    public static class AnimatorControllerHelper
    {
        /// <summary>
        /// 새 컨트롤러 생성 및 애니메이션 클립 적용
        /// </summary>
        public static AnimatorController CreateController(GameObject avatar, AnimationClipSet clipSet, Guid guid)
        {
            AnimatorController originalController = AssetDatabase.LoadAssetAtPath<AnimatorController>(
                BrightnessConstants.BRIGHTNESS_CONTROLLER_PATH);
            AnimatorController newController = UnityEngine.Object.Instantiate(originalController);

            Debug.Log($"[BrightnessControl] Applying animation clips to new controller for {avatar.name}.");

            AnimationClip dummyClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(BrightnessConstants.DUMMY_ANIM_PATH);

            ApplyClipsToLayers(newController, clipSet, dummyClip);

            string newControllerPath = GetControllerPath(avatar.name, guid);
            SaveController(newController, newControllerPath);

            Debug.Log($"[BrightnessControl] Created new controller at '{newControllerPath}'.");
            return newController;
        }

        /// <summary>
        /// 컨트롤러의 모든 레이어에 클립 적용
        /// </summary>
        private static void ApplyClipsToLayers(AnimatorController controller, AnimationClipSet clipSet, AnimationClip dummyClip)
        {
            foreach (var layer in controller.layers)
            {
                switch (layer.name)
                {
                    case BrightnessConstants.Layers.MIN_LIGHT:
                        ApplyStateMotion(layer.stateMachine, BrightnessConstants.Layers.MIN_LIGHT, clipSet.MinLight);
                        LogApplied(clipSet.MinLight.name, BrightnessConstants.Layers.MIN_LIGHT, layer.name);
                        break;

                    case BrightnessConstants.Layers.MAX_LIGHT:
                        ApplyStateMotion(layer.stateMachine, BrightnessConstants.Layers.MAX_LIGHT, clipSet.MaxLight);
                        LogApplied(clipSet.MaxLight.name, BrightnessConstants.Layers.MAX_LIGHT, layer.name);
                        break;

                    case BrightnessConstants.Layers.BACK_LIGHT:
                        ApplyStateMotion(layer.stateMachine, BrightnessConstants.Layers.BACK_LIGHT, clipSet.BackLight);
                        LogApplied(clipSet.BackLight.name, BrightnessConstants.Layers.BACK_LIGHT, layer.name);
                        break;

                    case BrightnessConstants.Layers.SHADOW:
                        ApplyStateMotion(layer.stateMachine, "Dummy", dummyClip);
                        ApplyStateMotion(layer.stateMachine, BrightnessConstants.Layers.SHADOW, clipSet.Shadow);
                        LogApplied(clipSet.Shadow.name, BrightnessConstants.Layers.SHADOW, layer.name);
                        break;

                    case BrightnessConstants.Layers.SHADOW_XANGLE:
                        ApplyStateMotion(layer.stateMachine, "Dummy", dummyClip);
                        ApplyStateMotion(layer.stateMachine, BrightnessConstants.Layers.SHADOW_XANGLE, clipSet.ShadowXAngle);
                        LogApplied(clipSet.ShadowXAngle.name, BrightnessConstants.Layers.SHADOW_XANGLE, layer.name);
                        break;

                    case BrightnessConstants.Layers.SHADOW_YANGLE:
                        ApplyStateMotion(layer.stateMachine, "Dummy", dummyClip);
                        ApplyStateMotion(layer.stateMachine, BrightnessConstants.Layers.SHADOW_YANGLE, clipSet.ShadowYAngle);
                        LogApplied(clipSet.ShadowYAngle.name, BrightnessConstants.Layers.SHADOW_YANGLE, layer.name);
                        break;
                }
            }
        }

        /// <summary>
        /// 상태 머신에서 특정 이름의 상태를 찾아 모션 적용
        /// </summary>
        private static void ApplyStateMotion(AnimatorStateMachine stateMachine, string stateName, AnimationClip clip)
        {
            if (clip == null || stateMachine == null) return;

            foreach (var childState in stateMachine.states)
            {
                if (childState.state.name == stateName)
                {
                    childState.state.motion = clip;
                }
            }

            foreach (var childSm in stateMachine.stateMachines)
            {
                ApplyStateMotion(childSm.stateMachine, stateName, clip);
            }
        }

        /// <summary>
        /// 컨트롤러 저장
        /// </summary>
        private static void SaveController(AnimatorController controller, string path)
        {
            if (AssetDatabase.LoadAssetAtPath<AnimatorController>(path) != null)
            {
                AssetDatabase.DeleteAsset(path);
            }
            AssetDatabase.CreateAsset(controller, path);
            AssetDatabase.SaveAssets();
        }

        private static string GetControllerPath(string avatarName, Guid guid)
        {
            return $"{BrightnessConstants.CREATE_PATH}{avatarName}_{guid}/BrightnessController_{avatarName}.controller";
        }

        private static void LogApplied(string clipName, string stateName, string layerName)
        {
            Debug.Log($"Applied {clipName} to {stateName} state in {layerName} layer.");
        }
    }
}
