using System;
using UnityEngine;
using UnityEngine.Events;

public class PingPongBall : MonoBehaviour
{
    public UnityEvent<Rigidbody> AIreact;

    public UnityEvent<bool> scored;

    public UnityEvent reset;

    public UnityEvent AIReset;

    private float aiReturnSpeed = 8f;

    private bool playerpoint;
    private bool isServe = true;
    private bool table1Hit = false;
    private bool table2Hit = false;

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
            if (isServe && !table1Hit)
            {
                table1Hit = true;
            }
            else if (table1Hit && isServe)
            {
                ResetTriggers();
                reset.Invoke();
            }
            else if (playerpoint && !table1Hit)
            {
                playerpoint = false;
                table1Hit = true;
                table2Hit = false;
            }
            else if (!playerpoint && table1Hit)
            {
                scored.Invoke(playerpoint);
                ResetTriggers();
                reset.Invoke();
            }
        }
        else if (trigger.CompareTag("Table2"))
        {
            if (table1Hit && isServe)
            {
                playerpoint = true;
                table2Hit = true;
                table1Hit = false;
                isServe = false;
            }
            else if (!table1Hit && isServe)
            {
                ResetTriggers();
                reset.Invoke();
            }
            else if (playerpoint && table2Hit)
            {
                scored.Invoke(playerpoint);
                ResetTriggers();
                reset.Invoke();
            }
            else if (!playerpoint && !table2Hit)
            {
                playerpoint = true;
                table2Hit = true;
            }
        }
        else if (trigger.CompareTag("Boundary"))
        {
            if (!isServe)
            {
                scored.Invoke(playerpoint);
                ResetTriggers();
                reset.Invoke();
            }
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
            AIReset.Invoke();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("AIPaddle"))
        {
            // Get contact point
            ContactPoint contact = collision.contacts[0];

            // Get the forward direction of the AI paddle (assumes it is hitting toward the player)
            Vector3 paddleForward = collision.transform.forward;

            // Slightly adjust direction based on where it hit the paddle
            Vector3 direction = (paddleForward + contact.normal).normalized;

            // Set ball velocity
            rb.AddForce(direction * aiReturnSpeed);
        }
    }

    private void ResetTriggers()
    {
        table1Hit = false; ;
        table2Hit = false;
        isServe = true;
    }
}
