using TMPro;
using UnityEngine;

public class GolfBallHitHandler : MonoBehaviour
{
    [SerializeField] private Transform clubHead;
    [SerializeField] private Rigidbody ballRigid;
    [SerializeField] private float forceMultiplier = 1;
    [Space(10)]
    [SerializeField] private Transform[] followers;

    private Vector3 previousPos = Vector3.zero;
    private int followerToSet = 0;
    private bool hit = false;

    private void Start()
    {
        foreach (Transform t in followers) t.GetComponent<GolfClubHeadFollower>().SetGolfBallHitHandler(this);
    }

    private void Update()
    {
        if (followerToSet == followers.Length) followerToSet = 0;
        followers[followerToSet++].position = previousPos + ((clubHead.position - previousPos) * 0.5f);
        previousPos = clubHead.position;
    }

    public void ForceBall(Vector3 clubVelocity)
    {
        float ballSpeed = ballRigid.linearVelocity.magnitude;
        if (hit && ballSpeed >= 0.1f) return;
        Vector3 hitDirection = clubVelocity.normalized;
        float hitStrength = clubVelocity.magnitude * forceMultiplier;
        ballRigid.AddForce(hitDirection * hitStrength, ForceMode.Impulse);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball")) hit = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ball")) hit = false;
    }
}