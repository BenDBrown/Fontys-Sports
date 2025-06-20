using UnityEngine;

public class SE_ColliderHandler : MonoBehaviour
{
    [SerializeField] private Collider colliderZPositive;
    [SerializeField] private Collider colliderZNegative;

    public void Disable()
    {
        colliderZPositive.enabled = false;
        colliderZNegative.enabled = false;
    }

    public void SwitchCollider(bool zPositiveTriggered)
    {
        colliderZPositive.enabled = !zPositiveTriggered;
        colliderZNegative.enabled = zPositiveTriggered;
    }
}