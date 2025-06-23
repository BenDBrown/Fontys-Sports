using UnityEngine;

public class PingPongAI : MonoBehaviour
{
    [SerializeField]
    private float trackingSpeed = 20;

    [SerializeField]
    private float ballTrackingSpeedThreshold = 2000; // above this speed the goalie will pick a random position to block instead of tracking the ball

    private Vector3 destination;

    private Rigidbody rb;

    private Rigidbody ballRb = null;

    private float speed;

    private void Start()
    {
        destination = transform.position;
        if (!TryGetComponent(out Rigidbody rb))
        {
            Debug.LogError($"rigidbody was nod added to goalie: {gameObject.name}");
            enabled = false;
            return;
        }
        this.rb = rb;

    }
    private void Update()
    {
        if (ballRb == null) return;
        if (ballRb.linearVelocity.magnitude <= ballTrackingSpeedThreshold)
        {
            destination = new(ballRb.position.x, transform.position.y, transform.position.z);
            speed = trackingSpeed;
        }
        if ((destination - transform.position).magnitude < speed * Time.deltaTime) return;
        rb.MovePosition(transform.position + ((destination - transform.position).normalized * speed * Time.deltaTime));
    }

    public void OnBallShot(Rigidbody ballRb) => this.ballRb = ballRb;

    public void onReset()
    {
        ballRb = null;
        rb.MovePosition(new(transform.position.x, transform.position.y, transform.position.z));
    }

}
