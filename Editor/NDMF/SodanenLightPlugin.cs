using System.Collections.Generic;
using System.Linq;
using nadena.dev.ndmf;
using Sodanen.LightControl;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using VRC.SDK3.Avatars.Components;
using VRC.SDK3.Avatars.ScriptableObjects;
using nadena.dev.modular_avatar.core;

[assembly: ExportsPlugin(typeof(Brightness.Utility.SodanenLightPlugin))]

namespace Brightness.Utility
{
    /// <summary>
    /// NDMF 플러그인 - 빌드 시 SodanenLightControl 컴포넌트를 처리
    /// </summary>
    public class SodanenLightPlugin : Plugin<SodanenLightPlugin>
    {
        public override string DisplayName => "Sodanen Light Control";
        public override string QualifiedName => "com.sodanen.light-control";

        // 셰이더 프로퍼티 이름
        private const string PROP_MIN_LIGHT = "material._LightMinLimit";
        private const string PROP_MAX_LIGHT = "material._LightMaxLimit";
        private const string PROP_BACK_LIGHT = "material._BacklightBorder";
        private const string PROP_SHADOW = "material._ShadowStrength";
        private const string PROP_SHADOW_X = "material._LightDirection.x";
        private const string PROP_SHADOW_Y = "material._LightDirection.y";

        protected override void Configure()
        {
            InPhase(BuildPhase.Generating)
                .BeforePlugin("nadena.dev.modular-avatar")
                .Run("Process Light Control", ctx =>
                {
                    var components = ctx.AvatarRootObject
                        .GetComponentsInChildren<SodanenLightControl>(true);

                    foreach (var component in components)
                    {
                        ProcessLightControl(ctx, component);
                    }
                });
        }

        private void ProcessLightControl(BuildContext ctx, SodanenLightControl component)
        {
            if (!component.HasAnyFeatureEnabled())
            {
                Object.DestroyImmediate(component);
                return;
            }

            var avatarRoot = ctx.AvatarRootObject;
            var descriptor = avatarRoot.GetComponent<VRCAvatarDescriptor>();
            if (descriptor == null) return;

            // 타겟 머티리얼 경로 수집
            var materialPaths = CollectMaterialPaths(avatarRoot);

            if (materialPaths.Count == 0)
            {
                Debug.LogWarning("[SodanenLight] lilToon 머티리얼을 찾을 수 없습니다.");
                Object.DestroyImmediate(component);
                return;
            }

            // 디버그: 설정값 출력
            LogComponentSettings(component, materialPaths);

            // 애니메이션 클립 동적 생성
            var clipSet = CreateAnimationClipsDynamic(avatarRoot, materialPaths, component);

            // 디버그: 생성된 클립 정보 출력
            LogGeneratedClips(clipSet);

            // 애니메이터 컨트롤러 생성
            var controller = CreateControllerInMemory(clipSet, component);

            // MA 컴포넌트 설정
            SetupModularAvatarComponents(component.gameObject, controller, component);

            Debug.Log("[SodanenLight] 빌드 완료");

            // 원본 컴포넌트 제거
            Object.DestroyImmediate(component);
        }

        private void LogComponentSettings(SodanenLightControl component, List<string> materialPaths)
        {
            Debug.Log($"[SodanenLight] ===== 설정값 확인 =====");
            Debug.Log($"[SodanenLight] 타겟 머티리얼 경로: {materialPaths.Count}개");
            foreach (var path in materialPaths)
                Debug.Log($"  - {path}");

            Debug.Log($"[SodanenLight] ----- 기능 설정 -----");
            if (component.enableMinLight)
                Debug.Log($"  MinLight: {component.minLightRange.x} → {component.minLightRange.y}");
            if (component.enableMaxLight)
                Debug.Log($"  MaxLight: {component.maxLightRange.x} → {component.maxLightRange.y}");
            if (component.enableBackLight)
                Debug.Log($"  BackLight: 1 → {component.backLightValue}");
            if (component.enableShadow)
            {
                Debug.Log($"  Shadow (기본): {component.shadowRange.x} → {component.shadowRange.y}");
                foreach (var overrideItem in component.shadowOverrides)
                {
                    if (overrideItem.targetRenderer != null)
                        Debug.Log($"    └ {overrideItem.targetRenderer.name}: {overrideItem.shadowRange.x} → {overrideItem.shadowRange.y}");
                }
            }
            if (component.enableShadowXAngle)
                Debug.Log($"  ShadowXAngle: 기존 애니메이션 사용");
            if (component.enableShadowYAngle)
                Debug.Log($"  ShadowYAngle: 기존 애니메이션 사용");
        }

        private void LogGeneratedClips(SelectiveAnimationClipSet clipSet)
        {
            Debug.Log($"[SodanenLight] ----- 생성된 클립 -----");

            if (clipSet.MinLight != null)
                LogClipInfo("MinLight", clipSet.MinLight);
            if (clipSet.MaxLight != null)
                LogClipInfo("MaxLight", clipSet.MaxLight);
            if (clipSet.BackLight != null)
                LogClipInfo("BackLight", clipSet.BackLight);
            if (clipSet.Shadow != null)
                LogClipInfo("Shadow", clipSet.Shadow);
            if (clipSet.ShadowXAngle != null)
                LogClipInfo("ShadowXAngle", clipSet.ShadowXAngle);
            if (clipSet.ShadowYAngle != null)
                LogClipInfo("ShadowYAngle", clipSet.ShadowYAngle);
        }

        private void LogClipInfo(string name, AnimationClip clip)
        {
            var bindings = AnimationUtility.GetCurveBindings(clip);
            Debug.Log($"  {name}: {bindings.Length}개 바인딩");

            foreach (var binding in bindings)
            {
                var curve = AnimationUtility.GetEditorCurve(clip, binding);
                if (curve != null && curve.keys.Length >= 2)
                {
                    float startVal = curve.keys[0].value;
                    float endVal = curve.keys[curve.keys.Length - 1].value;
                    Debug.Log($"    [{binding.path}] {binding.propertyName}: {startVal} → {endVal}");
                }
            }
        }

        private List<string> CollectMaterialPaths(GameObject avatarRoot)
        {
            var paths = new List<string>();
            var avatarTransform = avatarRoot.transform;

            foreach (var renderer in avatarRoot.GetComponentsInChildren<Renderer>(true))
            {
                bool hasLilToon = renderer.sharedMaterials.Any(m =>
                    m != null && m.shader != null && m.shader.name.Contains(BrightnessConstants.SHADER_SHORT_NAME));

                if (hasLilToon)
                {
                    string path = AnimationUtility.CalculateTransformPath(renderer.transform, avatarTransform);
                    if (!string.IsNullOrEmpty(path))
                        paths.Add(path);
                }
            }

            return paths;
        }

        /// <summary>
        /// 컴포넌트의 범위 값을 기반으로 애니메이션 클립 동적 생성
        /// </summary>
        private SelectiveAnimationClipSet CreateAnimationClipsDynamic(
            GameObject avatar,
            List<string> targetPaths,
            SodanenLightControl component)
        {
            var clipSet = new SelectiveAnimationClipSet();

            if (component.enableMinLight)
                clipSet.MinLight = CreateDynamicClip(avatar, targetPaths, PROP_MIN_LIGHT,
                    component.minLightRange.x, component.minLightRange.y);

            if (component.enableMaxLight)
                clipSet.MaxLight = CreateDynamicClip(avatar, targetPaths, PROP_MAX_LIGHT,
                    component.maxLightRange.x, component.maxLightRange.y);

            // BackLight는 1 → 설정값 범위 (lilToon에서 1=역광없음, 0=역광최대)
            if (component.enableBackLight)
                clipSet.BackLight = CreateDynamicClip(avatar, targetPaths, PROP_BACK_LIGHT, 1f, component.backLightValue);

            if (component.enableShadow)
                clipSet.Shadow = CreateShadowClipWithOverrides(avatar, targetPaths, component);

            // Shadow X/Y Angle은 복잡한 커브라서 기존 애니메이션 복사
            if (component.enableShadowXAngle)
                clipSet.ShadowXAngle = CreateClipFromSource(avatar, targetPaths, BrightnessConstants.SHADOW_XANGLE_ANIM);

            if (component.enableShadowYAngle)
                clipSet.ShadowYAngle = CreateClipFromSource(avatar, targetPaths, BrightnessConstants.SHADOW_YANGLE_ANIM);

            return clipSet;
        }

        /// <summary>
        /// 기존 애니메이션 파일에서 클립 복사
        /// </summary>
        private AnimationClip CreateClipFromSource(GameObject avatar, List<string> targetPaths, string sourceClipPath)
        {
            var sourceClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(sourceClipPath);
            if (sourceClip == null)
            {
                Debug.LogError($"[SodanenLight] 소스 클립을 찾을 수 없음: {sourceClipPath}");
                return null;
            }

            var newClip = new AnimationClip();
            var bindings = AnimationUtility.GetCurveBindings(sourceClip);
            var avatarTransform = avatar.transform;

            foreach (var binding in bindings)
            {
                var curve = AnimationUtility.GetEditorCurve(sourceClip, binding);

                foreach (var targetPath in targetPaths)
                {
                    var targetTransform = avatarTransform.Find(targetPath);
                    if (targetTransform == null) continue;

                    System.Type rendererType = null;
                    if (targetTransform.GetComponent<SkinnedMeshRenderer>() != null)
                        rendererType = typeof(SkinnedMeshRenderer);
                    else if (targetTransform.GetComponent<MeshRenderer>() != null)
                        rendererType = typeof(MeshRenderer);

                    if (rendererType != null)
                    {
                        var newBinding = new EditorCurveBinding
                        {
                            type = rendererType,
                            path = targetPath,
                            propertyName = binding.propertyName
                        };
                        AnimationUtility.SetEditorCurve(newClip, newBinding, curve);
                    }
                }
            }

            return newClip;
        }

        /// <summary>
        /// 지정된 프로퍼티와 범위로 애니메이션 클립 생성
        /// </summary>
        private AnimationClip CreateDynamicClip(
            GameObject avatar,
            List<string> targetPaths,
            string propertyName,
            float startValue,
            float endValue)
        {
            var clip = new AnimationClip();
            var avatarTransform = avatar.transform;

            // 0~1초 동안 startValue에서 endValue로 변화하는 커브
            var curve = new AnimationCurve(
                new Keyframe(0f, startValue),
                new Keyframe(1f, endValue)
            );

            foreach (var targetPath in targetPaths)
            {
                var targetTransform = avatarTransform.Find(targetPath);
                if (targetTransform == null) continue;

                System.Type rendererType = null;
                if (targetTransform.GetComponent<SkinnedMeshRenderer>() != null)
                    rendererType = typeof(SkinnedMeshRenderer);
                else if (targetTransform.GetComponent<MeshRenderer>() != null)
                    rendererType = typeof(MeshRenderer);

                if (rendererType != null)
                {
                    var binding = new EditorCurveBinding
                    {
                        type = rendererType,
                        path = targetPath,
                        propertyName = propertyName
                    };
                    AnimationUtility.SetEditorCurve(clip, binding, curve);
                }
            }

            return clip;
        }

        /// <summary>
        /// Shadow 클립 생성 (오버라이드 지원)
        /// </summary>
        private AnimationClip CreateShadowClipWithOverrides(
            GameObject avatar,
            List<string> targetPaths,
            SodanenLightControl component)
        {
            var clip = new AnimationClip();
            var avatarTransform = avatar.transform;

            // 오버라이드 맵 생성 (Renderer 경로 → Shadow 범위)
            var overrideMap = new Dictionary<string, Vector2>();
            foreach (var overrideItem in component.shadowOverrides)
            {
                if (overrideItem.targetRenderer != null)
                {
                    string path = AnimationUtility.CalculateTransformPath(overrideItem.targetRenderer.transform, avatarTransform);
                    overrideMap[path] = overrideItem.shadowRange;
                }
            }

            foreach (var targetPath in targetPaths)
            {
                var targetTransform = avatarTransform.Find(targetPath);
                if (targetTransform == null) continue;

                System.Type rendererType = null;
                if (targetTransform.GetComponent<SkinnedMeshRenderer>() != null)
                    rendererType = typeof(SkinnedMeshRenderer);
                else if (targetTransform.GetComponent<MeshRenderer>() != null)
                    rendererType = typeof(MeshRenderer);

                if (rendererType != null)
                {
                    // 오버라이드가 있으면 해당 값 사용, 없으면 기본값
                    Vector2 range = overrideMap.ContainsKey(targetPath)
                        ? overrideMap[targetPath]
                        : component.shadowRange;

                    var curve = new AnimationCurve(
                        new Keyframe(0f, range.x),
                        new Keyframe(1f, range.y)
                    );

                    var binding = new EditorCurveBinding
                    {
                        type = rendererType,
                        path = targetPath,
                        propertyName = PROP_SHADOW
                    };
                    AnimationUtility.SetEditorCurve(clip, binding, curve);
                }
            }

            return clip;
        }

        /// <summary>
        /// 고정값 애니메이션 클립 생성 (BackLight용)
        /// </summary>
        private AnimationClip CreateFixedValueClip(
            GameObject avatar,
            List<string> targetPaths,
            string propertyName,
            float value)
        {
            var clip = new AnimationClip();
            var avatarTransform = avatar.transform;

            // 고정값 커브 (시작과 끝이 같은 값)
            var curve = new AnimationCurve(
                new Keyframe(0f, value),
                new Keyframe(1f, value)
            );

            foreach (var targetPath in targetPaths)
            {
                var targetTransform = avatarTransform.Find(targetPath);
                if (targetTransform == null) continue;

                System.Type rendererType = null;
                if (targetTransform.GetComponent<SkinnedMeshRenderer>() != null)
                    rendererType = typeof(SkinnedMeshRenderer);
                else if (targetTransform.GetComponent<MeshRenderer>() != null)
                    rendererType = typeof(MeshRenderer);

                if (rendererType != null)
                {
                    var binding = new EditorCurveBinding
                    {
                        type = rendererType,
                        path = targetPath,
                        propertyName = propertyName
                    };
                    AnimationUtility.SetEditorCurve(clip, binding, curve);
                }
            }

            return clip;
        }

        private AnimatorController CreateControllerInMemory(
            SelectiveAnimationClipSet clipSet,
            SodanenLightControl component)
        {
            var originalController = AssetDatabase.LoadAssetAtPath<AnimatorController>(
                BrightnessConstants.BRIGHTNESS_CONTROLLER_PATH);

            if (originalController == null)
            {
                Debug.LogError("[SodanenLight] 원본 컨트롤러를 찾을 수 없음");
                return null;
            }

            var newController = Object.Instantiate(originalController);
            var dummyClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(BrightnessConstants.DUMMY_ANIM_PATH);

            string[] featureNames = { "MinLight", "MaxLight", "BackLight", "Shadow", "Shadow_XAngle", "Shadow_YAngle" };
            bool[] featureFlags = {
                component.enableMinLight,
                component.enableMaxLight,
                component.enableBackLight,
                component.enableShadow,
                component.enableShadowXAngle,
                component.enableShadowYAngle
            };
            AnimationClip[] clips = {
                clipSet.MinLight,
                clipSet.MaxLight,
                clipSet.BackLight,
                clipSet.Shadow,
                clipSet.ShadowXAngle,
                clipSet.ShadowYAngle
            };

            var layersToKeep = new List<AnimatorControllerLayer>();

            foreach (var layer in newController.layers)
            {
                int featureIndex = System.Array.IndexOf(featureNames, layer.name);

                if (featureIndex < 0 || featureFlags[featureIndex])
                {
                    if (featureIndex >= 0 && featureIndex < clips.Length && clips[featureIndex] != null)
                    {
                        if (featureIndex >= 3)
                            ApplyStateMotion(layer.stateMachine, "Dummy", dummyClip);
                        ApplyStateMotion(layer.stateMachine, layer.name, clips[featureIndex]);
                    }
                    layersToKeep.Add(layer);
                }
            }

            newController.layers = layersToKeep.ToArray();
            RemoveUnusedParameters(newController, featureFlags);

            return newController;
        }

        private void ApplyStateMotion(AnimatorStateMachine sm, string stateName, AnimationClip clip)
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

        private void RemoveUnusedParameters(AnimatorController controller, bool[] featureFlags)
        {
            string[] featureNames = { "MinLight", "MaxLight", "BackLight", "Shadow", "Shadow_XAngle", "Shadow_YAngle" };
            var paramsToRemove = new HashSet<string>();

            for (int i = 0; i < featureNames.Length && i < featureFlags.Length; i++)
            {
                if (!featureFlags[i])
                    paramsToRemove.Add(featureNames[i]);
            }

            if (!featureFlags[4] && !featureFlags[5])
                paramsToRemove.Add("Toggle_Angle");

            for (int i = controller.parameters.Length - 1; i >= 0; i--)
            {
                if (paramsToRemove.Contains(controller.parameters[i].name))
                    controller.RemoveParameter(i);
            }
        }

        private void SetupModularAvatarComponents(
            GameObject target,
            AnimatorController controller,
            SodanenLightControl component)
        {
            var maAnimator = target.AddComponent<ModularAvatarMergeAnimator>();
            maAnimator.animator = controller;
            maAnimator.deleteAttachedAnimator = true;
            maAnimator.pathMode = MergeAnimatorPathMode.Absolute;
            maAnimator.matchAvatarWriteDefaults = true;

            var maParams = target.AddComponent<ModularAvatarParameters>();
            var configs = new List<ParameterConfig>();

            if (component.enableMaxLight)
                configs.Add(CreateFloatParam(BrightnessConstants.Parameters.MAX_LIGHT));
            if (component.enableMinLight)
                configs.Add(CreateFloatParam(BrightnessConstants.Parameters.MIN_LIGHT));
            if (component.enableShadow)
                configs.Add(CreateFloatParam(BrightnessConstants.Parameters.SHADOW));
            if (component.enableBackLight)
                configs.Add(CreateFloatParam(BrightnessConstants.Parameters.BACK_LIGHT));
            if (component.enableShadowXAngle || component.enableShadowYAngle)
                configs.Add(CreateBoolParam(BrightnessConstants.Parameters.TOGGLE_ANGLE));
            if (component.enableShadowXAngle)
                configs.Add(CreateFloatParam(BrightnessConstants.Parameters.SHADOW_XANGLE));
            if (component.enableShadowYAngle)
                configs.Add(CreateFloatParam(BrightnessConstants.Parameters.SHADOW_YANGLE));

            maParams.parameters.AddRange(configs);

            // 서브메뉴 생성
            var subMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
            subMenu.name = "Light";
            subMenu.controls = new List<VRCExpressionsMenu.Control>();

            if (component.enableMaxLight)
                subMenu.controls.Add(CreateRadialControl("Max Light", BrightnessConstants.Parameters.MAX_LIGHT));
            if (component.enableMinLight)
                subMenu.controls.Add(CreateRadialControl("Min Light", BrightnessConstants.Parameters.MIN_LIGHT));
            if (component.enableShadow)
                subMenu.controls.Add(CreateRadialControl("Shadow", BrightnessConstants.Parameters.SHADOW));
            if (component.enableBackLight)
                subMenu.controls.Add(CreateRadialControl("Back Light", BrightnessConstants.Parameters.BACK_LIGHT));
            if (component.enableShadowXAngle || component.enableShadowYAngle)
                subMenu.controls.Add(CreateToggleControl("Toggle Angle", BrightnessConstants.Parameters.TOGGLE_ANGLE));
            if (component.enableShadowXAngle)
                subMenu.controls.Add(CreateRadialControl("Shadow X", BrightnessConstants.Parameters.SHADOW_XANGLE));
            if (component.enableShadowYAngle)
                subMenu.controls.Add(CreateRadialControl("Shadow Y", BrightnessConstants.Parameters.SHADOW_YANGLE));

            var rootMenu = ScriptableObject.CreateInstance<VRCExpressionsMenu>();
            rootMenu.name = "SodanenLightMenu";
            rootMenu.controls = new List<VRCExpressionsMenu.Control>
            {
                new VRCExpressionsMenu.Control
                {
                    name = "Light",
                    type = VRCExpressionsMenu.Control.ControlType.SubMenu,
                    subMenu = subMenu
                }
            };

            var menuInstaller = target.AddComponent<ModularAvatarMenuInstaller>();
            menuInstaller.menuToAppend = rootMenu;
        }

        private VRCExpressionsMenu.Control CreateRadialControl(string name, string parameter)
        {
            return new VRCExpressionsMenu.Control
            {
                name = name,
                type = VRCExpressionsMenu.Control.ControlType.RadialPuppet,
                subParameters = new[] { new VRCExpressionsMenu.Control.Parameter { name = parameter } }
            };
        }

        private VRCExpressionsMenu.Control CreateToggleControl(string name, string parameter)
        {
            return new VRCExpressionsMenu.Control
            {
                name = name,
                type = VRCExpressionsMenu.Control.ControlType.Toggle,
                parameter = new VRCExpressionsMenu.Control.Parameter { name = parameter },
                value = 1
            };
        }

        private ParameterConfig CreateFloatParam(string name) => new ParameterConfig
        {
            nameOrPrefix = name,
            syncType = ParameterSyncType.Float,
            saved = true,
            defaultValue = BrightnessConstants.Defaults.PARAMETER_DEFAULT
        };

        private ParameterConfig CreateBoolParam(string name) => new ParameterConfig
        {
            nameOrPrefix = name,
            syncType = ParameterSyncType.Bool,
            saved = true,
            hasExplicitDefaultValue = true
        };
    }
}
