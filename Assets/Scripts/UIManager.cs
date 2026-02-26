using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace VRPool
{
    /// <summary>
    /// Manages all in-game UI: world-space HUD panels, score display,
    /// shot messages, and the game-over screen.
    /// </summary>
    public class UIManager : MonoBehaviour
    {
        [Header("HUD Panel")]
        [SerializeField] private GameObject hudPanel;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI shotsText;
        [SerializeField] private TextMeshProUGUI ballsRemainingText;
        [SerializeField] private TextMeshProUGUI messageText;

        [Header("Game Over Panel")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI finalScoreText;
        [SerializeField] private TextMeshProUGUI finalShotsText;
        [SerializeField] private Button playAgainButton;

        [Header("Message Settings")]
        [SerializeField] private float messageDuration = 2.5f;

        private float _messageTimer;

        private void Awake()
        {
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);

            if (messageText != null)
                messageText.text = string.Empty;

            if (playAgainButton != null)
                playAgainButton.onClick.AddListener(OnPlayAgainClicked);
        }

        private void Update()
        {
            if (_messageTimer > 0f)
            {
                _messageTimer -= Time.deltaTime;
                if (_messageTimer <= 0f && messageText != null)
                    messageText.text = string.Empty;
            }
        }

        /// <summary>Refresh the main HUD counters.</summary>
        public void UpdateHUD(int score, int shots, int ballsPocketed, int totalBalls)
        {
            if (scoreText != null)
                scoreText.text = $"Score: {score}";

            if (shotsText != null)
                shotsText.text = $"Shots: {shots}";

            if (ballsRemainingText != null)
                ballsRemainingText.text = $"Balls: {ballsPocketed}/{totalBalls}";
        }

        /// <summary>Show a temporary feedback message to the player.</summary>
        public void ShowMessage(string message)
        {
            if (messageText == null) return;
            messageText.text = message;
            _messageTimer = messageDuration;
        }

        /// <summary>Hide HUD and show the game-over panel.</summary>
        public void ShowGameOver(int finalScore, int totalShots)
        {
            if (hudPanel != null)
                hudPanel.SetActive(false);

            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);

            if (finalScoreText != null)
                finalScoreText.text = $"Final Score\n{finalScore}";

            if (finalShotsText != null)
                finalShotsText.text = $"Completed in {totalShots} shots";
        }

        private void OnPlayAgainClicked()
        {
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);

            if (hudPanel != null)
                hudPanel.SetActive(true);

            GameManager.Instance.StartGame();
        }
    }
}
