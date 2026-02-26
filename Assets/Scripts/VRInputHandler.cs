using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace VRPool
{
    /// <summary>
    /// Handles Quest 3-specific VR input configuration.
    /// Manages controller tracking, boundary checking,
    /// and forwards button events to game systems.
    /// </summary>
    public class VRInputHandler : MonoBehaviour
    {
        [Header("XR Rig References")]
        [SerializeField] private Transform xrRig;
        [SerializeField] private Transform leftControllerTransform;
        [SerializeField] private Transform rightControllerTransform;
        [SerializeField] private XRBaseController leftController;
        [SerializeField] private XRBaseController rightController;

        [Header("Menu")]
        [SerializeField] private GameObject pauseMenuPanel;
        [SerializeField] private UnityEngine.InputSystem.InputActionProperty menuButtonAction;

        private bool _paused;

        private void OnEnable()
        {
            menuButtonAction.action?.Enable();
            menuButtonAction.action.performed += OnMenuPressed;
        }

        private void OnDisable()
        {
            menuButtonAction.action.performed -= OnMenuPressed;
            menuButtonAction.action?.Disable();
        }

        private void OnMenuPressed(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
        {
            TogglePause();
        }

        private void TogglePause()
        {
            _paused = !_paused;
            Time.timeScale = _paused ? 0f : 1f;

            if (pauseMenuPanel != null)
                pauseMenuPanel.SetActive(_paused);
        }

        /// <summary>Returns world-space position of the right controller tip.</summary>
        public Vector3 RightControllerPosition =>
            rightControllerTransform != null
                ? rightControllerTransform.position
                : Vector3.zero;

        /// <summary>Returns world-space position of the left controller tip.</summary>
        public Vector3 LeftControllerPosition =>
            leftControllerTransform != null
                ? leftControllerTransform.position
                : Vector3.zero;
    }
}
