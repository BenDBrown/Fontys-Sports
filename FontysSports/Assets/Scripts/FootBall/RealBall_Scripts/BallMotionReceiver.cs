using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using System.Globalization;

public class BallController : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;
    private Thread clientThread;

    private readonly object dataLock = new object();
    private float pitch;
    private float roll;

    private float initialPitch = 0f;
    private float initialRoll = 0f;
    private bool isCalibrated = false;

    [SerializeField] private string ipAddress = "192.168.4.1"; // Arduino AP IP
    [SerializeField] private int port = 80;

    [SerializeField] private GameObject ball;  // Assign the ball in the inspector.
    [SerializeField] private Transform startTransform;  // Empty GameObject used as the reset position
    [SerializeField] private float tiltSensitivity = 0.05f;
    [SerializeField] private float maxSpeed = 2.0f;
    [SerializeField] private float damping = 5f;

    [SerializeField] private float resetDelay = 10f;
    [SerializeField] private float positionThreshold = 0.01f;

    private Vector3 velocity = Vector3.zero;
    private StringBuilder dataBuffer = new StringBuilder();
    private bool isRunning = true;

    private bool isTimerActive = false;
    private float timer = 0f;

    void Start()
    {
        clientThread = new Thread(ConnectToServer);
        clientThread.IsBackground = true;
        clientThread.Start();
    }

    void Update()
    {
        if (ball == null || startTransform == null || !isCalibrated)
            return;

        float currentRoll, currentPitch;

        lock (dataLock)
        {
            currentRoll = roll;
            currentPitch = pitch;
        }

        float relRoll = currentRoll - initialRoll;
        float relPitch = currentPitch - initialPitch;

        float moveX = Mathf.Clamp(relRoll * tiltSensitivity, -maxSpeed, maxSpeed);
        float moveZ = Mathf.Clamp(relPitch * tiltSensitivity, -maxSpeed, maxSpeed);

        Vector3 targetVelocity = new Vector3(moveX, 0, moveZ);
        velocity = Vector3.Lerp(velocity, targetVelocity, Time.deltaTime * damping);

        ball.transform.Translate(velocity * Time.deltaTime, Space.World);

        // Detect movement from start position
        float distanceFromStart = Vector3.Distance(ball.transform.position, startTransform.position);

        if (distanceFromStart > positionThreshold)
        {
            if (!isTimerActive)
            {
                isTimerActive = true;
                timer = 0f;
                Debug.Log("Ball left start position. Starting reset timer.");
            }
        }
        else
        {
            // If returned manually, cancel reset
            isTimerActive = false;
            timer = 0f;
        }

        if (isTimerActive)
        {
            timer += Time.deltaTime;
            if (timer >= resetDelay)
            {
                ResetBall();
            }
        }
    }

    private void ResetBall()
    {
        ball.transform.position = startTransform.position;
        velocity = Vector3.zero;
        isCalibrated = false; // Recalibrate on next data
        isTimerActive = false;
        timer = 0f;

        Debug.Log("Ball reset to startTransform position. Awaiting recalibration.");
    }

    private void ConnectToServer()
    {
        try
        {
            client = new TcpClient(ipAddress, port);
            stream = client.GetStream();
            Debug.Log("Connected to Arduino");

            byte[] buffer = new byte[1024];

            while (isRunning)
            {
                if (!stream.DataAvailable)
                {
                    Thread.Sleep(10);
                    continue;
                }

                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0)
                {
                    Debug.LogWarning("Server closed connection.");
                    break;
                }

                string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Debug.Log($"Received raw data: [{data.Replace("\n", "\\n")}]");

                dataBuffer.Append(data);

                while (true)
                {
                    string currentBuffer = dataBuffer.ToString();
                    int newlineIndex = currentBuffer.IndexOf('\n');
                    if (newlineIndex < 0)
                        break;

                    string line = currentBuffer.Substring(0, newlineIndex).Trim();
                    dataBuffer.Remove(0, newlineIndex + 1);
                    ParseData(line);
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("TCP Error: " + e.Message);
        }
        finally
        {
            stream?.Close();
            client?.Close();
        }
    }

    private void ParseData(string line)
    {
        if (string.IsNullOrWhiteSpace(line))
            return;

        Debug.Log($"Parsing line: '{line}'");

        string[] parts = line.Split(',');
        if (parts.Length != 6)
        {
            Debug.LogWarning($"Incorrect data length: expected 6, got {parts.Length}");
            return;
        }

        var culture = CultureInfo.InvariantCulture;

        if (float.TryParse(parts[3], NumberStyles.Float, culture, out float parsedRoll) &&
            float.TryParse(parts[4], NumberStyles.Float, culture, out float parsedPitch))
        {
            lock (dataLock)
            {
                if (!isCalibrated)
                {
                    initialRoll = parsedRoll;
                    initialPitch = parsedPitch;
                    isCalibrated = true;
                    Debug.Log($"Calibrated: initialRoll={initialRoll}, initialPitch={initialPitch}");
                }

                roll = parsedRoll;
                pitch = parsedPitch;
            }

            Debug.Log($"Received tilt → Roll: {parsedRoll}, Pitch: {parsedPitch}");
        }
        else
        {
            Debug.LogWarning("Failed to parse roll/pitch as floats");
        }
    }

    private void OnApplicationQuit()
    {
        isRunning = false;
        clientThread?.Join(500);
        client?.Close();
    }
}
