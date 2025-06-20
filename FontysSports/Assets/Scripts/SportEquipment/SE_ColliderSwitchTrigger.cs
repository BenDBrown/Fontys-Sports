using UnityEngine;

public class SE_ColliderSwitchTrigger : MonoBehaviour
{
    [SerializeField] private bool isZPositive;
    [SerializeField] private SE_ColliderHandler colliderHandler;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball")) colliderHandler.SwitchCollider(isZPositive);
    }
}
