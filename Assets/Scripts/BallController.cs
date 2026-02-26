using UnityEngine;

namespace VRPool
{
    /// <summary>
    /// Represents a single pool ball. Handles physics, pocket detection,
    /// respawn for the cue ball, and exposes key properties to other systems.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(SphereCollider))]
    public class BallController : MonoBehaviour
    {
        [Header("Ball Identity")]
        [SerializeField] private int ballNumber;   // 0 = cue ball, 1-15 = object balls
        [SerializeField] private bool isCueBall;

        [Header("Physics Settings")]
        [SerializeField] private float rollingFriction = 0.05f;
        [SerializeField] private float spinDrag = 1.5f;

        [Header("Respawn")]
        [SerializeField] private Vector3 cueBallSpawnPosition = new Vector3(0f, 0.875f, -0.6f);

        public int BallNumber => ballNumber;
        public bool IsCueBall => isCueBall;
        public bool IsPocketed { get; private set; }
        public Rigidbody Rigidbody { get; private set; }

        private Vector3 _startPosition;
        private Quaternion _startRotation;

        private void Awake()
        {
            Rigidbody = GetComponent<Rigidbody>();
            Rigidbody.mass = 0.17f;          // regulation pool ball ~170 g
            Rigidbody.angularDamping = spinDrag;
            Rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            _startPosition = transform.position;
            _startRotation = transform.rotation;
        }

        private void FixedUpdate()
        {
            if (IsPocketed) return;

            // Apply rolling friction proportional to horizontal velocity
            Vector3 velocity = Rigidbody.linearVelocity;
            if (velocity.magnitude > 0.01f)
            {
                Vector3 frictionForce = -velocity.normalized * rollingFriction;
                Rigidbody.AddForce(frictionForce, ForceMode.VelocityChange);
            }
        }

        /// <summary>Mark ball as pocketed and disable its physics.</summary>
        public void SetPocketed()
        {
            IsPocketed = true;
            Rigidbody.isKinematic = true;
            Rigidbody.linearVelocity = Vector3.zero;
            Rigidbody.angularVelocity = Vector3.zero;
            gameObject.SetActive(false);
        }

        /// <summary>Respawn the cue ball at the default position (after a scratch).</summary>
        public void Respawn()
        {
            if (!isCueBall) return;

            IsPocketed = false;
            gameObject.SetActive(true);
            transform.position = cueBallSpawnPosition;
            transform.rotation = _startRotation;
            Rigidbody.isKinematic = false;
            Rigidbody.linearVelocity = Vector3.zero;
            Rigidbody.angularVelocity = Vector3.zero;
        }

        /// <summary>Reset ball to its rack position (used at start of game).</summary>
        public void ResetToStart()
        {
            IsPocketed = false;
            gameObject.SetActive(true);
            transform.position = _startPosition;
            transform.rotation = _startRotation;
            Rigidbody.isKinematic = false;
            Rigidbody.linearVelocity = Vector3.zero;
            Rigidbody.angularVelocity = Vector3.zero;
        }

        /// <summary>Override the start position from the rack setup script.</summary>
        public void SetStartPosition(Vector3 position)
        {
            _startPosition = position;
            transform.position = position;
        }
    }
}
