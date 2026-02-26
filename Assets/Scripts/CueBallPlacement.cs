using UnityEngine;

namespace VRPool
{
    /// <summary>
    /// Allows the player to place the cue ball anywhere behind the head
    /// string (kitchen area) after a scratch. The player grabs and releases
    /// the cue ball using the grip trigger on either controller.
    /// </summary>
    public class CueBallPlacement : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private BallController cueBall;
        [SerializeField] private PoolTableManager tableManager;
        [SerializeField] private Transform headStringTransform;

        [Header("Input")]
        [SerializeField] private UnityEngine.InputSystem.InputActionProperty rightGripAction;
        [SerializeField] private UnityEngine.InputSystem.InputActionProperty leftGripAction;

        [Header("Controllers")]
        [SerializeField] private Transform rightControllerTransform;
        [SerializeField] private Transform leftControllerTransform;

        private bool _placementMode;
        private bool _isHolding;
        private Transform _holdingController;

        public void BeginPlacement()
        {
            _placementMode = true;
            cueBall.Rigidbody.isKinematic = true;
            cueBall.gameObject.SetActive(true);
        }

        private void Update()
        {
            if (!_placementMode) return;

            bool rightGrip = rightGripAction.action?.IsPressed() ?? false;
            bool leftGrip  = leftGripAction.action?.IsPressed()  ?? false;

            if (!_isHolding)
            {
                // Pick up cue ball if controller is near it
                if (rightGrip && IsNearBall(rightControllerTransform))
                    StartHolding(rightControllerTransform);
                else if (leftGrip && IsNearBall(leftControllerTransform))
                    StartHolding(leftControllerTransform);
            }
            else
            {
                if (!rightGrip && !leftGrip)
                    TryPlaceBall();
                else
                    MoveBallWithController();
            }
        }

        private bool IsNearBall(Transform controller)
        {
            return Vector3.Distance(controller.position, cueBall.transform.position) < 0.15f;
        }

        private void StartHolding(Transform controller)
        {
            _isHolding = true;
            _holdingController = controller;
        }

        private void MoveBallWithController()
        {
            Vector3 pos = _holdingController.position;
            // Constrain to table surface height
            pos.y = tableManager.TableCentre.y + 0.028f;
            cueBall.transform.position = pos;
        }

        private void TryPlaceBall()
        {
            Vector3 pos = cueBall.transform.position;

            if (tableManager.IsOnTable(pos) && IsBehindHeadString(pos))
            {
                // Valid placement — release ball and return control to the player
                cueBall.Rigidbody.isKinematic = false;
                _placementMode = false;
                _isHolding     = false;
                // Transition back to PlayerTurn so the cue is re-enabled
                GameManager.Instance.ResumePlayerTurn();
            }
            // else: keep in placement mode until valid drop
        }

        private bool IsBehindHeadString(Vector3 worldPos)
        {
            if (headStringTransform == null) return true;
            return worldPos.z <= headStringTransform.position.z;
        }
    }
}
