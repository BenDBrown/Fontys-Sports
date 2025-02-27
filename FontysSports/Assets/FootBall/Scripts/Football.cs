using UnityEngine;
using UnityEngine.Events;

public class Football : MonoBehaviour
{
    public UnityEvent shot;

    public UnityEvent shotEnded;

    public UnityEvent scored;

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

    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger.tag != "Goal") return;
        IsBeingShot = false;
        scored?.Invoke();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.tag != "Foot") return;
        IsBeingShot = true;
        shot?.Invoke();
    }

    void Update()
    {
        if((!IsBeingShot) || rb.linearVelocity.magnitude >= 0) return;
        IsBeingShot = false;
        shotEnded?.Invoke();
    }
}
