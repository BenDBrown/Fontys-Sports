using UnityEngine;

public class Despawner : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Cube"))
        {
            Destroy(other.gameObject);
        }
    }
}
