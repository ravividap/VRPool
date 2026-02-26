using UnityEngine;

namespace VRPool
{
    /// <summary>
    /// Handles collision audio for each ball.
    /// Distinguishes between ball-ball and ball-cushion contacts
    /// and forwards the event to AudioManager.
    /// </summary>
    [RequireComponent(typeof(BallController))]
    public class BallCollisionAudio : MonoBehaviour
    {
        [SerializeField] private float minCollisionVelocity = 0.2f;

        private void OnCollisionEnter(Collision collision)
        {
            float vel = collision.relativeVelocity.magnitude;
            if (vel < minCollisionVelocity) return;

            if (AudioManager.Instance == null) return;

            if (collision.gameObject.CompareTag("Ball"))
                AudioManager.Instance.PlayBallHitBall(vel);
            else if (collision.gameObject.CompareTag("Cushion"))
                AudioManager.Instance.PlayBallHitCushion(vel);
        }
    }
}
