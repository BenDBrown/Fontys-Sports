using UnityEngine;

public class AiTargetCheckpoint : MonoBehaviour
{
    public delegate void GolfBallCollisionEventHandler();

    public GolfBallCollisionEventHandler GolfBallEntered;

    // the position the AI will aim for when in this checkpoint
    [SerializeField]
    private Transform target;

    public Vector3 Target => target.position;

    private void OnTriggerEnter(Collider col)
    {
        if (col.tag != "Ball") return;
        GolfBallEntered?.Invoke();
    }

}
