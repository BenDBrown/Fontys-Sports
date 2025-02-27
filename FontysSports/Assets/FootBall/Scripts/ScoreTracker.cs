using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class ScoreTracker : MonoBehaviour
{
    private static int score = 0;

    public UnityEvent RoundEnded;

    [SerializeField] private bool resetOnLoad = false;

    [SerializeField] private int shotsTillEnd = 1;

    [SerializeField] private Football ball;

    [SerializeField] private float ballResetDelay = 1; // seconds

    private int shotNr = 0;

    private Vector3 ballStartPos;

    private Quaternion ballStartRot;

    public void IncrementScore()
    {
        score++;
        shotNr++;
        if (shotNr >= shotsTillEnd)
        {
            RoundEnded?.Invoke();
            Debug.Log("round ended");
        }
        StartCoroutine(ResetBall());
        Debug.Log($"shot:{shotNr}\nscore:{score}");
    }

    public void NextShot()
    {
        shotNr++;
        if (shotNr >= shotsTillEnd) RoundEnded?.Invoke();
        StartCoroutine(ResetBall());
        Debug.Log($"shot:{shotNr}\nscore:{score}");
    }

    private void Start()
    {
        if (resetOnLoad) score = 0;
        ballStartPos = ball.transform.position;
        ballStartRot = ball.transform.rotation;
    }

    private IEnumerator ResetBall()
    { 
        GameObject ballObj = ball.gameObject;
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


}
