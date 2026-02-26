using UnityEngine;

namespace VRPool
{
    /// <summary>
    /// Provides visual feedback for the cue's shot power.
    /// Scales and tints a bar or sphere mesh along a gradient
    /// from green (low power) to red (full power).
    /// Attach to a child GameObject of the cue stick.
    /// </summary>
    public class ShotPowerIndicator : MonoBehaviour
    {
        [Header("Visuals")]
        [SerializeField] private Renderer indicatorRenderer;
        [SerializeField] private Transform indicatorBar;

        [Header("Gradient")]
        [SerializeField] private Color lowPowerColor  = Color.green;
        [SerializeField] private Color highPowerColor = Color.red;

        [Header("Scale")]
        [SerializeField] private Vector3 minScale = new Vector3(0.02f, 0.02f, 0.02f);
        [SerializeField] private Vector3 maxScale = new Vector3(0.02f, 0.02f, 0.4f);

        private static readonly int ColorProperty = Shader.PropertyToID("_BaseColor");

        private MaterialPropertyBlock _mpb;

        private void Awake()
        {
            _mpb = new MaterialPropertyBlock();
        }

        /// <summary>
        /// Set the displayed power level (0 = no power, 1 = full power).
        /// </summary>
        public void SetPower(float normalised)
        {
            normalised = Mathf.Clamp01(normalised);

            if (indicatorBar != null)
                indicatorBar.localScale = Vector3.Lerp(minScale, maxScale, normalised);

            if (indicatorRenderer != null)
            {
                Color col = Color.Lerp(lowPowerColor, highPowerColor, normalised);
                indicatorRenderer.GetPropertyBlock(_mpb);
                _mpb.SetColor(ColorProperty, col);
                indicatorRenderer.SetPropertyBlock(_mpb);
            }
        }
    }
}
