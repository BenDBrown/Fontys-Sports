using System.Collections;
using TMPro;
using UnityEngine;

public class PingPongScoring : MonoBehaviour
{
    private static int player1Score = 0;
    private static int player2Score = 0;


    [SerializeField]
    private TextMeshProUGUI player1ScoreDisplay;
    [SerializeField] 
    private TextMeshProUGUI player2ScoreDisplay;
    [SerializeField]
    private TextMeshProUGUI winnertext;
    [SerializeField]
    private PingPongBall pingPongBall;

    [SerializeField] 
    private float ballResetDelay = 1; // seconds

    private Vector3 ballStartPos;
    private Quaternion ballStartRot;

    void Start()
    {
        ballStartPos = pingPongBall.transform.position;
        ballStartRot = pingPongBall.transform.rotation;
    }

    public void Reset()
    {
        StartCoroutine(ResetBall());
    }

    private IEnumerator ResetBall()
    {
        GameObject ballObj = pingPongBall.gameObject;
        yield return new WaitForSeconds(ballResetDelay);
        if (!ballObj.TryGetComponent(out Rigidbody ballRb))
        {
            Debug.LogError("ball did not have a rigidBody");
            yield break;
        }
        ballRb.linearVelocity = Vector3.zero;
        ballRb.angularVelocity = Vector3.zero;
        ballObj.transform.position = ballStartPos;
        ballObj.transform.rotation = ballStartRot;
    }

    public void onScore(bool player)
    {
        if (player)
        {
            player1Score++;
            updateUI();
        }
        else
        {
            player2Score++;
            updateUI();
        }
    }

    private void updateUI()
    {
        player1ScoreDisplay.text = player1Score.ToString();
        player2ScoreDisplay.text = player2Score.ToString();
        if (player1Score == 11)
        {
            winnertext.text = "You win!";
        }
        else if (player2Score == 11)
        {
            winnertext.text = "The AI wins!";
        }
    }
}
