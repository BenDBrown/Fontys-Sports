using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;

public class GolfBall : MonoBehaviour
{
    public UnityEvent GolfBallHit = new();
    public bool PoseInvalid = false;

    private bool hitCooldown = false;

    public Pose PrevPose { get; private set; }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag != "GolfClub") return;
        TriggerHit();
    }

    public void TriggerHit()
    {
        if (hitCooldown) return;
        StartCoroutine(HitCooldown());
        GolfBallHit?.Invoke();
        PrevPose = transform.GetWorldPose();
    }

    private IEnumerator HitCooldown()
    { 
        hitCooldown = true;
        yield return new WaitForSeconds(0.1f);
        hitCooldown = false;
    }
}
