using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("Networking")]
    public string ipAddress = "192.168.4.1";
    public int port = 80;

    [Header("Game Objects")]
    public GameObject ball;
    public Transform respawnPoint;

    [Header("Goal Box (Editable in Inspector)")]
    public Vector3 boxCenter = new Vector3(0, 0, 10);
    public Vector3 boxSize = new Vector3(4, 4, 4); // 3D box

    [Header("Movement Settings")]
    public float tiltSensitivity = 0.05f;
    public float maxSpeed = 2f;
    public float damping = 5f;
    public float moveThreshold = 0.1f;
    public float respawnTime = 10f;
    public float ballRadius = 0.5f;

    [Header("Hop Settings")]
    public float hopForce = 2.5f;
    public float hopDuration = 0.3f;

    [Header("Bounce Settings")]
    public float bounceImpulse = 2.0f;
    public LayerMask groundLayer;

    private TcpClient client;
    private NetworkStream stream;
    private Thread clientThread;

    private float roll = 0f;
    private float pitch = 0f;

    private float initialRoll = 0f;
    private float initialPitch = 0f;
    private bool isCalibrated = false;

    private Vector3 velocity = Vector3.zero;
    private bool isMoving = false;
    private float moveTimer = 0f;

    private bool targetLocked = false;
    private Vector3 targetPosition;

    private Rigidbody ballRb;
    private bool hasHopped = false;
    private float hopTimer = 0f;
    private bool wasGrounded = true;

    void Start()
    {
        if (ball != null)
        {
            ballRb = ball.GetComponent<Rigidbody>();
            if (ballRb == null)
            {
                Debug.LogError("⚠️ Ball GameObject must have a Rigidbody component!");
            }
            ballRb.useGravity = true;
            ballRb.linearVelocity = Vector3.zero;
        }
        else
        {
            Debug.LogError("⚠️ Ball GameObject not assigned!");
        }

        ResetBall();

        clientThread = new Thread(ConnectToServer);
        clientThread.IsBackground = true;
        clientThread.Start();
    }

    void Update()
    {
        if (ball == null || respawnPoint == null) return;
        if (!isCalibrated) return;

        if (!targetLocked && (Mathf.Abs(roll - initialRoll) > moveThreshold || Mathf.Abs(pitch - initialPitch) > moveThreshold))
        {
            LockTarget3D();
            hasHopped = false;
            hopTimer = 0f;
        }

        if (!targetLocked) return;

        if (IsBallInsideGoalBox())
        {
            ballRb.linearVelocity = Vector3.zero;
            isMoving = false;
            moveTimer = 0f;
            targetLocked = false;
            Debug.Log("🎉 Goal reached! Respawning ball...");
            ResetBall();
            return;
        }

        float relRoll = roll - initialRoll;
        float relPitch = pitch - initialPitch;
        float inputAmount = Mathf.Clamp01(new Vector2(relRoll, relPitch).magnitude * tiltSensitivity);

        if (inputAmount > moveThreshold)
        {
            isMoving = true;

            Vector3 toTarget = targetPosition - ball.transform.position;
            Vector3 moveDirection = toTarget.normalized;
            Vector3 targetVelocity = moveDirection * inputAmount * maxSpeed;

            if (!hasHopped)
            {
                Vector3 hopVelocity = new Vector3(
                    targetVelocity.x,
                    hopForce,
                    targetVelocity.z);

                ballRb.linearVelocity = hopVelocity;
                hasHopped = true;
                hopTimer = 0f;
            }
            else
            {
                hopTimer += Time.deltaTime;

                if (hopTimer > hopDuration)
                {
                    Vector3 currentVelocity = ballRb.linearVelocity;
                    Vector3 horizontalVelocity = Vector3.Lerp(
                        new Vector3(currentVelocity.x, 0, currentVelocity.z),
                        new Vector3(targetVelocity.x, 0, targetVelocity.z),
                        Time.deltaTime * damping);

                    ballRb.linearVelocity = new Vector3(horizontalVelocity.x, currentVelocity.y, horizontalVelocity.z);
                }
            }

            ApplyRollingRotation(new Vector3(ballRb.linearVelocity.x, 0, ballRb.linearVelocity.z));
        }
        else
        {
            Vector3 currentVelocity = ballRb.linearVelocity;
            Vector3 horizontalVelocity = Vector3.Lerp(
                new Vector3(currentVelocity.x, 0, currentVelocity.z),
                Vector3.zero,
                Time.deltaTime * damping);

            ballRb.linearVelocity = new Vector3(horizontalVelocity.x, currentVelocity.y, horizontalVelocity.z);

            isMoving = false;
            moveTimer = 0f;
        }

        HandleBounceDetection();
    }

    private bool IsBallInsideGoalBox()
    {
        Vector3 ballPos = ball.transform.position;
        Vector3 halfSize = boxSize * 0.5f;

        bool insideX = ballPos.x >= (boxCenter.x - halfSize.x) && ballPos.x <= (boxCenter.x + halfSize.x);
        bool insideY = ballPos.y >= (boxCenter.y - halfSize.y) && ballPos.y <= (boxCenter.y + halfSize.y);
        bool insideZ = ballPos.z >= (boxCenter.z - halfSize.z) && ballPos.z <= (boxCenter.z + halfSize.z);

        return insideX && insideY && insideZ;
    }

    private void HandleBounceDetection()
    {
        if (ball == null || ballRb == null) return;

        float rayDistance = ballRadius + 0.1f;
        bool grounded = Physics.Raycast(ball.transform.position, Vector3.down, rayDistance, groundLayer);

        if (!wasGrounded && grounded)
        {
            if (ballRb.linearVelocity.y <= 0f)
            {
                ballRb.AddForce(Vector3.up * bounceImpulse, ForceMode.Impulse);
                Debug.Log("🏀 Bounce applied!");
            }
        }

        wasGrounded = grounded;
    }

    private void ApplyRollingRotation(Vector3 horizontalVelocity)
    {
        if (horizontalVelocity.magnitude < 0.001f) return;

        Vector3 rollAxis = Vector3.Cross(Vector3.up, horizontalVelocity.normalized);
        float distance = horizontalVelocity.magnitude * Time.deltaTime;
        float rotationDegrees = (distance / (2 * Mathf.PI * ballRadius)) * 360f;

        ball.transform.Rotate(rollAxis, rotationDegrees, Space.World);
    }

    private void LockTarget3D()
    {
        Vector3 direction = new Vector3(roll - initialRoll, pitch - initialPitch, pitch - initialPitch).normalized;
        Vector3 halfSize = boxSize * 0.5f;

        Vector3 targetLocal = new Vector3(
            Mathf.Clamp(direction.x * halfSize.x, -halfSize.x, halfSize.x),
            Mathf.Clamp(direction.y * halfSize.y, -halfSize.y, halfSize.y),
            Mathf.Clamp(direction.z * halfSize.z, -halfSize.z, halfSize.z)
        );

        targetPosition = boxCenter + targetLocal;
        targetLocked = true;
        Debug.Log($"🎯 Target locked: {targetPosition}");
    }

    private void ConnectToServer()
    {
        try
        {
            client = new TcpClient(ipAddress, port);
            stream = client.GetStream();
            Debug.Log("✅ Connected to Arduino");

            byte[] buffer = new byte[1024];
            while (true)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                ParseData(data);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("❌ TCP Error: " + e.Message);
        }
    }

    private void ParseData(string data)
    {
        foreach (string line in data.Split('\n'))
        {
            string[] parts = line.Trim().Split(',');
            if (parts.Length != 2) continue;

            if (float.TryParse(parts[0], out float parsedRoll) &&
                float.TryParse(parts[1], out float parsedPitch))
            {
                if (!isCalibrated)
                {
                    initialRoll = parsedRoll;
                    initialPitch = parsedPitch;
                    isCalibrated = true;
                    Debug.Log($"🎯 Calibrated: initialRoll={initialRoll}, initialPitch={initialPitch}");
                }

                roll = parsedRoll;
                pitch = parsedPitch;
            }
        }
    }

    private void ResetBall()
    {
        Debug.Log("🔄 Ball reset and recalibrating...");
        isCalibrated = false;
        targetLocked = false;
        velocity = Vector3.zero;
        isMoving = false;
        moveTimer = 0f;
        hasHopped = false;
        hopTimer = 0f;

        if (ball != null && respawnPoint != null && ballRb != null)
        {
            ballRb.linearVelocity = Vector3.zero;
            ball.transform.position = respawnPoint.position;
            ball.transform.rotation = Quaternion.identity;
        }
    }

    private void OnApplicationQuit()
    {
        client?.Close();
        clientThread?.Abort();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}
