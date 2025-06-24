using System.Collections;
using UnityEngine;

public class WindMillReset : MonoBehaviour
{
    [SerializeField] private GolfBallReset golfBall;
    [SerializeField] private Transform tempRespawnPos;
    private int hitCount = 0;
    private readonly int hitCountLimit = 2;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball")) BallHitBlade();
    }

    private void BallHitBlade()
    {
        if (++hitCount >= hitCountLimit)
        {
            hitCount = 0;
            StartCoroutine(golfBall.BallReset(tempRespawnPos.position));
        }
        else StartCoroutine(HitCountReset());
    }

    private IEnumerator HitCountReset()
    {
        yield return new WaitForSeconds(1);
        hitCount = 0;
    }
}