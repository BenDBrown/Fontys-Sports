using UnityEngine;
using UnityEngine.Events;

public class Football : MonoBehaviour
{
    public UnityEvent<Rigidbody> shot;

    public UnityEvent shotEnded;

    public UnityEvent<float> scored;

    [SerializeField]
    BoxCollider goalCol;

    public bool IsBeingShot { get; private set; } = false;

    private Rigidbody rb;

    private void Start()
    {
        if (!TryGetComponent(out Rigidbody rb))
        {
            Debug.LogError("football did not have a rigidbody attached");
        }
        this.rb = rb;
    }

    void Update()
    {
        if((!IsBeingShot) || IsMovingTowardsGoal()) return;
        IsBeingShot = false;
        shotEnded?.Invoke();
    }

    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger.tag != "Goal" || (!IsBeingShot)) return;
        IsBeingShot = false;
        scored?.Invoke(rb.linearVelocity.magnitude);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag != "Foot") return;
        IsBeingShot = true;
        shot?.Invoke(rb);
    }

    private bool IsMovingTowardsGoal()
    {
        if(rb.linearVelocity.magnitude == 0) return false;
        Vector3 ballToRightPost = new Vector3(goalCol.transform.position.x + goalCol.size.x/2, goalCol.transform.position.y, goalCol.transform.position.z) - transform.position;
        Vector3 ballToLeftPost = new Vector3(goalCol.transform.position.x - goalCol.size.x/2, goalCol.transform.position.y, goalCol.transform.position.z) - transform.position;
        return Vector3.Dot(ballToRightPost, rb.linearVelocity) > 0 || Vector3.Dot(ballToLeftPost, rb.linearVelocity) > 0 ;
    }


}
