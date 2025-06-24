using UnityEngine;

public class GolfBallHidden : MonoBehaviour
{
    [SerializeField] private GolfBallReset golfBall;
    [SerializeField] private Transform respawnTransform;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball")) golfBall.IsHidden(true, respawnTransform.position);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ball")) golfBall.IsHidden(false, respawnTransform.position);
    }
}
