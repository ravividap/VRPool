using UnityEngine;

namespace VRPool
{
    /// <summary>
    /// Arranges the 15 object balls in a standard triangle rack
    /// at the foot spot of the pool table.
    /// Ball order: 1(front), 2,9,3,8,10,4,14,7,11,5,13,6,15,12 (standard rack)
    /// </summary>
    public class BallRack : MonoBehaviour
    {
        [Header("Rack Position")]
        [SerializeField] private Transform footSpot;          // foot-spot marker on table
        [SerializeField] private float ballDiameter = 0.057f; // regulation 2-1/4 inch ball

        [Header("Ball References (1–15)")]
        [SerializeField] private BallController[] objectBalls; // 15 balls, index 0 = ball #1
        [SerializeField] private BallController cueBall;

        [Header("Cue Ball Spawn")]
        [SerializeField] private Transform headString;        // head-string position marker

        // Standard rack order (row by row, front to back)
        private static readonly int[] RackOrder =
        {
            1,
            2, 9,
            3, 8, 10,
            4, 14, 7, 11,
            5, 13, 6, 15, 12
        };

        /// <summary>Place all balls in their starting positions.</summary>
        public void RackBalls()
        {
            // Reset and hide all balls first
            foreach (var ball in objectBalls)
                ball.ResetToStart();

            // Place each ball
            int idx = 0;
            for (int row = 0; row < 5; row++)
            {
                int ballsInRow = row + 1;
                for (int col = 0; col < ballsInRow; col++)
                {
                    int ballNum = RackOrder[idx++];
                    Vector3 pos = CalculateRackPosition(row, col);
                    BallController ball = GetBallByNumber(ballNum);
                    if (ball != null)
                        ball.SetStartPosition(pos);
                }
            }

            // Place cue ball at head string
            if (cueBall != null && headString != null)
            {
                cueBall.Respawn();
                cueBall.transform.position = headString.position;
            }
        }

        private Vector3 CalculateRackPosition(int row, int col)
        {
            float spacing = ballDiameter + 0.002f; // tiny gap to avoid initial overlap
            float rowDepth = row * spacing * Mathf.Sin(60f * Mathf.Deg2Rad);

            // Centre column within each row
            float rowWidth = col * spacing - row * spacing * 0.5f;

            Vector3 spotPos = footSpot != null
                ? footSpot.position
                : new Vector3(0f, 0.875f, 0.6f);

            return new Vector3(
                spotPos.x + rowWidth,
                spotPos.y,
                spotPos.z + rowDepth
            );
        }

        private BallController GetBallByNumber(int number)
        {
            foreach (var ball in objectBalls)
            {
                if (ball != null && ball.BallNumber == number)
                    return ball;
            }
            return null;
        }
    }
}
