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
    private PingPongBall pingPongBall;

    [SerializeField] 
    private float ballResetDelay = 1; // seconds

    private Vector3 ballStartPos;
    private Quaternion ballStartRot;

    public void Score()
    {
        player1Score++;
        player1ScoreDisplay.text = player1Score.ToString();
    }

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

    // Update is called once per frame
    void Update()
    {
        
    }
}
