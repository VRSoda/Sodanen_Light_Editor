using UnityEngine;

namespace Brightness.Utility
{
    public class SelectiveAnimationClipSet
    {
        public AnimationClip MinLight { get; set; }
        public AnimationClip MaxLight { get; set; }
        public AnimationClip BackLight { get; set; }
        public AnimationClip BackLightHue { get; set; }
        public AnimationClip Shadow { get; set; }
        public AnimationClip ShadowXAngle { get; set; }
        public AnimationClip ShadowYAngle { get; set; }
    }
}
