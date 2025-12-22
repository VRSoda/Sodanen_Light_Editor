using UnityEngine;
using System.Collections.Generic;
using System.Text;

namespace Brightness.Utility
{
    /// <summary>
    /// 쉐이더 관련 유틸리티
    /// </summary>
    public static class ShaderHelper
    {
        /// <summary>
        /// 아바타에서 호환 가능한 쉐이더를 사용하는 오브젝트 경로 목록 반환
        /// </summary>
        public static List<string> GetCompatibleShaderPaths(GameObject avatar)
        {
            var renderers = avatar.GetComponentsInChildren<Renderer>(true);
            var pathSet = new HashSet<string>();
            var pathList = new List<string>();

            foreach (var renderer in renderers)
            {
                var materials = renderer.sharedMaterials;
                if (materials == null || materials.Length == 0) continue;

                foreach (var material in materials)
                {
                    if (material != null && material.shader != null &&
                        material.shader.name.Contains(BrightnessConstants.SHADER_SHORT_NAME))
                    {
                        string path = PathHelper.GetHierarchyPath(renderer.transform);
                        if (pathSet.Add(path))
                        {
                            pathList.Add(path);
                        }
                        break;
                    }
                }
            }

            return pathList;
        }
    }

    /// <summary>
    /// 경로 관련 유틸리티
    /// </summary>
    public static class PathHelper
    {
        [System.ThreadStatic]
        private static StringBuilder s_pathBuilder;
        [System.ThreadStatic]
        private static List<string> s_pathParts;

        /// <summary>
        /// Transform의 계층 경로 반환 (루트 제외)
        /// </summary>
        public static string GetHierarchyPath(Transform transform)
        {
            if (s_pathParts == null) s_pathParts = new List<string>(16);
            else s_pathParts.Clear();

            while (transform.parent != null)
            {
                s_pathParts.Add(transform.name);
                transform = transform.parent;
            }

            if (s_pathParts.Count == 0) return string.Empty;
            if (s_pathParts.Count == 1) return s_pathParts[0];

            if (s_pathBuilder == null) s_pathBuilder = new StringBuilder(256);
            else s_pathBuilder.Clear();

            for (int i = s_pathParts.Count - 1; i >= 0; i--)
            {
                s_pathBuilder.Append(s_pathParts[i]);
                if (i > 0) s_pathBuilder.Append('/');
            }

            return s_pathBuilder.ToString();
        }
    }

    /// <summary>
    /// Renderer 관련 유틸리티
    /// </summary>
    public static class RendererHelper
    {
        private static readonly Bounds s_defaultBounds = new Bounds(Vector3.zero, Vector3.one * 2f);

        /// <summary>
        /// 아바타의 모든 SkinnedMeshRenderer에 Anchor Override와 Bounds 설정
        /// </summary>
        public static void SetAnchorOverrideRecursively(Transform root, Transform anchorBone)
        {
            var renderers = root.GetComponentsInChildren<SkinnedMeshRenderer>(true);
            foreach (var renderer in renderers)
            {
                renderer.rootBone = anchorBone;
                renderer.probeAnchor = anchorBone;
                renderer.localBounds = s_defaultBounds;
            }
        }
    }
}
