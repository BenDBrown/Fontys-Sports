using UnityEngine;

public class GolfClubColliderSwitch : MonoBehaviour
{
    [SerializeField] private float speedThreshold = 3;
    [SerializeField] private Transform clubHead;
    [SerializeField] private Collider bigCollider;

    private Vector3 clubHeadPreviousPos = Vector3.zero;
    private bool swapping = false;
    private bool overSpeedThreshold = false;

    private void FixedUpdate()
    {
        Vector3 clubHeadPos = clubHead.position;
        float distance = Vector3.Distance(clubHeadPreviousPos, clubHeadPos);
        if (overSpeedThreshold != distance >= speedThreshold) swapping = true;
        overSpeedThreshold = distance >= speedThreshold;
        clubHeadPreviousPos = clubHeadPos;

        if (swapping)
        {
            bigCollider.enabled = overSpeedThreshold;
            swapping = false;
        }
    }
}