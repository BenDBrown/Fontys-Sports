using UnityEngine;

public class Calibrator : MonoBehaviour
{
    [SerializeField]
    private GameObject rightFoot;

    [SerializeField]
    private GameObject leftFoot;

    private void Start()
    {
        Calibrate();
    }

    public void Calibrate()
    {
        CalibrateFoot(rightFoot);
        CalibrateFoot(leftFoot);
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
