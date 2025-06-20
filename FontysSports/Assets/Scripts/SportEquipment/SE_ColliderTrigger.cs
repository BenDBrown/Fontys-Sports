using UnityEngine;

public class SE_ColliderTrigger : MonoBehaviour
{
    [SerializeField] private bool isZPositive;
    [SerializeField] private SE_ColliderHandler colliderHandler;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ball")) colliderHandler.EnableCollider(isZPositive);
    }
}
