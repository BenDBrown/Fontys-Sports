using System.Collections;
using UnityEngine;

public class DisableColliders : MonoBehaviour
{
    [SerializeField] private Collider[] colliders;

    public void Disable()
    {
        foreach (Collider col in colliders) col.enabled = false;
    }

    public IEnumerator Enable()
    {
        yield return new WaitForSeconds(0.1f);
        foreach (Collider col in colliders) col.enabled = true;
    }
}
