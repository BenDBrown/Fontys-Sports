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

    public string ipAddress = "192.168.4.1"; // Arduino AP IP
    public int port = 80;

    public GameObject ball;  // Assign your ball in Inspector
    public float tiltSensitivity = 0.05f;
    public float maxSpeed = 2.0f;
    public float damping = 5f;

    private Vector3 velocity = Vector3.zero;
    private StringBuilder dataBuffer = new StringBuilder();

    private bool isRunning = true;

    void Start()
    {
        clientThread = new Thread(ConnectToServer);
        clientThread.IsBackground = true;
        clientThread.Start();
    }

    void Update()
    {
        if (ball == null || !isCalibrated)
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
    }

    private void ConnectToServer()
    {
        try
        {
            client = new TcpClient(ipAddress, port);
            stream = client.GetStream();
            Debug.Log("✅ Connected to Arduino");

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
                    Debug.LogWarning("⚠️ Server closed connection.");
                    break;
                }

                string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Debug.Log($"Received raw data: [{data.Replace("\n", "\\n")}]"); // Log raw data with \n visible

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
            Debug.LogError("❌ TCP Error: " + e.Message);
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
            Debug.LogWarning($"⚠️ Incorrect data length: expected 6, got {parts.Length}");
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
                    Debug.Log($"🎯 Calibrated: initialRoll={initialRoll}, initialPitch={initialPitch}");
                }

                roll = parsedRoll;
                pitch = parsedPitch;
            }

            Debug.Log($"📡 Received tilt → Roll: {parsedRoll}, Pitch: {parsedPitch}");
        }
        else
        {
            Debug.LogWarning("⚠️ Failed to parse roll/pitch as floats");
        }
    }

    private void OnApplicationQuit()
    {
        isRunning = false;
        clientThread?.Join(500);
        client?.Close();
    }
}
