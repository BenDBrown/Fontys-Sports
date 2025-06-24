using System.Collections;
using UnityEngine;

public class GolfBallReset : MonoBehaviour
{
    private Rigidbody rigid;
    private bool isHidden = false;
    private Vector3 currentRespawnPos;

    private void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!isHidden || rigid.linearVelocity.magnitude >= 0.1f) return;
        StartCoroutine(BallReset(currentRespawnPos));
    }

    public void IsHidden(bool status, Vector3 respawnPos)
    {
        isHidden = status;
        currentRespawnPos = respawnPos;
    }

    public IEnumerator BallReset(Vector3 newRespawnPos)
    {
        rigid.isKinematic = true;
        yield return new WaitForSeconds(0.1f);
        rigid.isKinematic = false;
        transform.position = newRespawnPos;
    }
}