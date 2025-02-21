using UnityEngine;

public class Saber : MonoBehaviour
{
    public LayerMask layer;
    private Vector3 previousPos;

    void Start()
    {
        previousPos = transform.position;
    }

    void Update()
    {
        RaycastHit hit;

        // Cast a ray forward from the saber's position
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1f, layer))
        {
            Debug.DrawRay(transform.position, transform.forward * hit.distance, Color.red); // Red when hitting

            if (Vector3.Angle(transform.position - previousPos, hit.transform.up) > 130)
            {
                Destroy(hit.transform.gameObject);
            }
        }
        else
        {
            Debug.DrawRay(transform.position, transform.forward * 1f, Color.green); // Green when missing
        }

        previousPos = transform.position;
    }
}
