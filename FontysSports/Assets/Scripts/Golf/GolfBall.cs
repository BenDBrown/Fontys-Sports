using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;

public class GolfBall : MonoBehaviour
{
    public UnityEvent GolfBallHit = new();
    public bool PoseInvalid = false;

    private bool hitCooldown = false;

    public Pose PrevPose { get { return prevPose; } }
    private Pose prevPose;

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag != "GolfClub") return;
        TriggerHit();
    }

    public void TriggerHit()
    {
        if (hitCooldown) return;
        Debug.Log("golfball hit");
        StartCoroutine(HitCooldown());
        GolfBallHit?.Invoke();
        prevPose = transform.GetWorldPose();
    }

    private IEnumerator HitCooldown()
    { 
        hitCooldown = true;
        yield return new WaitForSeconds(0.1f);
        hitCooldown = false;
    }
}
