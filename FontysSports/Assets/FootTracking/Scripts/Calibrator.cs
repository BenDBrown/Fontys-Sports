using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class Calibrator : MonoBehaviour
{
    public UnityEvent calibrated;

    [SerializeField]
    private GameObject rightFoot;

    [SerializeField]
    private GameObject leftFoot;

    private void Start()
    {
        StartCoroutine(Calibrate());
    }

    public IEnumerator Calibrate()
    {
        yield return new WaitForSeconds(1);
        CalibrateFoot(rightFoot);
        CalibrateFoot(leftFoot);
        calibrated.Invoke();
    }

    private void CalibrateFoot(GameObject foot)
    {
        if(!foot.TryGetComponent(out BoxCollider col))
        {
            Debug.LogError($"{foot.name} does not have a box collider, as is required");
            return;
        }
        Transform transform = foot.transform;
        transform.localRotation = Quaternion.identity;
        transform.position = new (transform.position.x, 0 + (col.size.y/2), transform.position.z);
    }
}
