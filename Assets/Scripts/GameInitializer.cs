using UnityEngine;
using UnityEngine.XR.Management;

namespace VRPool
{
    /// <summary>
    /// Bootstrap script that configures the XR subsystem for Meta Quest 3
    /// and ensures the XR rig is properly positioned at the table.
    /// Attach to a persistent GameObject in the main scene.
    /// </summary>
    public class GameInitializer : MonoBehaviour
    {
        [Header("XR Rig")]
        [SerializeField] private GameObject xrRig;
        [SerializeField] private Vector3 xrRigStartPosition = new Vector3(0f, 0f, -1.8f);

        [Header("Game Systems")]
        [SerializeField] private GameManager gameManager;
        [SerializeField] private AudioManager audioManager;

        private void Awake()
        {
            Application.targetFrameRate = 72; // Quest 3 minimum comfortable rate
            Screen.sleepTimeout = SleepTimeout.NeverSleep;

            PositionRig();
        }

        private void PositionRig()
        {
            if (xrRig != null)
                xrRig.transform.position = xrRigStartPosition;
        }
    }
}
