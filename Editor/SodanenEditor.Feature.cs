using System.Collections.Generic;
using UnityEngine;

namespace Brightness.Utility
{
    public partial class SodanenEditor
    {
        private List<FeatureInfo> CreateLightFeatures()
        {
            return new List<FeatureInfo>
            {
                new FeatureInfo
                {
                    Label = "Min Light",
                    Tooltip = "어두운 환경에서의 최소 밝기 제한",
                    IsEnabled = t => t.MinLight,
                    SetEnabled = (t, v) => t.MinLight = v,
                    IsExpanded = s => s.ShowMinLight,
                    SetExpanded = (s, v) => s.ShowMinLight = v,
                    GetMaterials = s => s.MinLight
                },
                new FeatureInfo
                {
                    Label = "Max Light",
                    Tooltip = "밝은 환경에서의 최대 밝기 제한",
                    IsEnabled = t => t.MaxLight,
                    SetEnabled = (t, v) => t.MaxLight = v,
                    IsExpanded = s => s.ShowMaxLight,
                    SetExpanded = (s, v) => s.ShowMaxLight = v,
                    GetMaterials = s => s.MaxLight
                },
                new FeatureInfo
                {
                    Label = "Back Light",
                    Tooltip = "뒤에서 오는 빛의 강도",
                    IsEnabled = t => t.BackLight,
                    SetEnabled = (t, v) => t.BackLight = v,
                    IsExpanded = s => s.ShowBackLight,
                    SetExpanded = (s, v) => s.ShowBackLight = v,
                    GetMaterials = s => s.BackLight
                }
            };
        }

        private List<FeatureInfo> CreateShadowFeatures()
        {
            return new List<FeatureInfo>
            {
                new FeatureInfo
                {
                    Label = "Shadow",
                    Tooltip = "그림자 방향 조절",
                    IsEnabled = t => t.Shadow,
                    SetEnabled = (t, v) => t.Shadow = v,
                    IsExpanded = s => s.ShowShadow,
                    SetExpanded = (s, v) => s.ShowShadow = v,
                    GetMaterials = s => s.Shadow
                },
                new FeatureInfo
                {
                    Label = "Shadow X",
                    Tooltip = "X축 그림자 방향",
                    IsEnabled = t => t.ShadowXAngle,
                    SetEnabled = (t, v) => t.ShadowXAngle = v,
                    IsExpanded = s => s.ShowShadowX,
                    SetExpanded = (s, v) => s.ShowShadowX = v,
                    GetMaterials = s => s.ShadowX
                },
                new FeatureInfo
                {
                    Label = "Shadow Y",
                    Tooltip = "Y축 그림자 방향",
                    IsEnabled = t => t.ShadowYAngle,
                    SetEnabled = (t, v) => t.ShadowYAngle = v,
                    IsExpanded = s => s.ShowShadowY,
                    SetExpanded = (s, v) => s.ShowShadowY = v,
                    GetMaterials = s => s.ShadowY
                }
            };
        }

        private void DrawFeatureSection()
        {
            SodanenEditorUI.DrawSectionBox("기능 선택", () =>
            {
                GUILayout.Space(5);
                SodanenEditorUI.DrawGroupLabel("Light");
                DrawFeatureSet(_lightFeatures);

                GUILayout.Space(8);
                SodanenEditorUI.DrawGroupLabel("Shadow");
                DrawFeatureSet(_shadowFeatures);
            });
        }

        private void DrawFeatureSet(List<FeatureInfo> features)
        {
            foreach (var feature in features)
            {
                var isEnabled = feature.IsEnabled(_featureToggles);
                var isExpanded = feature.IsExpanded(_materialSelections);
                var materials = feature.GetMaterials(_materialSelections);

                SodanenEditorComponents.DrawFeatureWithMaterials(
                    feature.Label, feature.Tooltip,
                    ref isEnabled, ref isExpanded, materials, _allMaterialPaths);

                feature.SetEnabled(_featureToggles, isEnabled);
                feature.SetExpanded(_materialSelections, isExpanded);
            }
        }
    }
}
