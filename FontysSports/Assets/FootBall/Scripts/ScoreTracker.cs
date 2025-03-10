using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using System.Runtime.InteropServices;
using System;

public class ScoreTracker : MonoBehaviour
{
    private static int score = 0;

    private static float fastestGoalSpeed = 0;

    public UnityEvent RoundEnded;

    [SerializeField] private TextMeshProUGUI goalAmountDisplay;
    [SerializeField] private TextMeshProUGUI missAmountDisplay;
    [SerializeField] private TextMeshProUGUI currentGoalSpeedDisplay;
    [SerializeField] private TextMeshProUGUI fastestGoalSpeedDisplay;

    [SerializeField] private bool resetOnLoad = false;

    [SerializeField] private int shotsTillEnd = 1;

    [SerializeField] private Football ball;

    [SerializeField] private float ballResetDelay = 1; // seconds

    private int shotNr = 0;

    private Vector3 ballStartPos;

    private Quaternion ballStartRot;

    public void OnScore(float speed)
    {
        score++;
        shotNr++;
        if(speed > fastestGoalSpeed) fastestGoalSpeed = speed;
        UpdateUi(speed);
        if (shotNr >= shotsTillEnd) RoundEnded?.Invoke();
        StartCoroutine(ResetBall());
    }

    public void NextShot()
    {
        shotNr++;
        UpdateUi();
        if (shotNr >= shotsTillEnd) RoundEnded?.Invoke();
        StartCoroutine(ResetBall());
    }

    private void Start()
    {
        if (resetOnLoad) score = 0;
        ballStartPos = ball.transform.position;
        ballStartRot = ball.transform.rotation;
    }

    private void UpdateUi(float? currentGoalSpeed = null)
    {
        goalAmountDisplay.text = score.ToString();
        missAmountDisplay.text = (shotNr - score).ToString();
        if(currentGoalSpeed != null) currentGoalSpeedDisplay.text = GetRoundedKmPerHourSpeed((float)currentGoalSpeed).ToString();
        fastestGoalSpeedDisplay.text = GetRoundedKmPerHourSpeed(fastestGoalSpeed).ToString();
    }

    private int GetRoundedKmPerHourSpeed(float speed) => (int)(speed * 3.6f);

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
