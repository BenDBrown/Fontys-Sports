using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class VisualAudioListener : MonoBehaviour
{
    LineRenderer lineRenderer;
    float[] spectrum = new float[256]; // Store the spectrum data

    [SerializeField]
    private Vector3 fixedPosition;  // Stores a fixed position for the visualizer
        
    [SerializeField]
    private Quaternion fixedRotation;  // Stores a fixed rotation for the visualizer
        
    [SerializeField]
    private Vector3 fixedScale;  // Stores a fixed scale for the visualizer

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();

        // Set line renderer settings
        lineRenderer.startWidth = 0.1f * fixedScale.x; // Adjust width based on scale
        lineRenderer.endWidth = 0.1f * fixedScale.x;
        lineRenderer.positionCount = spectrum.Length;
        lineRenderer.useWorldSpace = true; // Ensure it stays in world space

        // Store the initial world position, rotation, and scale
        fixedPosition = transform.position;
        fixedRotation = transform.rotation;
        fixedScale = transform.localScale;
    }
    
    void Update()
    {
        // Lock position, rotation, and scale
        transform.SetPositionAndRotation(fixedPosition, fixedRotation);
        transform.localScale = fixedScale;

        // Get spectrum data from the global AudioListener
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);

        for (int i = 0; i < spectrum.Length; i++)
        {
            // Keep the line renderer at a fixed position in world space
            Vector3 worldPosition = fixedPosition + (fixedRotation * new Vector3(i * 0.2f * fixedScale.x, spectrum[i] * 10 * fixedScale.y, 0));
            lineRenderer.SetPosition(i, worldPosition);
        }
    }
}
