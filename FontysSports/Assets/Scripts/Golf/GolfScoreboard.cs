using UnityEngine;

public class GolfScoreboard : MonoBehaviour
{
    [SerializeField]
    GolfPlayerScoreRow[] scoreboardRows;

    public void SetGolfPlayerScores(GolfPlayerScoreInfo[] scores)
    {
        for (int i = 0; i < scoreboardRows.Length; i++)
        {
            if (i >= scores.Length)
            {
                scoreboardRows[i].gameObject.SetActive(false);
                continue;
            }
            scoreboardRows[i].gameObject.SetActive(true);
            scoreboardRows[i].SetPlayerScoreInfo(scores[i]);
        }
    }
}
