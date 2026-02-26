using UnityEngine;

namespace VRPool
{
    /// <summary>
    /// Draws a dotted-line aim guide from the cue tip through
    /// the cue ball, showing the predicted path of the cue ball
    /// and indicating which object ball will be hit first.
    /// </summary>
    public class AimGuide : MonoBehaviour
    {
        [Header("Line Renderers")]
        [SerializeField] private LineRenderer cueBallLine;     // cue ball travel path
        [SerializeField] private LineRenderer objectBallLine;  // predicted object ball path

        [Header("Settings")]
        [SerializeField] private float maxGuideLength = 2.0f;
        [SerializeField] private LayerMask ballLayerMask;
        [SerializeField] private LayerMask tableLayerMask;
        [SerializeField] private bool showObjectBallPath = true;

        [Header("Dot Material")]
        [SerializeField] private Material guideMaterial;

        private bool _visible;

        private void Awake()
        {
            SetVisible(false);
        }

        /// <summary>Show or hide the aim guide.</summary>
        public void SetVisible(bool visible)
        {
            _visible = visible;
            if (cueBallLine  != null) cueBallLine.enabled  = visible;
            if (objectBallLine != null) objectBallLine.enabled = visible && showObjectBallPath;
        }

        /// <summary>
        /// Update the guide from the cue tip position toward the cue ball.
        /// Call every frame while the player is aiming.
        /// </summary>
        public void UpdateGuide(Vector3 cueTipPosition, Vector3 cueBallPosition)
        {
            if (!_visible) return;

            Vector3 direction = (cueBallPosition - cueTipPosition).normalized;

            // Ray from cue ball along shot direction
            if (Physics.SphereCast(cueBallPosition, 0.028f, direction,
                                   out RaycastHit hit, maxGuideLength, ballLayerMask))
            {
                DrawLine(cueBallLine, cueBallPosition, hit.point);

                if (showObjectBallPath && hit.collider.TryGetComponent<BallController>(out _))
                {
                    // Reflect from contact normal for object ball path
                    Vector3 reflectedDir = Vector3.Reflect(direction, hit.normal);
                    DrawLine(objectBallLine, hit.point,
                             hit.point + reflectedDir * maxGuideLength);
                }
                else
                {
                    DrawLine(objectBallLine, Vector3.zero, Vector3.zero);
                }
            }
            else
            {
                // No ball in path – draw full guide line
                DrawLine(cueBallLine, cueBallPosition,
                         cueBallPosition + direction * maxGuideLength);
                DrawLine(objectBallLine, Vector3.zero, Vector3.zero);
            }
        }

        private void DrawLine(LineRenderer lr, Vector3 start, Vector3 end)
        {
            if (lr == null) return;
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
        }
    }
}
