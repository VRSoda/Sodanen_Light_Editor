using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace Brightness.Utility
{
    /// <summary>
    /// 애니메이션 클립 생성 및 수정 관련 유틸리티
    /// </summary>
    public static class AnimationClipHelper
    {
        private static AnimationClipSet s_cachedSourceClips;

        /// <summary>
        /// 아바타의 모든 호환 오브젝트에 대해 애니메이션 클립들을 생성
        /// </summary>
        public static AnimationClipSet CreateAnimationClips(GameObject avatar, List<string> targetPaths, string outputPath)
        {
            var targetPathSet = new HashSet<string>(targetPaths);
            string avatarName = avatar.name;

            var clipSet = new AnimationClipSet
            {
                MinLight = new AnimationClip { name = "MinLight_" + avatarName },
                MaxLight = new AnimationClip { name = "MaxLight_" + avatarName },
                BackLight = new AnimationClip { name = "BackLight_" + avatarName },
                Shadow = new AnimationClip { name = "Shadow_" + avatarName },
                ShadowXAngle = new AnimationClip { name = "ShadowXAngle_" + avatarName },
                ShadowYAngle = new AnimationClip { name = "ShadowYAngle_" + avatarName }
            };

            var sourceClipSet = GetSourceClips();
            EnsureDirectoryExists(outputPath);

            CopyAndSaveClip(avatar, targetPathSet, sourceClipSet.MinLight, clipSet.MinLight, outputPath);
            CopyAndSaveClip(avatar, targetPathSet, sourceClipSet.MaxLight, clipSet.MaxLight, outputPath);
            CopyAndSaveClip(avatar, targetPathSet, sourceClipSet.BackLight, clipSet.BackLight, outputPath);
            CopyAndSaveClip(avatar, targetPathSet, sourceClipSet.Shadow, clipSet.Shadow, outputPath);
            CopyAndSaveClip(avatar, targetPathSet, sourceClipSet.ShadowXAngle, clipSet.ShadowXAngle, outputPath);
            CopyAndSaveClip(avatar, targetPathSet, sourceClipSet.ShadowYAngle, clipSet.ShadowYAngle, outputPath);

            AssetDatabase.SaveAssets();
            return clipSet;
        }

        /// <summary>
        /// 원본 애니메이션 클립들 로드 (캐싱)
        /// </summary>
        private static AnimationClipSet GetSourceClips()
        {
            if (s_cachedSourceClips == null || s_cachedSourceClips.MinLight == null)
            {
                s_cachedSourceClips = new AnimationClipSet
                {
                    MinLight = AssetDatabase.LoadAssetAtPath<AnimationClip>(BrightnessConstants.MIN_LIGHT_ANIM),
                    MaxLight = AssetDatabase.LoadAssetAtPath<AnimationClip>(BrightnessConstants.MAX_LIGHT_ANIM),
                    BackLight = AssetDatabase.LoadAssetAtPath<AnimationClip>(BrightnessConstants.BACK_LIGHT_ANIM),
                    Shadow = AssetDatabase.LoadAssetAtPath<AnimationClip>(BrightnessConstants.SHADOW_ANGLE_ANIM),
                    ShadowXAngle = AssetDatabase.LoadAssetAtPath<AnimationClip>(BrightnessConstants.SHADOW_XANGLE_ANIM),
                    ShadowYAngle = AssetDatabase.LoadAssetAtPath<AnimationClip>(BrightnessConstants.SHADOW_YANGLE_ANIM)
                };
            }
            return s_cachedSourceClips;
        }

        /// <summary>
        /// 애니메이션 클립 복사 및 저장
        /// </summary>
        private static void CopyAndSaveClip(GameObject avatar, HashSet<string> targetPaths,
            AnimationClip source, AnimationClip dest, string outputPath)
        {
            string clipPath = outputPath + dest.name + ".anim";

            var existingClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(clipPath);
            if (existingClip != null)
            {
                AssetDatabase.DeleteAsset(clipPath);
            }

            CopyClipCurves(avatar, targetPaths, source, dest);
            AssetDatabase.CreateAsset(dest, clipPath);
        }

        /// <summary>
        /// 클립 커브 복사
        /// </summary>
        private static void CopyClipCurves(GameObject avatar, HashSet<string> targetPaths,
            AnimationClip source, AnimationClip dest)
        {
            var bindings = AnimationUtility.GetCurveBindings(source);
            var avatarTransform = avatar.transform;

            foreach (var binding in bindings)
            {
                var curve = AnimationUtility.GetEditorCurve(source, binding);
                ApplyCurveToTargetsIterative(avatarTransform, targetPaths, binding, curve, dest);
            }
        }

        /// <summary>
        /// 커브를 모든 대상에 반복적으로 적용 (재귀 대신 스택 사용)
        /// </summary>
        private static void ApplyCurveToTargetsIterative(Transform avatarRoot, HashSet<string> targetPaths,
            EditorCurveBinding binding, AnimationCurve curve, AnimationClip dest)
        {
            var stack = new Stack<Transform>(32);
            stack.Push(avatarRoot);

            while (stack.Count > 0)
            {
                var current = stack.Pop();

                for (int i = 0; i < current.childCount; i++)
                {
                    var child = current.GetChild(i);
                    string fullPath = AnimationUtility.CalculateTransformPath(child, avatarRoot);

                    if (targetPaths.Contains(fullPath))
                    {
                        System.Type rendererType = null;
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

        /// <summary>
        /// 애니메이션에서 밝기 값 읽기
        /// </summary>
        public static float ReadValue(string animPath, string propertyName)
        {
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(animPath);
            if (clip == null)
            {
                Debug.LogError($"Animation Clip not found: {animPath}");
                return 0.0f;
            }

            var bindings = AnimationUtility.GetCurveBindings(clip);
            foreach (var binding in bindings)
            {
                if (!binding.propertyName.Contains(propertyName)) continue;

                var curve = AnimationUtility.GetEditorCurve(clip, binding);
                if (curve != null && curve.keys.Length > 0)
                {
                    return curve.keys[0].value;
                }
            }
            return 0.0f;
        }

        /// <summary>
        /// 애니메이션에 밝기 값 설정
        /// </summary>
        public static void SetValue(string animPath, string propertyName, float value)
        {
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(animPath);
            if (clip == null)
            {
                Debug.LogError($"Animation Clip not found: {animPath}");
                return;
            }

            var binding = new EditorCurveBinding
            {
                type = typeof(SkinnedMeshRenderer),
                path = "Body",
                propertyName = "material." + propertyName
            };

            var curve = new AnimationCurve(new Keyframe(0, value));
            AnimationUtility.SetEditorCurve(clip, binding, curve);

            EditorUtility.SetDirty(clip);
            AssetDatabase.SaveAssets();
        }

        private static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
    }

    /// <summary>
    /// 애니메이션 클립 세트 컨테이너
    /// </summary>
    public class AnimationClipSet
    {
        public AnimationClip MinLight { get; set; }
        public AnimationClip MaxLight { get; set; }
        public AnimationClip BackLight { get; set; }
        public AnimationClip Shadow { get; set; }
        public AnimationClip ShadowXAngle { get; set; }
        public AnimationClip ShadowYAngle { get; set; }
    }
}
