using TMPro;
using UnityEngine;

public class GolfBallHitHandler : MonoBehaviour
{
    [SerializeField] private Transform clubHead;
    [SerializeField] private Rigidbody ballRigid;
    [SerializeField] private float forceMultiplier = 1;
 
    private Vector3 lastClubHeadPos;
    private bool hit = false;

    private void FixedUpdate()
    { 
        if (hit && ballRigid.linearVelocity.magnitude <= 0.1f)
        {
            Vector3 clubVelocity = (clubHead.position - lastClubHeadPos) / Time.fixedDeltaTime;
            Vector3 hitDirection = clubVelocity.normalized;
            float hitStrength = clubVelocity.magnitude * forceMultiplier;
            ballRigid.AddForce(hitDirection * hitStrength, ForceMode.Impulse);
        }
        lastClubHeadPos = clubHead.position;
        hit = false;
    }

    private void OnCollisionEnter(Collision collision) => hit = true;
}