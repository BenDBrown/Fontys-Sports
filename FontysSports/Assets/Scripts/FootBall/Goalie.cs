using UnityEngine;

public class Goalie : MonoBehaviour
{
    [SerializeField]
    private float trackingSpeed = 10;

    [SerializeField]
    private float diveSpeed = 20;

    [SerializeField]
    private float[] randomXPositions = {0};

    [SerializeField]
    private float ballTrackingSpeedThreshold = 2000; // above this speed the goalie will pick a random position to block instead of tracking the ball

    private Vector3 destination;

    private Rigidbody rb;

    private Rigidbody ballRb = null;

    private float speed;

    private bool diving = false;

    private void Start()
    {
        destination = transform.position;
        if(!TryGetComponent(out Rigidbody rb))
        {
            Debug.LogError($"rigidbody was nod added to goalie: {gameObject.name}");
            enabled = false;
            return;
        }
        this.rb = rb;

        if(randomXPositions.Length <= 0)
        {
            Debug.LogError("randomXPositions must have a length greater than or equal to 1");
            enabled = false;
        }
    }

    private void Update()
    {
        if(ballRb == null) return;
        if(ballRb.linearVelocity.magnitude <= ballTrackingSpeedThreshold)
        {
            destination = new(ballRb.position.x, transform.position.y, transform.position.z);
            speed = trackingSpeed;
            diving = false;
        }
        else if(!diving)
        {
            float randomX = randomXPositions[Random.Range(0,randomXPositions.Length)];
            destination = new(randomX, transform.position.y, transform.position.z);
            speed = diveSpeed;
            diving = true;
        }
        if((destination - transform.position).magnitude < speed * Time.deltaTime) return;
        rb.MovePosition(transform.position + ((destination - transform.position).normalized * speed * Time.deltaTime));
    }

    public void OnBallShot(Rigidbody ballRb) => this.ballRb = ballRb;

    public void OnShotEnded() 
    {
        diving = false;
        ballRb = null;
        rb.MovePosition(new(0, transform.position.y, transform.position.z));
    }

    
}
