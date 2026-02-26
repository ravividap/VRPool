using UnityEngine;

namespace VRPool
{
    /// <summary>
    /// Moves the world-space HUD panel to stay within the player's
    /// comfortable viewing area in VR. The panel gently follows the
    /// head direction with a configurable lag.
    /// </summary>
    public class WorldSpaceHUD : MonoBehaviour
    {
        [Header("Follow Settings")]
        [SerializeField] private Transform headTransform;      // Camera/head
        [SerializeField] private float followDistance = 1.5f;  // metres in front
        [SerializeField] private float verticalOffset  = -0.2f;
        [SerializeField] private float followSpeed     = 2f;
        [SerializeField] private float deadZoneDegrees = 30f;  // only follow outside this cone

        private Vector3 _targetPosition;
        private Quaternion _targetRotation;

        private void Start()
        {
            // Initialise target to the HUD's current position so it doesn't
            // lerp toward Vector3.zero on the first frame.
            _targetPosition = transform.position;
        }

        private void LateUpdate()
        {
            if (headTransform == null) return;

            // Desired position: in front of and slightly below the head
            Vector3 forward = Vector3.ProjectOnPlane(headTransform.forward, Vector3.up).normalized;
            Vector3 desired = headTransform.position
                              + forward * followDistance
                              + Vector3.up * verticalOffset;

            // Check if the head has turned far enough to warrant re-centering
            float angle = Vector3.Angle(transform.position - headTransform.position,
                                        forward * followDistance);
            if (angle > deadZoneDegrees)
                _targetPosition = desired;

            transform.position = Vector3.Lerp(transform.position, _targetPosition, 
                                              followSpeed * Time.deltaTime);

            // Always face the player
            transform.LookAt(headTransform.position);
            transform.forward = -transform.forward;
        }
    }
}
