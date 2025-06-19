using System;
using UnityEngine;
using UnityEngine.Events;

public class PingPongBall : MonoBehaviour
{
    public UnityEvent scored;

    public UnityEvent reset;

    [SerializeField]
    BoxCollider Table1;
    [SerializeField]
    BoxCollider Net;

    private bool tableHit = false;
    private bool wallHit = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
            } else
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
        } else if (trigger.CompareTag("Boundary"))
        {
            Debug.Log("Boundary hit");
            ResetTriggers();
            reset.Invoke();
        }
    }

    private void ResetTriggers()
    {
        tableHit = false; ;
        wallHit = false;
    }
}
