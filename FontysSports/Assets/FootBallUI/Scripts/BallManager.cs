using UnityEngine;

public class BallManager : MonoBehaviour
{
    [SerializeField] private Vector3 resetCoord;
    [SerializeField] private Collider goal;
    [SerializeField] private ScoreboardManager scoreboardManager;

    private Rigidbody rb;

    private void Start()
    {
        rb = gameObject.GetComponent<Rigidbody>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other == null) return;

        if (goal.name == other.name) scoreboardManager.Goal(Random.Range(0.1f,30));
        else scoreboardManager.Miss();

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        transform.position = resetCoord;
    }
}
