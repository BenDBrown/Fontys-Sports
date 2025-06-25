using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class WindMillBladesTrigger : MonoBehaviour
{
    public UnityEvent BallStuck;

    [SerializeField] private Rigidbody golfBallRigid;
    [SerializeField] private float bladeHitResetDelay = 1;
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
            golfBallRigid.linearVelocity = Vector3.zero;
            golfBallRigid.angularVelocity = Vector3.zero;
            BallStuck?.Invoke();
        }
        else StartCoroutine(HitCountReset());
    }

    private IEnumerator HitCountReset()
    {
        yield return new WaitForSeconds(bladeHitResetDelay);
        hitCount = 0;
    }
}