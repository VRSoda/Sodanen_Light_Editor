using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VRC.SDK3.Avatars.Components;

namespace Brightness.Utility
{
    public class BrightnessControlParameters
    {
        public GameObject TargetAvatar { get; set; }
        public FeatureToggles Toggles { get; set; }
        public FeatureMaterialSelections Selections { get; set; }
        public List<string> AllMaterialPaths { get; set; }
    }

    public static class SodanenEditorLogic
    {
        public static void ApplyBrightnessControl(BrightnessControlParameters parameters)
        {
            if (parameters.AllMaterialPaths.Count == 0)
            {
                EditorUtility.DisplayDialog("경고", "lilToon 쉐이더를 사용하는 오브젝트를 찾을 수 없습니다.", "확인");
                return;
            }

            string outputPath = $"{BrightnessConstants.CREATE_PATH}{parameters.TargetAvatar.name}/";

            var materialSelections = new MaterialSelections
            {
                MinLight = parameters.Toggles.MinLight ? GetSelectedMaterials(parameters.Selections.MinLight) : new List<string>(),
                MaxLight = parameters.Toggles.MaxLight ? GetSelectedMaterials(parameters.Selections.MaxLight) : new List<string>(),
                BackLight = parameters.Toggles.BackLight ? GetSelectedMaterials(parameters.Selections.BackLight) : new List<string>(),
                Shadow = parameters.Toggles.Shadow ? GetSelectedMaterials(parameters.Selections.Shadow) : new List<string>(),
                ShadowX = parameters.Toggles.ShadowXAngle ? GetSelectedMaterials(parameters.Selections.ShadowX) : new List<string>(),
                ShadowY = parameters.Toggles.ShadowYAngle ? GetSelectedMaterials(parameters.Selections.ShadowY) : new List<string>()
            };

            var clipSet = SelectiveAnimationClipHelper.CreateSelectiveAnimationClipsWithMaterials(
                parameters.TargetAvatar, outputPath, materialSelections);

            bool useMinLight = parameters.Toggles.MinLight && materialSelections.MinLight.Any();
            bool useMaxLight = parameters.Toggles.MaxLight && materialSelections.MaxLight.Any();
            bool useBackLight = parameters.Toggles.BackLight && materialSelections.BackLight.Any();
            bool useShadow = parameters.Toggles.Shadow && materialSelections.Shadow.Any();
            bool useShadowX = parameters.Toggles.ShadowXAngle && materialSelections.ShadowX.Any();
            bool useShadowY = parameters.Toggles.ShadowYAngle && materialSelections.ShadowY.Any();

            var controller = SelectiveAnimatorControllerHelper.CreateSelectiveController(
                parameters.TargetAvatar, clipSet, outputPath,
                useMinLight, useMaxLight, useBackLight, useShadow, useShadowX, useShadowY);

            SelectiveModularAvatarHelper.SetupSelectiveBrightnessObject(
                parameters.TargetAvatar, controller,
                useMinLight, useMaxLight, useBackLight, useShadow, useShadowX, useShadowY);

            var featureLogs = new List<string>();
            if (useMinLight) featureLogs.Add($"Min Light({materialSelections.MinLight.Count})");
            if (useMaxLight) featureLogs.Add($"Max Light({materialSelections.MaxLight.Count})");
            if (useBackLight) featureLogs.Add($"Back Light({materialSelections.BackLight.Count})");
            if (useShadow) featureLogs.Add($"Shadow({materialSelections.Shadow.Count})");
            if (useShadowX) featureLogs.Add($"Shadow X({materialSelections.ShadowX.Count})");
            if (useShadowY) featureLogs.Add($"Shadow Y({materialSelections.ShadowY.Count})");
            
            string features = string.Join(", ", featureLogs);

            EditorUtility.DisplayDialog("완료",
                $"밝기 조절 기능이 적용되었습니다!\n\n적용된 기능 (적용된 오브젝트 수):\n{features}", "확인");
        }

        private static List<string> GetSelectedMaterials(Dictionary<string, bool> materials)
        {
            return materials.Where(x => x.Value).Select(x => x.Key).ToList();
        }
    }
}
