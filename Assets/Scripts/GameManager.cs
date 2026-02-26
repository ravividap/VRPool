using UnityEngine;

namespace VRPool
{
    /// <summary>
    /// Manages overall game state, turn flow, win/loss conditions,
    /// and score tracking for the single-player VR pool game.
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [Header("Ball References")]
        [SerializeField] private BallController cueBall;
        [SerializeField] private BallController[] objectBalls;

        [Header("Game Settings")]
        [SerializeField] private int totalBallCount = 15;
        [SerializeField] private float ballStopThreshold = 0.01f;
        [SerializeField] private float turnTimeoutSeconds = 30f;

        [Header("References")]
        [SerializeField] private UIManager uiManager;
        [SerializeField] private CueController cueController;
        [SerializeField] private BallRack ballRack;
        [SerializeField] private CueBallPlacement cueBallPlacement;

        public GameState CurrentState { get; private set; } = GameState.Idle;
        public int Score { get; private set; }
        public int ShotsTaken { get; private set; }
        public int BallsPocketed { get; private set; }

        private bool _waitingForBallsToStop;
        private bool _cueBallPocketed;
        private int _lastPocketedCount;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Start()
        {
            StartGame();
        }

        /// <summary>Initialises a new game session.</summary>
        public void StartGame()
        {
            Score = 0;
            ShotsTaken = 0;
            BallsPocketed = 0;
            _cueBallPocketed = false;
            _lastPocketedCount = 0;

            ballRack.RackBalls();
            SetState(GameState.PlayerTurn);
            uiManager.UpdateHUD(Score, ShotsTaken, BallsPocketed, totalBallCount);
        }

        /// <summary>Called by CueController when the player takes a shot.</summary>
        public void OnShotTaken()
        {
            if (CurrentState != GameState.PlayerTurn)
                return;

            ShotsTaken++;
            SetState(GameState.WaitingForBalls);
            _waitingForBallsToStop = true;
            _cueBallPocketed = false;
        }

        /// <summary>Called by PocketDetector when a ball enters a pocket.</summary>
        public void OnBallPocketed(BallController ball)
        {
            if (ball.IsCueBall)
            {
                _cueBallPocketed = true;
                return;
            }

            BallsPocketed++;
            Score += CalculateBallScore(ball);
            ball.SetPocketed();
            uiManager.UpdateHUD(Score, ShotsTaken, BallsPocketed, totalBallCount);
        }

        private void Update()
        {
            if (_waitingForBallsToStop && CurrentState == GameState.WaitingForBalls)
            {
                if (AllBallsStopped())
                {
                    _waitingForBallsToStop = false;
                    ProcessEndOfShot();
                }
            }
        }

        private void ProcessEndOfShot()
        {
            if (_cueBallPocketed)
            {
                HandleScratch();
                return;
            }

            if (BallsPocketed >= totalBallCount)
            {
                HandleGameWon();
                return;
            }

            bool pocketedThisTurn = BallsPocketed > _lastPocketedCount;
            _lastPocketedCount = BallsPocketed;

            SetState(GameState.PlayerTurn);
            cueController.EnableCue(true);

            if (pocketedThisTurn)
                uiManager.ShowMessage("Great shot! Keep going!");
        }

        private void HandleScratch()
        {
            Score = Mathf.Max(0, Score - 5);
            cueBall.Respawn();
            SetState(GameState.CueBallInHand);
            cueController.EnableCue(false);
            cueBallPlacement?.BeginPlacement();
            uiManager.ShowMessage("Scratch! -5 points. Place the cue ball.");
            uiManager.UpdateHUD(Score, ShotsTaken, BallsPocketed, totalBallCount);
        }

        /// <summary>
        /// Called by CueBallPlacement after the player has placed the cue ball.
        /// Re-enables the cue and returns to PlayerTurn state.
        /// </summary>
        public void ResumePlayerTurn()
        {
            SetState(GameState.PlayerTurn);
            cueController.EnableCue(true);
        }

        private void HandleGameWon()
        {
            SetState(GameState.GameOver);
            cueController.EnableCue(false);
            uiManager.ShowGameOver(Score, ShotsTaken);
        }

        private bool AllBallsStopped()
        {
            if (cueBall != null && !cueBall.IsPocketed &&
                cueBall.Rigidbody.linearVelocity.magnitude > ballStopThreshold)
                return false;

            foreach (var ball in objectBalls)
            {
                if (ball != null && !ball.IsPocketed &&
                    ball.Rigidbody.linearVelocity.magnitude > ballStopThreshold)
                    return false;
            }

            return true;
        }

        private int CalculateBallScore(BallController ball)
        {
            // Solid balls (1-7) = 1 point, Stripe balls (9-15) = 2 points, 8-ball = 5 points
            if (ball.BallNumber == 8) return 5;
            if (ball.BallNumber >= 9) return 2;
            return 1;
        }

        private void SetState(GameState newState)
        {
            CurrentState = newState;
        }
    }

    public enum GameState
    {
        Idle,
        PlayerTurn,
        WaitingForBalls,
        CueBallInHand,
        GameOver
    }
}
