using System.Collections;
using UnityEngine;

public class DisableColliders : MonoBehaviour
{
    [SerializeField] private Collider col;

    public void Disable()
    {
        col.enabled = false;
    }

    public IEnumerator Enable()
    {
        yield return new WaitForSeconds(0.1f);
        col.enabled = true;
    }
}
