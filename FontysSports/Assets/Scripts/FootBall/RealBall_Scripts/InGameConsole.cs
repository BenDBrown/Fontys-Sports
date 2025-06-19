using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class InGameConsole : MonoBehaviour

{
    [SerializeField] private TextMeshProUGUI logText; // Assign the LogText UI element
    [SerializeField] private ScrollRect scrollRect;

    private readonly List<string> logMessages = new List<string>();
    private const int maxMessages = 100;

    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string color = type switch
        {
            LogType.Error => "red",
            LogType.Warning => "yellow",
            LogType.Exception => "magenta",
            _ => "white"
        };

        string formatted = $"<color={color}>{logString}</color>";
        logMessages.Add(formatted);

        if (logMessages.Count > maxMessages)
            logMessages.RemoveAt(0);

        logText.text = string.Join("\n", logMessages);
        Canvas.ForceUpdateCanvases(); // Make sure scroll updates
        scrollRect.verticalNormalizedPosition = 0f; // Scroll to bottom
    }

    public void ClearLogs()
    {
        logMessages.Clear();
        logText.text = "";
    }
}