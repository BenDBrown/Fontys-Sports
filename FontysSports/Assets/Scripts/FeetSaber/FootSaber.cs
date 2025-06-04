using UnityEngine;

public class FootSaber : MonoBehaviour
{
    public LayerMask layer; // Define which layers the raycast should hit

    void Update()
    {
        RaycastHit hit;

        // Cast a ray downward from the saber's position
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f, layer))
        {
            Debug.DrawRay(transform.position, Vector3.down * hit.distance, Color.red);

            // Destroy the object hit by the raycast
            Destroy(hit.transform.gameObject);
        }
        else
        {
            Debug.DrawRay(transform.position, Vector3.down * 1f, Color.green);
        }
    }
}

