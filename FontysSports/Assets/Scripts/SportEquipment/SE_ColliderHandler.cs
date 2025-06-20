using UnityEngine;

public class SE_ColliderHandler : MonoBehaviour
{
    [SerializeField] private BoxCollider colliderZPositive;
    [SerializeField] private BoxCollider colliderZNegative;

    public void EnableCollider(bool zPositiveTriggered)
    {
        colliderZPositive.enabled = !zPositiveTriggered;
        colliderZNegative.enabled = zPositiveTriggered;
    }

    public void DisableColliders()
    {
        colliderZPositive.enabled = false;
        colliderZNegative.enabled = false;
    }
}