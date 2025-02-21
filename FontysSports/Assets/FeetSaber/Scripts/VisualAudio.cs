using UnityEngine;

[RequireComponent(typeof(AudioListener))]
[RequireComponent(typeof(LineRenderer))]
public class VisualAudioListener : MonoBehaviour
{
    LineRenderer lineRenderer;
    float[] spectrum = new float[256]; // Store the spectrum data

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // Set line renderer settings
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = spectrum.Length;
        lineRenderer.useWorldSpace = false;
    }
    
    void Update()
    {
        // Get spectrum data from the global AudioListener
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

        for (int i = 0; i < spectrum.Length; i++)
        {
            // Directly map spectrum data to line positions
            lineRenderer.SetPosition(i, new Vector3(i, spectrum[i] * 10, 0));
        }
    }
}
