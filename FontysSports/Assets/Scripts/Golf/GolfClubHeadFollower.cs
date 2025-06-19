using UnityEngine;

public class GolfClubHeadFollower : MonoBehaviour
{
    private GolfBallHitHandler golfBall;
    private Vector3 lastClubHeadPos;
    private bool hit = false;

    public void SetGolfBallHitHandler(GolfBallHitHandler golfBallHitHandler) => golfBall = golfBallHitHandler;

    private void FixedUpdate()
    {
        if (hit) golfBall.ForceBall((transform.position - lastClubHeadPos) / Time.fixedDeltaTime);
        lastClubHeadPos = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball")) hit = true;
    }
}
