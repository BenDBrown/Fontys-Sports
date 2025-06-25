using UnityEngine;

public class GolfBallPosInvalidTrigger : MonoBehaviour
{
    [SerializeField] private GolfBall golfBall;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball")) golfBall.PoseInvalid = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Ball")) golfBall.PoseInvalid = false;
    }
}