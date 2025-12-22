using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.ScriptableObjects;
using nadena.dev.modular_avatar.core;

namespace Brightness.Utility
{
    /// <summary>
    /// Modular Avatar 컴포넌트 설정 관련 유틸리티
    /// </summary>
    public static class ModularAvatarHelper
    {
        /// <summary>
        /// 아바타에 Brightness Control 오브젝트 추가 및 MA 컴포넌트 설정
        /// </summary>
        public static void SetupBrightnessObject(GameObject avatar, AnimatorController controller)
        {
            RemoveExistingBrightnessObject(avatar);

            GameObject brightnessObject = CreateBrightnessObject(avatar);
            SetupComponents(brightnessObject, controller);
        }

        /// <summary>
        /// 기존 BrightnessControl 오브젝트 제거
        /// </summary>
        private static void RemoveExistingBrightnessObject(GameObject avatar)
        {
            Transform existingObject = avatar.transform.Find(BrightnessConstants.OBJECT_NAME);
            if (existingObject != null)
            {
                GameObject.DestroyImmediate(existingObject.gameObject);
            }
        }

        /// <summary>
        /// BrightnessControl 오브젝트 생성
        /// </summary>
        private static GameObject CreateBrightnessObject(GameObject avatar)
        {
            GameObject brightnessObject = new GameObject(BrightnessConstants.OBJECT_NAME);
            brightnessObject.transform.SetParent(avatar.transform);
            return brightnessObject;
        }

        /// <summary>
        /// MA 컴포넌트 설정
        /// </summary>
        private static void SetupComponents(GameObject brightnessObject, AnimatorController controller)
        {
            SetupMergeAnimator(brightnessObject, controller);
            SetupMenuInstaller(brightnessObject);
            SetupParameters(brightnessObject);
        }

        /// <summary>
        /// MergeAnimator 컴포넌트 설정
        /// </summary>
        private static void SetupMergeAnimator(GameObject target, AnimatorController controller)
        {
            var maAnimator = target.AddComponent<ModularAvatarMergeAnimator>();
            maAnimator.animator = controller;
            maAnimator.deleteAttachedAnimator = true;
            maAnimator.pathMode = MergeAnimatorPathMode.Absolute;
            maAnimator.matchAvatarWriteDefaults = true;
        }

        /// <summary>
        /// MenuInstaller 컴포넌트 설정
        /// </summary>
        private static void SetupMenuInstaller(GameObject target)
        {
            var maMenu = target.AddComponent<ModularAvatarMenuInstaller>();
            maMenu.menuToAppend = AssetDatabase.LoadAssetAtPath<VRCExpressionsMenu>(
                BrightnessConstants.SETTINGS_ASSET_PATH);
        }

        /// <summary>
        /// Parameters 컴포넌트 설정
        /// </summary>
        private static void SetupParameters(GameObject target)
        {
            var maParams = target.AddComponent<ModularAvatarParameters>();
            AddAllParameters(maParams);
        }

        /// <summary>
        /// 모든 파라미터 추가
        /// </summary>
        private static void AddAllParameters(ModularAvatarParameters maParams)
        {
            var configs = new ParameterConfig[]
            {
                CreateFloatParameter(BrightnessConstants.Parameters.MAX_LIGHT),
                CreateFloatParameter(BrightnessConstants.Parameters.MIN_LIGHT),
                CreateFloatParameter(BrightnessConstants.Parameters.BACK_LIGHT),
                CreateFloatParameter(BrightnessConstants.Parameters.SHADOW),
                CreateBoolParameter(BrightnessConstants.Parameters.TOGGLE_ANGLE),
                CreateFloatParameter(BrightnessConstants.Parameters.SHADOW_XANGLE),
                CreateFloatParameter(BrightnessConstants.Parameters.SHADOW_YANGLE)
            };

            maParams.parameters.AddRange(configs);
        }

        /// <summary>
        /// Float 타입 파라미터 설정 생성
        /// </summary>
        private static ParameterConfig CreateFloatParameter(string name)
        {
            return new ParameterConfig
            {
                nameOrPrefix = name,
                syncType = ParameterSyncType.Float,
                saved = true,
                defaultValue = BrightnessConstants.Defaults.PARAMETER_DEFAULT
            };
        }

        /// <summary>
        /// Bool 타입 파라미터 설정 생성
        /// </summary>
        private static ParameterConfig CreateBoolParameter(string name)
        {
            return new ParameterConfig
            {
                nameOrPrefix = name,
                syncType = ParameterSyncType.Bool,
                saved = true,
                hasExplicitDefaultValue = true
            };
        }
    }
}
