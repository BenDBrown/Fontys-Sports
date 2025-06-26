using System;
using UnityEngine;
using UnityEngine.Events;

public class PingPongBall : MonoBehaviour
{
    [Header("Events")]
    public UnityEvent<Rigidbody> AIreact;
    public UnityEvent<bool> scored;
    public UnityEvent reset;
    public UnityEvent AIReset;

    [Header("Settings")]
    public float aiReturnSpeed = 8f;

    private bool playerpoint;
    private bool isServe = true;
    private bool table1Hit = false;
    private bool table2Hit = false;

    private Rigidbody rb;
    private Vector3 startPosition;

    void Start()
    {
        if (!TryGetComponent(out rb))
        {
            Debug.LogError("Ball didn't have a Rigidbody attached.");
        }

        startPosition = transform.position; // Save original spawn point
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
                ResetRound(false);
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
                ResetRound(true);
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
                ResetRound(false);
            }
            else if (playerpoint && table2Hit)
            {
                scored.Invoke(playerpoint);
                ResetRound(true);
            }
            else if (!playerpoint && !table2Hit)
            {
                playerpoint = true;
                table2Hit = true;
            }
        }
        else if (trigger.CompareTag("Boundary"))
        {
            Debug.Log("Boundary hit");

            if (!isServe)
            {
                scored.Invoke(playerpoint);
            }

            ResetRound(true);
        }
        else if (trigger.CompareTag("AITrigger"))
        {
            AIreact?.Invoke(rb);
        }
    }

    private void OnTriggerExit(Collider trigger)
    {
        if (trigger.CompareTag("AITrigger"))
        {
            AIReset?.Invoke();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("AIPaddle"))
        {
            ContactPoint contact = collision.contacts[0];
            Vector3 paddleForward = collision.transform.forward;
            Vector3 direction = (paddleForward + contact.normal).normalized;

            rb.linearVelocity = Vector3.zero;
            rb.AddForce(direction * aiReturnSpeed, ForceMode.VelocityChange);
        }
    }

    private void ResetRound(bool doRespawn)
    {
        ResetTriggers();
        reset.Invoke();

        if (doRespawn)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            transform.position = startPosition;
        }
    }

    private void ResetTriggers()
    {
        table1Hit = false;
        table2Hit = false;
        isServe = true;
    }
}
