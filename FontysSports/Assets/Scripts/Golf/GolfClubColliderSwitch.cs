using System.Collections;
using UnityEngine;

public class GolfClubColliderSwitch : MonoBehaviour
{
    [SerializeField] private Transform clubHead;
    [SerializeField] private float speedThreshold = 0.01f;
    [Space(10)]
    [SerializeField] private Collider bigCollider;
    [SerializeField] private GameObject testCube;
    [SerializeField] private float lifeSpan = 0.1f;

    private Vector3 clubHeadPreviousPos = Vector3.zero;
    private bool swapped = false;

    private void FixedUpdate()
    {
        Vector3 clubHeadPos = clubHead.position;
        float distance = Vector3.Distance(clubHeadPreviousPos, clubHeadPos);
        bool swapping = false;
        if (distance >= speedThreshold) swapping = true;
        clubHeadPreviousPos = clubHeadPos;

        if (!swapped && swapping) StartCoroutine(Swap());
    }

    private IEnumerator Swap()
    {
        swapped = true;
        bigCollider.enabled = true;
        testCube.SetActive(true);
        yield return new WaitForSeconds(lifeSpan);
        bigCollider.enabled = false;
        testCube.SetActive(false);
        swapped = false;
    }
}