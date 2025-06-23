using System;
using UnityEngine;
using UnityEngine.Events;

public class PingPongBall : MonoBehaviour
{
    public UnityEvent<Rigidbody> AIreact;

    public UnityEvent scored;

    public UnityEvent reset;

    [SerializeField]
    BoxCollider Table1;
    [SerializeField]
    BoxCollider Net;

    private bool tableHit = false;
    private bool wallHit = false;

    private Rigidbody rb;
    void Start()
    {
        if (!TryGetComponent(out Rigidbody rb))
        {
            Debug.LogError("ball didn't have a rigidbody attached");
        }
        this.rb = rb;
    }

    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger.CompareTag("Table1"))
        {
            if (tableHit == true)
            {
                ResetTriggers();
                reset.Invoke();
                Debug.Log("Table hit twice");
            }
            else
            {
                Debug.Log("Table hit");
                tableHit = true;
            }
        }
        else if (trigger.CompareTag("Wall"))
        {
            if (tableHit && !wallHit)
            {
                wallHit = true;
                Debug.Log("wall hit after table");
                scored.Invoke();
                ResetTriggers();
            }
        }
        else if (trigger.CompareTag("Boundary"))
        {
            Debug.Log("Boundary hit");
            ResetTriggers();
            reset.Invoke();
        } 
        else if (trigger.CompareTag("AITrigger"))
        {
            AIreact?.Invoke(rb);
        }
    }

    private void OnTriggerExit(Collider trigger)
    {
        if(trigger.CompareTag("AITrigger"))
        {
            reset.Invoke();
        }
    }

    private void ResetTriggers()
    {
        tableHit = false; ;
        wallHit = false;
    }
}
