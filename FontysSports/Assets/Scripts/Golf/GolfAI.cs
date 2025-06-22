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

    private GameObject golfClub;

    private SplineAnimate splineAnimation;

    private void Start()
    {
        if ((!TryGetComponent(out GolfPlayer player)) || (!clubHeadTargetPos.TryGetComponent(out SplineAnimate splineAnimation)))
        {
            Debug.LogError($"Golf AI does not have a GolfPlayer or SplineAnimate component as is required");
            return;
        }
        player.TurnStarted.AddListener(OnTurnStart);
        player.HitStarted.AddListener(OnHitStart);
        player.TurnEnded.AddListener(OnTurnEnd);
        golfClub = player.GolfClub;
        this.splineAnimation = splineAnimation;
        HitBall();
    }

    public void OnTurnStart()
    { 
        
    }

    public void OnHitStart()
    { 
        
    }

    public void OnTurnEnd()
    { 
        
    }

    private void HitBall()
    { 
        Vector3 lineFromHeadToTargetHeadPos = clubHeadTargetPos.position - clubHeadPos.position;
        rightHandOffset.position += lineFromHeadToTargetHeadPos;
        rightHandOffset.parent = clubHeadTargetPos;
        splineAnimation.Restart(true);
    }

}
