using UnityEngine;

public class PingPongAI : MonoBehaviour
{
    [SerializeField]
    private float trackingSpeed = 20;

    [SerializeField]
    private float ballTrackingSpeedThreshold = 2000; // above this speed the goalie will pick a random position to block instead of tracking the ball

    [SerializeField] 
    private Transform lookAtPoint;

    private Vector3 destination;

    private Vector3 currentVelocity;

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

        // Update tracking destination based on ball speed
        if (ballRb.linearVelocity.magnitude <= ballTrackingSpeedThreshold)
        {
            destination = new Vector3(ballRb.position.x, ballRb.position.y, transform.position.z);
            speed = trackingSpeed;
        }

        // Move towards destination if not already there
        if ((destination - transform.position).magnitude >= 0.01f)
        {
            Vector3 newPosition = Vector3.SmoothDamp(transform.position, destination, ref currentVelocity, 0.1f, speed);
            rb.MovePosition(newPosition);
        }

        // Always look at the defined point
        if (lookAtPoint != null)
        {
            transform.LookAt(lookAtPoint.position);
        }
    }

    public void OnBallShot(Rigidbody ballRb) => this.ballRb = ballRb;

    public void onReset()
    {
        ballRb = null;
        rb.MovePosition(new(transform.position.x, transform.position.y, transform.position.z));
    }

}
