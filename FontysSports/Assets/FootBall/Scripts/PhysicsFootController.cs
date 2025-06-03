using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class PhysicsFootController : MonoBehaviour
{
    public UnityEvent calibrated;

    [SerializeField] 
    private Rigidbody kinematicFootBody;

    private Vector3 offsetPos = Vector3.zero;

    private Quaternion offsetRot = Quaternion.identity;


    void Start()
    {
        StartCoroutine(CalibrateOnStart());
    }

    private void FixedUpdate()
    {
        kinematicFootBody.MovePosition(transform.position + offsetPos);
        kinematicFootBody.MoveRotation(transform.rotation * offsetRot); // yes multiplication is intentional: quaternion fuckery
    }

    private IEnumerator CalibrateOnStart()
    {
        yield return new WaitForSeconds(1);
        Calibrate();
    }

    private void Calibrate()
    {
        Transform footTransform = kinematicFootBody.transform;
        footTransform.localRotation = Quaternion.identity;
        footTransform.position = new (footTransform.position.x, 0, footTransform.position.z);

        offsetPos = footTransform.position - transform.position;
        offsetRot = Quaternion.Inverse(transform.rotation) * footTransform.rotation; // quaternion fuckery

        calibrated?.Invoke();
    }
}
