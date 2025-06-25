using System;
using System.Collections;
using System.Collections.Generic;
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

    private GameObject currentGolfLevel;

    private Rigidbody golfBall;

    private SplineAnimate splineAnimation;

    private Pose restingPose;

    private Vector3 finalTargetPos;

    private Vector3 currentTargetPos => GetCurrentTargetPos();

    private Vector3? nullableCurrentTargetPos = null;

    private List<AiTargetCheckpoint> currentlyCollidingCheckpoints = new();

    private float golfballHeightDelta => golfBallHeightDeltaFunc.Invoke();

    private Func<float> golfBallHeightDeltaFunc;

    private void Start()
    {
        if ((!TryGetComponent(out GolfPlayer player)) || (!clubHeadTargetPos.TryGetComponent(out SplineAnimate splineAnimation)))
        {
            Debug.LogError($"Golf AI does not have a GolfPlayer or SplineAnimate component as is required");
            enabled = false;
            return;
        }
        player.TurnStarted += OnTurnStart;
        player.HitStarted += OnHitStart;
        player.TurnEnded += OnTurnEnd;
        restingPose = transform.GetWorldPose();
        golfClub = player.GolfClub;
        this.splineAnimation = splineAnimation;
    }

    private void Update()
    {
        if (!splineAnimation.IsPlaying) return;
        leftHandOffset.position = leftHandHoldingPosition.position;
    }

    public void OnBallHit(Rigidbody rb)
    {
        if (!rb.TryGetComponent(out GolfBall ball)) return;
        splineAnimation.Completed -= RestartHit;
        float randomizedHitPower = (hitPower + UnityEngine.Random.Range(-hitPowerRandomness, hitDirectionRandomness)) * (currentTargetPos - transform.position).magnitude;
        Vector3 randomizedHitDirection = Quaternion.Euler(0, UnityEngine.Random.Range(-hitDirectionRandomness, hitDirectionRandomness), 0) * -transform.TransformDirection(Vector3.forward);
        rb.AddForce(randomizedHitDirection * randomizedHitPower);
        ball.TriggerHit();
    }

    private void RotatePlayerToTarget()
    {
        transform.LookAt(new Vector3(currentTargetPos.x, transform.position.y, currentTargetPos.z), Vector3.up);
        transform.Rotate(0, 180, 0);
    }

    private void OnTurnStart(GameObject currentLevel)
    {
        currentGolfLevel = currentLevel;
        golfClub.SetActive(true);
        FindHolePosition();
        ListenToCheckPoints();
    }

    private void OnHitStart()
    {
        splineAnimation.Completed += RestartHit;
        RotatePlayerToTarget();
        StartCoroutine(HitBall());
    }

    private void OnTurnEnd()
    {
        Debug.Log("Turn ending");
        transform.SetWorldPose(restingPose);
        golfClub?.SetActive(false);
        StopAllCoroutines();
        StopListeningToCheckPoints();
    }

    public void SetGolfballInfo(Rigidbody golfballRb, Func<float> golfballHeightDeltaFunc)
    {
        golfBall = golfballRb;
        this.golfBallHeightDeltaFunc = golfballHeightDeltaFunc;
    }

    private void RestartHit()
    {
        Debug.LogWarning("AI tried hitting ball but missed. This could be due to the ball moving unexpectedly or it is an error");
        StopAllCoroutines();
        StartCoroutine(HitBall());
    }

    private void SetPositionToBall()
    {
        if (!TryGetComponent(out GolfPlayer player))
        {
            Debug.LogError("could not find GolfPlayer component on GolfAI object");
            return;
        }
        transform.position = new(golfBall.position.x, player.InitialHeight + golfballHeightDelta, golfBall.position.z);
    }

    private IEnumerator HitBall()
    {
        yield return new WaitForSeconds(hitDelay);
        SetPositionToBall();
        RotatePlayerToTarget();
        splineAnimation.Duration = hitDuration;
        splineAnimation.Restart(true);
    }

    private void ListenToCheckPoints()
    {
        List<AiTargetCheckpoint> checkpoints = new();
        foreach (AiTargetCheckpoint checkpoint in currentGolfLevel.GetComponentsInChildren<AiTargetCheckpoint>())
        {
            checkpoint.GolfBallEntered += SetCurrentTarget;
            checkpoint.GolfBallExited += RemoveCheckpointFromCollisions;
        }
    }

    private void StopListeningToCheckPoints()
    {
        List<AiTargetCheckpoint> checkpoints = new();
        foreach (AiTargetCheckpoint checkpoint in currentGolfLevel.GetComponentsInChildren<AiTargetCheckpoint>())
        {
            foreach (Delegate d in checkpoint.GolfBallEntered.GetInvocationList())
            {
                checkpoint.GolfBallEntered -= (AiTargetCheckpoint.GolfBallCollisionEventHandler)d;
            }
            foreach (Delegate d in checkpoint.GolfBallExited.GetInvocationList())
            {
                checkpoint.GolfBallExited -= (AiTargetCheckpoint.GolfBallCollisionEventHandler)d;
            }
        }
    }

    private void SetCurrentTarget(AiTargetCheckpoint checkpoint) => nullableCurrentTargetPos = checkpoint.Target;

    private void RemoveCheckpointFromCollisions(AiTargetCheckpoint checkpoint)
    { 
        if(currentlyCollidingCheckpoints.Contains(checkpoint)) currentlyCollidingCheckpoints.Remove(checkpoint);
        if (currentlyCollidingCheckpoints.Count <= 0)
        {
            nullableCurrentTargetPos = null;
            return;
        }
        SetCurrentTarget(currentlyCollidingCheckpoints[^1]);
    }

    private Vector3 GetCurrentTargetPos()
    { 
        if(!nullableCurrentTargetPos.HasValue) return finalTargetPos;
        return nullableCurrentTargetPos.Value;
    }

    private void FindHolePosition()
    {
        GolfPlayerManager[] managers = FindObjectsByType<GolfPlayerManager>(FindObjectsSortMode.None);
        if (managers.Length != 1)
        {
            Debug.LogError($"{managers.Length} {nameof(GolfPlayerManager)}(s) found. There must be only 1");
            return;
        }
        GolfPlayerManager manager = managers[0];
#nullable enable
        Transform? target = null;
        string holeTag = "Goal";
        foreach (Transform t in manager.CurrentLevel.transform)
        {
            if (t.tag != holeTag) continue;
            target = t;
        }
        if (target == null)
        {
            Debug.LogError($"could not find golf hole with tag {holeTag}");
            return;
        }
        finalTargetPos = target.position;
#nullable disable
    }

}
