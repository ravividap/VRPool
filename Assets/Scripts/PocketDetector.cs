using UnityEngine;

namespace VRPool
{
    /// <summary>
    /// Detects when a ball enters a pocket and notifies the GameManager.
    /// Attach to each pocket trigger collider on the table.
    /// </summary>
    [RequireComponent(typeof(SphereCollider))]
    public class PocketDetector : MonoBehaviour
    {
        [SerializeField] private int pocketIndex;

        private SphereCollider _trigger;

        private void Awake()
        {
            _trigger = GetComponent<SphereCollider>();
            _trigger.isTrigger = true;
            _trigger.radius = 0.055f;    // slightly larger than ball radius (0.028 m)
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<BallController>(out var ball))
            {
                if (!ball.IsPocketed)
                    GameManager.Instance.OnBallPocketed(ball);
            }
        }
    }
}
