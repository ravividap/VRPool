using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

namespace VRPool
{
    /// <summary>
    /// Controls the VR pool cue. The player grabs the cue with both controllers,
    /// aims it at the cue ball, and pulls back then pushes forward to take a shot.
    /// Provides haptic feedback on collision and shot release.
    /// </summary>
    public class CueController : MonoBehaviour
    {
        [Header("Cue Physics")]
        [SerializeField] private float minShotForce = 1f;
        [SerializeField] private float maxShotForce = 25f;
        [SerializeField] private float maxPullBackDistance = 0.4f;

        [Header("Cue Components")]
        [SerializeField] private Transform cueTip;
        [SerializeField] private Transform cueStick;
        [SerializeField] private ShotPowerIndicator powerIndicator;

        [Header("VR Input")]
        [SerializeField] private XRBaseController rightController;
        [SerializeField] private XRBaseController leftController;
        [SerializeField] private InputActionProperty gripActionRight;
        [SerializeField] private InputActionProperty gripActionLeft;

        [Header("Haptics")]
        [SerializeField] private float hapticAmplitude = 0.3f;
        [SerializeField] private float hapticDuration = 0.05f;

        private bool _isGripped;
        private bool _isAiming;
        private float _pullBackDistance;
        private Vector3 _aimStartPosition;
        private Vector3 _cueForwardAtAimStart;
        private bool _cueEnabled = true;

        private void Update()
        {
            if (!_cueEnabled) return;

            bool rightGrip = gripActionRight.action?.IsPressed() ?? false;
            bool leftGrip = gripActionLeft.action?.IsPressed() ?? false;

            if (rightGrip && leftGrip)
            {
                if (!_isGripped)
                    BeginGrip();

                UpdateAim();
            }
            else if (_isGripped)
            {
                ReleaseShot();
            }
        }

        private void BeginGrip()
        {
            _isGripped = true;
            _isAiming = false;
            _pullBackDistance = 0f;
            _aimStartPosition = cueStick.position;
            _cueForwardAtAimStart = cueStick.forward;
        }

        private void UpdateAim()
        {
            // Calculate how far the player has pulled the cue back
            Vector3 currentPos = cueStick.position;
            Vector3 delta = currentPos - _aimStartPosition;
            float pullSign = Vector3.Dot(delta, -_cueForwardAtAimStart);

            _pullBackDistance = Mathf.Clamp(pullSign, 0f, maxPullBackDistance);

            float power = _pullBackDistance / maxPullBackDistance;
            powerIndicator.SetPower(power);

            if (_pullBackDistance > 0.01f)
                _isAiming = true;
        }

        private void ReleaseShot()
        {
            _isGripped = false;

            if (!_isAiming || GameManager.Instance.CurrentState != GameState.PlayerTurn)
            {
                _pullBackDistance = 0f;
                powerIndicator.SetPower(0f);
                return;
            }

            float shotForce = Mathf.Lerp(minShotForce, maxShotForce,
                _pullBackDistance / maxPullBackDistance);

            // Detect cue ball at tip
            Collider[] hits = Physics.OverlapSphere(cueTip.position, 0.035f);
            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<BallController>(out var ball) && ball.IsCueBall)
                {
                    Vector3 direction = cueStick.forward.normalized;
                    ball.Rigidbody.AddForce(direction * shotForce, ForceMode.Impulse);
                    TriggerHaptics();
                    GameManager.Instance.OnShotTaken();
                    EnableCue(false);
                    break;
                }
            }

            _pullBackDistance = 0f;
            powerIndicator.SetPower(0f);
            _isAiming = false;
        }

        /// <summary>Enable or disable the cue (disabled while balls are rolling).</summary>
        public void EnableCue(bool enabled)
        {
            _cueEnabled = enabled;
            cueStick.gameObject.SetActive(enabled);
            powerIndicator.gameObject.SetActive(enabled);

            if (!enabled)
            {
                _isGripped = false;
                _isAiming = false;
                _pullBackDistance = 0f;
            }
        }

        private void TriggerHaptics()
        {
            rightController?.SendHapticImpulse(hapticAmplitude, hapticDuration);
            leftController?.SendHapticImpulse(hapticAmplitude, hapticDuration);
        }
    }
}
