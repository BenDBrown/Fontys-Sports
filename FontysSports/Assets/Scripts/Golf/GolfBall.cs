using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class GolfBall : MonoBehaviour
{
    public UnityEvent GolfBallHit = new();

    private bool hitCooldown = false;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag != "GolfClub" || hitCooldown) return;
        Debug.Log("golfball hit");
        StartCoroutine(HitCooldown());
        GolfBallHit?.Invoke();
    }

    private IEnumerator HitCooldown()
    { 
        hitCooldown = true;
        yield return new WaitForSeconds(0.1f);
        hitCooldown = false;
    }
}
