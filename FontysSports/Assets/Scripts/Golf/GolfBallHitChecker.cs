using UnityEngine;
using UnityEngine.Events;

public class GolfBallHitChecker : MonoBehaviour
{
    public UnityEvent<Rigidbody> Hit = new();

    [SerializeField]
    private bool freezeBallOnHit = true;

    private void OnCollisionEnter(Collision collision) => CheckForGolfBallCollision(collision.collider);

    private void OnTriggerEnter(Collider collision) => CheckForGolfBallCollision(collision);

    private void CheckForGolfBallCollision(Collider collision)
    {
        if (collision.tag != "Ball") return;
        if (!collision.TryGetComponent(out Rigidbody rb))
        {
            Debug.LogWarning($"Could not find rigidbody on Golfball: {collision.gameObject.name}");
            return;
        };
        Hit?.Invoke(rb);
        if (!freezeBallOnHit) return;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }
}
