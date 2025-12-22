using System.Collections.Generic;
using UnityEngine;
using Brightness.Localization;
using static Brightness.Localization.Loc;

namespace Brightness.Utility
{
    public partial class SodanenEditor
    {
        #region Feature Definitions

        private static List<FeatureInfo> CreateLightFeatures() => new()
        {
            new()
            {
                LabelKey = "feature.minlight",
                TooltipKey = "feature.minlight.tooltip",
                IsEnabled = t => t.MinLight,
                SetEnabled = (t, v) => t.MinLight = v,
                IsExpanded = s => s.ShowMinLight,
                SetExpanded = (s, v) => s.ShowMinLight = v,
                GetMaterials = s => s.MinLight
            },
            new()
            {
                LabelKey = "feature.maxlight",
                TooltipKey = "feature.maxlight.tooltip",
                IsEnabled = t => t.MaxLight,
                SetEnabled = (t, v) => t.MaxLight = v,
                IsExpanded = s => s.ShowMaxLight,
                SetExpanded = (s, v) => s.ShowMaxLight = v,
                GetMaterials = s => s.MaxLight
            },
            new()
            {
                LabelKey = "feature.backlight",
                TooltipKey = "feature.backlight.tooltip",
                IsEnabled = t => t.BackLight,
                SetEnabled = (t, v) => t.BackLight = v,
                IsExpanded = s => s.ShowBackLight,
                SetExpanded = (s, v) => s.ShowBackLight = v,
                GetMaterials = s => s.BackLight
            }
        };

        private static List<FeatureInfo> CreateShadowFeatures() => new()
        {
            new()
            {
                LabelKey = "feature.shadow_main",
                TooltipKey = "feature.shadow.tooltip",
                IsEnabled = t => t.Shadow,
                SetEnabled = (t, v) => t.Shadow = v,
                IsExpanded = s => s.ShowShadow,
                SetExpanded = (s, v) => s.ShowShadow = v,
                GetMaterials = s => s.Shadow
            },
            new()
            {
                LabelKey = "feature.shadow_x",
                TooltipKey = "feature.shadow_x.tooltip",
                IsEnabled = t => t.ShadowXAngle,
                SetEnabled = (t, v) => t.ShadowXAngle = v,
                IsExpanded = s => s.ShowShadowX,
                SetExpanded = (s, v) => s.ShowShadowX = v,
                GetMaterials = s => s.ShadowX
            },
            new()
            {
                LabelKey = "feature.shadow_y",
                TooltipKey = "feature.shadow_y.tooltip",
                IsEnabled = t => t.ShadowYAngle,
                SetEnabled = (t, v) => t.ShadowYAngle = v,
                IsExpanded = s => s.ShowShadowY,
                SetExpanded = (s, v) => s.ShowShadowY = v,
                GetMaterials = s => s.ShadowY
            }
        };

        #endregion

        #region Section - Features

        private void DrawFeatureSection()
        {
            SodanenEditorUI.DrawSectionBox(L("feature.title"), () =>
            {
                SodanenEditorUI.DrawGroupLabel(L("feature.light"));
                DrawFeatureSet(_lightFeatures);

                GUILayout.Space(8);

                SodanenEditorUI.DrawGroupLabel(L("feature.shadow"));
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

        #endregion
    }
}
