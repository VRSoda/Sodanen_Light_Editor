using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using nadena.dev.modular_avatar.core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static Brightness.Localization.Loc;

namespace Brightness.Utility
{
    /// <summary>
    /// 아바타 검색 및 선택 Helper
    /// </summary>
    public static class AvatarHelper
    {
        /// <summary>
        /// 씬에서 아바타 목록을 가져옵니다.
        /// </summary>
        public static VRCAvatarDescriptor[] GetSceneAvatars()
        {
            return Resources.FindObjectsOfTypeAll<VRCAvatarDescriptor>()
                .Where(a => a.gameObject.scene.isLoaded)
                .OrderByDescending(a => a.gameObject.activeInHierarchy)
                .ThenBy(a => a.gameObject.name)
                .ToArray();
        }

        /// <summary>
        /// 아바타 드롭다운용 이름 배열을 생성합니다.
        /// </summary>
        public static string[] GetAvatarNames(VRCAvatarDescriptor[] avatars)
        {
            var names = new string[avatars.Length + 1];
            names[0] = L("avatar.select_dropdown");

            for (int i = 0; i < avatars.Length; i++)
            {
                var go = avatars[i].gameObject;
                names[i + 1] = go.activeInHierarchy ? go.name : $"{go.name} (Off)";
            }

            return names;
        }

        /// <summary>
        /// 현재 선택된 아바타의 인덱스를 찾습니다.
        /// </summary>
        public static int FindAvatarIndex(VRCAvatarDescriptor[] avatars, GameObject targetAvatar)
        {
            if (targetAvatar == null) return 0;

            for (int i = 0; i < avatars.Length; i++)
            {
                if (avatars[i].gameObject == targetAvatar)
                    return i + 1;
            }

            return 0;
        }
    }

    /// <summary>
    /// 선택적 애니메이션 클립 생성 Helper
    /// </summary>
    public static class SelectiveAnimationClipHelper
    {
        private static readonly Dictionary<string, AnimationClip> s_sourceClipCache = new Dictionary<string, AnimationClip>();

        public static SelectiveAnimationClipSet CreateSelectiveAnimationClips(
            GameObject avatar,
            List<string> targetPaths,
            string outputPath,
            bool minLight,
            bool maxLight,
            bool backLight,
            bool shadow,
            bool shadowXAngle,
            bool shadowYAngle)
        {
            EnsureDirectoryExists(outputPath);
            var targetPathSet = new HashSet<string>(targetPaths);
            var clipSet = new SelectiveAnimationClipSet();
            string avatarName = avatar.name;

            if (minLight)
                clipSet.MinLight = CreateAndSaveClip(avatar, targetPathSet, outputPath,
                    BrightnessConstants.MIN_LIGHT_ANIM, "MinLight_" + avatarName);

            if (maxLight)
                clipSet.MaxLight = CreateAndSaveClip(avatar, targetPathSet, outputPath,
                    BrightnessConstants.MAX_LIGHT_ANIM, "MaxLight_" + avatarName);

            if (backLight)
                clipSet.BackLight = CreateAndSaveClip(avatar, targetPathSet, outputPath,
                    BrightnessConstants.BACK_LIGHT_ANIM, "BackLight_" + avatarName);

            if (shadow)
                clipSet.Shadow = CreateAndSaveClip(avatar, targetPathSet, outputPath,
                    BrightnessConstants.SHADOW_ANGLE_ANIM, "Shadow_" + avatarName);

            if (shadowXAngle)
                clipSet.ShadowXAngle = CreateAndSaveClip(avatar, targetPathSet, outputPath,
                    BrightnessConstants.SHADOW_XANGLE_ANIM, "ShadowXAngle_" + avatarName);

            if (shadowYAngle)
                clipSet.ShadowYAngle = CreateAndSaveClip(avatar, targetPathSet, outputPath,
                    BrightnessConstants.SHADOW_YANGLE_ANIM, "ShadowYAngle_" + avatarName);

            AssetDatabase.SaveAssets();
            return clipSet;
        }

        public static SelectiveAnimationClipSet CreateSelectiveAnimationClipsWithMaterials(
            GameObject avatar,
            string outputPath,
            MaterialSelections materialSelections)
        {
            EnsureDirectoryExists(outputPath);
            var clipSet = new SelectiveAnimationClipSet();
            string avatarName = avatar.name;

            if (materialSelections.MinLight != null && materialSelections.MinLight.Count > 0)
                clipSet.MinLight = CreateAndSaveClip(avatar, new HashSet<string>(materialSelections.MinLight), outputPath,
                    BrightnessConstants.MIN_LIGHT_ANIM, "MinLight_" + avatarName);

            if (materialSelections.MaxLight != null && materialSelections.MaxLight.Count > 0)
                clipSet.MaxLight = CreateAndSaveClip(avatar, new HashSet<string>(materialSelections.MaxLight), outputPath,
                    BrightnessConstants.MAX_LIGHT_ANIM, "MaxLight_" + avatarName);

            if (materialSelections.BackLight != null && materialSelections.BackLight.Count > 0)
                clipSet.BackLight = CreateAndSaveClip(avatar, new HashSet<string>(materialSelections.BackLight), outputPath,
                    BrightnessConstants.BACK_LIGHT_ANIM, "BackLight_" + avatarName);

            if (materialSelections.Shadow != null && materialSelections.Shadow.Count > 0)
                clipSet.Shadow = CreateAndSaveClip(avatar, new HashSet<string>(materialSelections.Shadow), outputPath,
                    BrightnessConstants.SHADOW_ANGLE_ANIM, "Shadow_" + avatarName);

            if (materialSelections.ShadowX != null && materialSelections.ShadowX.Count > 0)
                clipSet.ShadowXAngle = CreateAndSaveClip(avatar, new HashSet<string>(materialSelections.ShadowX), outputPath,
                    BrightnessConstants.SHADOW_XANGLE_ANIM, "ShadowXAngle_" + avatarName);

            if (materialSelections.ShadowY != null && materialSelections.ShadowY.Count > 0)
                clipSet.ShadowYAngle = CreateAndSaveClip(avatar, new HashSet<string>(materialSelections.ShadowY), outputPath,
                    BrightnessConstants.SHADOW_YANGLE_ANIM, "ShadowYAngle_" + avatarName);

            AssetDatabase.SaveAssets();
            return clipSet;
        }

        public static SelectiveAnimationClipSet CreateSelectiveAnimationClipsWithMaterials(
            GameObject avatar,
            string outputPath,
            object materialSelectionsObj)
        {
            var materialSelections = materialSelectionsObj as MaterialSelections;
            if (materialSelections != null)
                return CreateSelectiveAnimationClipsWithMaterials(avatar, outputPath, materialSelections);

            EnsureDirectoryExists(outputPath);
            AssetDatabase.SaveAssets();
            return new SelectiveAnimationClipSet();
        }

        private static AnimationClip CreateAndSaveClip(
            GameObject avatar,
            HashSet<string> targetPaths,
            string outputPath,
            string sourceClipPath,
            string newClipName)
        {
            if (!s_sourceClipCache.TryGetValue(sourceClipPath, out var sourceClip) || sourceClip == null)
            {
                sourceClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(sourceClipPath);
                s_sourceClipCache[sourceClipPath] = sourceClip;
            }

            var newClip = new AnimationClip { name = newClipName };
            CopyClipCurvesIterative(avatar, targetPaths, sourceClip, newClip);

            string clipPath = outputPath + newClipName + ".anim";
            if (AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath) != null)
                AssetDatabase.DeleteAsset(clipPath);

            AssetDatabase.CreateAsset(newClip, clipPath);
            return newClip;
        }

        private static void CopyClipCurvesIterative(GameObject avatar, HashSet<string> targetPaths,
            AnimationClip source, AnimationClip dest)
        {
            var bindings = AnimationUtility.GetCurveBindings(source);
            var avatarTransform = avatar.transform;
            var stack = new Stack<Transform>(32);

            foreach (var binding in bindings)
            {
                var curve = AnimationUtility.GetEditorCurve(source, binding);
                stack.Clear();
                stack.Push(avatarTransform);

                while (stack.Count > 0)
                {
                    var current = stack.Pop();

                    for (int i = 0; i < current.childCount; i++)
                    {
                        var child = current.GetChild(i);
                        string fullPath = AnimationUtility.CalculateTransformPath(child, avatarTransform);

                        if (targetPaths.Contains(fullPath))
                        {
                            Type rendererType = null;
                            if (child.GetComponent<SkinnedMeshRenderer>() != null)
                                rendererType = typeof(SkinnedMeshRenderer);
                            else if (child.GetComponent<MeshRenderer>() != null)
                                rendererType = typeof(MeshRenderer);

                            if (rendererType != null)
                            {
                                var newBinding = new EditorCurveBinding
                                {
                                    type = rendererType,
                                    path = fullPath,
                                    propertyName = binding.propertyName
                                };
                                AnimationUtility.SetEditorCurve(dest, newBinding, curve);
                            }
                        }

                        stack.Push(child);
                    }
                }
            }
        }

        private static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }
    }

    /// <summary>
    /// 선택적 애니메이터 컨트롤러 생성 Helper
    /// </summary>
    public static class SelectiveAnimatorControllerHelper
    {
        private static readonly string[] s_featureNames = { "MinLight", "MaxLight", "BackLight", "Shadow", "Shadow_XAngle", "Shadow_YAngle" };

        public static AnimatorController CreateSelectiveController(
            GameObject avatar,
            SelectiveAnimationClipSet clipSet,
            string outputPath,
            bool minLight,
            bool maxLight,
            bool backLight,
            bool shadow,
            bool shadowXAngle,
            bool shadowYAngle)
        {
            var originalController = AssetDatabase.LoadAssetAtPath<AnimatorController>(
                BrightnessConstants.BRIGHTNESS_CONTROLLER_PATH);
            var newController = UnityEngine.Object.Instantiate(originalController);
            var dummyClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(BrightnessConstants.DUMMY_ANIM_PATH);

            bool[] featureFlags = { minLight, maxLight, backLight, shadow, shadowXAngle, shadowYAngle };
            AnimationClip[] clips = { clipSet.MinLight, clipSet.MaxLight, clipSet.BackLight,
                                       clipSet.Shadow, clipSet.ShadowXAngle, clipSet.ShadowYAngle };

            var layersToKeep = new List<AnimatorControllerLayer>();

            foreach (var layer in newController.layers)
            {
                int featureIndex = Array.IndexOf(s_featureNames, layer.name);

                if (featureIndex < 0 || featureFlags[featureIndex])
                {
                    if (featureIndex >= 0 && featureIndex < clips.Length)
                    {
                        if (featureIndex >= 3) // Shadow layers need dummy clip
                            ApplyStateMotion(layer.stateMachine, "Dummy", dummyClip);
                        ApplyStateMotion(layer.stateMachine, layer.name, clips[featureIndex]);
                    }
                    layersToKeep.Add(layer);
                }
            }

            newController.layers = layersToKeep.ToArray();
            RemoveUnusedParameters(newController, featureFlags);

            string controllerPath = outputPath + $"BrightnessController_{avatar.name}.controller";

            if (AssetDatabase.LoadAssetAtPath<AnimatorController>(controllerPath) != null)
                AssetDatabase.DeleteAsset(controllerPath);

            AssetDatabase.CreateAsset(newController, controllerPath);
            AssetDatabase.SaveAssets();

            return newController;
        }

        private static void ApplyStateMotion(AnimatorStateMachine sm, string stateName, AnimationClip clip)
        {
            if (clip == null || sm == null) return;

            foreach (var childState in sm.states)
            {
                if (childState.state.name == stateName)
                    childState.state.motion = clip;
            }

            foreach (var childSm in sm.stateMachines)
            {
                ApplyStateMotion(childSm.stateMachine, stateName, clip);
            }
        }

        private static void RemoveUnusedParameters(AnimatorController controller, bool[] featureFlags)
        {
            var paramsToRemove = new HashSet<string>();

            for (int i = 0; i < s_featureNames.Length && i < featureFlags.Length; i++)
            {
                if (!featureFlags[i])
                    paramsToRemove.Add(s_featureNames[i]);
            }

            // Toggle_Angle은 Shadow X/Y가 모두 비활성화되면 제거
            if (!featureFlags[4] && !featureFlags[5])
                paramsToRemove.Add("Toggle_Angle");

            for (int i = controller.parameters.Length - 1; i >= 0; i--)
            {
                if (paramsToRemove.Contains(controller.parameters[i].name))
                    controller.RemoveParameter(i);
            }
        }
    }

    /// <summary>
    /// 선택적 Modular Avatar 설정 Helper
    /// </summary>
    public static class SelectiveModularAvatarHelper
    {
        public static void SetupSelectiveBrightnessObject(
            GameObject avatar,
            AnimatorController controller,
            VRCExpressionsMenu targetMenu,
            bool minLight,
            bool maxLight,
            bool backLight,
            bool shadow,
            bool shadowXAngle,
            bool shadowYAngle)
        {
            var existing = avatar.transform.Find(BrightnessConstants.OBJECT_NAME);
            if (existing != null)
                GameObject.DestroyImmediate(existing.gameObject);

            var brightnessObject = new GameObject(BrightnessConstants.OBJECT_NAME);
            brightnessObject.transform.SetParent(avatar.transform);

            SetupMergeAnimator(brightnessObject, controller);
            SetupMenuInstaller(brightnessObject, targetMenu);
            SetupSelectiveParameters(brightnessObject, minLight, maxLight, backLight, shadow, shadowXAngle, shadowYAngle);
        }

        private static void SetupMergeAnimator(GameObject target, AnimatorController controller)
        {
            var maAnimator = target.AddComponent<ModularAvatarMergeAnimator>();
            maAnimator.animator = controller;
            maAnimator.deleteAttachedAnimator = true;
            maAnimator.pathMode = MergeAnimatorPathMode.Absolute;
            maAnimator.matchAvatarWriteDefaults = true;
        }

        private static void SetupMenuInstaller(GameObject target, VRCExpressionsMenu targetMenu)
        {
            var maMenu = target.AddComponent<ModularAvatarMenuInstaller>();
            maMenu.menuToAppend = AssetDatabase.LoadAssetAtPath<VRCExpressionsMenu>(
                BrightnessConstants.SETTINGS_ASSET_PATH);

            // 선택된 메뉴가 있으면 해당 메뉴에 설치
            if (targetMenu != null)
            {
                maMenu.installTargetMenu = targetMenu;
            }
        }

        private static void SetupSelectiveParameters(GameObject target,
            bool minLight, bool maxLight, bool backLight, bool shadow, bool shadowXAngle, bool shadowYAngle)
        {
            var maParams = target.AddComponent<ModularAvatarParameters>();
            var configs = new List<ParameterConfig>(7);

            if (minLight) configs.Add(CreateFloatParam(BrightnessConstants.Parameters.MIN_LIGHT));
            if (maxLight) configs.Add(CreateFloatParam(BrightnessConstants.Parameters.MAX_LIGHT));
            if (backLight) configs.Add(CreateFloatParam(BrightnessConstants.Parameters.BACK_LIGHT));
            if (shadow) configs.Add(CreateFloatParam(BrightnessConstants.Parameters.SHADOW));
            if (shadowXAngle || shadowYAngle) configs.Add(CreateBoolParam(BrightnessConstants.Parameters.TOGGLE_ANGLE));
            if (shadowXAngle) configs.Add(CreateFloatParam(BrightnessConstants.Parameters.SHADOW_XANGLE));
            if (shadowYAngle) configs.Add(CreateFloatParam(BrightnessConstants.Parameters.SHADOW_YANGLE));

            maParams.parameters.AddRange(configs);
        }

        private static ParameterConfig CreateFloatParam(string name) => new ParameterConfig
        {
            nameOrPrefix = name,
            syncType = ParameterSyncType.Float,
            saved = true,
            defaultValue = BrightnessConstants.Defaults.PARAMETER_DEFAULT
        };

        private static ParameterConfig CreateBoolParam(string name) => new ParameterConfig
        {
            nameOrPrefix = name,
            syncType = ParameterSyncType.Bool,
            saved = true,
            hasExplicitDefaultValue = true
        };
    }

    /// <summary>
    /// 선택적 애니메이션 클립 세트
    /// </summary>
    public class SelectiveAnimationClipSet
    {
        public AnimationClip MinLight { get; set; }
        public AnimationClip MaxLight { get; set; }
        public AnimationClip BackLight { get; set; }
        public AnimationClip Shadow { get; set; }
        public AnimationClip ShadowXAngle { get; set; }
        public AnimationClip ShadowYAngle { get; set; }
    }
}
