using UnityEngine;

public class WindMillBlades : MonoBehaviour
{
    [SerializeField] private Transform bladesMesh;
    [SerializeField] private Transform bladesCollider;
    [SerializeField] private float spinDistance = 1;
    [SerializeField] private float spinSpeed = 1;
    private Vector3 velocity = Vector3.zero;

    private void Update()
    {
        Vector3 currentRot = new(0, 0, bladesMesh.eulerAngles.z);
        Vector3 nextRot = new(0, 0, bladesMesh.eulerAngles.z + spinDistance);
        bladesMesh.eulerAngles = Vector3.SmoothDamp(currentRot, nextRot, ref velocity, spinSpeed);
        bladesCollider.eulerAngles = Vector3.SmoothDamp(currentRot, nextRot, ref velocity, spinSpeed);
    }
}