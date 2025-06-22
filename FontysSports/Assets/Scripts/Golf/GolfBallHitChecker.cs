using UnityEngine;
using UnityEngine.Events;

public class GolfBallHitChecker : MonoBehaviour
{
    public UnityEvent Hit = new();

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag != "Ball") return;
        Hit?.Invoke();
        if (collision.collider.TryGetComponent(out Rigidbody rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        else Debug.LogWarning($"Could not find rigidbody on Golfball: {collision.gameObject.name}");
    }
}
