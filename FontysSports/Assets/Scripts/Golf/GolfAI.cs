using System.Collections;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Splines;


public class GolfAI : MonoBehaviour
{
    [SerializeField]
    private Transform leftHandHoldingPosition;

    [SerializeField]
    private Transform clubHeadTargetPos;

    [SerializeField]
    private Transform clubHeadPos;

    [SerializeField]
    private Transform rightHandOffset;

    [SerializeField]
    private Transform leftHandOffset;

    [SerializeField]
    private float hitDelay = 2; // time that the npc pauses to give the illusion of aiming

    [SerializeField]
    private float hitDuration = 1;

    [SerializeField]
    private float hitPower = 100;

    [SerializeField]
    private float hitPowerRandomness = 20;

    [SerializeField]
    private float hitDirectionRandomness = 10; // euler degrees

    private GameObject golfClub;

    private SplineAnimate splineAnimation;

    private Pose restingPose;

    private void Start()
    {
        if ((!TryGetComponent(out GolfPlayer player)) || (!clubHeadTargetPos.TryGetComponent(out SplineAnimate splineAnimation)))
        {
            Debug.LogError($"Golf AI does not have a GolfPlayer or SplineAnimate component as is required");
            enabled = false;
            return;
        }
        player.TurnStarted.AddListener(OnTurnStart);
        player.HitStarted.AddListener(OnHitStart);
        player.TurnEnded.AddListener(OnTurnEnd);
        restingPose = transform.GetWorldPose();
        golfClub = player.GolfClub;
        this.splineAnimation = splineAnimation;
    }

    private void Update()
    {
        if(!splineAnimation.IsPlaying) return;
        leftHandOffset.position = leftHandHoldingPosition.position;
    }

    public void OnBallHit(Rigidbody rb)
    {
        splineAnimation.Completed -= RestartHit;
        float randomizedHitPower = hitPower + Random.Range(-hitPowerRandomness, hitDirectionRandomness);
        Vector3 randomizedHitDirection = Quaternion.Euler(0, Random.Range(-hitDirectionRandomness, hitDirectionRandomness), 0) * transform.TransformDirection(Vector3.forward);
        rb.AddForce(randomizedHitDirection * randomizedHitPower);
    }

    private void OnTurnStart()
    { 
        golfClub.SetActive(true);    
    }

    private void OnHitStart()
    {
        splineAnimation.Completed += RestartHit;
        StartCoroutine(HitBall());
    }

    private void OnTurnEnd()
    { 
        transform.SetWorldPose(restingPose);
        golfClub?.SetActive(false);
        StopAllCoroutines();
    }

    private void RestartHit()
    {
        Debug.LogWarning("AI tried hitting ball but missed. This could be due to the ball moving unexpectedly or it is an error");
        StopAllCoroutines();
        StartCoroutine(HitBall());
    }

    private IEnumerator HitBall()
    {
        yield return new WaitForSeconds(hitDelay);
        splineAnimation.Duration = hitDuration;
        splineAnimation.Restart(true);
    }

}
