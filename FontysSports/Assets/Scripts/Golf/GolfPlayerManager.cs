using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class GolfPlayerManager : MonoBehaviour
{
    public UnityEvent<GolfPlayerScoreInfo[]> PlayerScoreChanged = new();

    public UnityEvent GameFinished = new();

    [SerializeField]
    private Rigidbody golfBallRigid;

    [SerializeField]
    private float teleportToBallMinimumDelay = 3;

    [SerializeField]
    private CourseObjAndBallSpawnLocation[] levelArray; // first item in list will be first level 2nd item will be 2nd lvl etc

    [SerializeField]
    private TeleportationProvider playerTeleportationProvider;

    public GolfPlayer CurrentPlayer => Players[currentPlayerIndex];

    public GolfPlayer[] Players { get; private set; }

    public GameObject CurrentLevel => levelArray[currentLevelIndex].Level;

    private Transform golfBallSpawnLocation => levelArray[currentLevelIndex].GolfBallSpawnLocation;

    private GolfBall golfBall;

    // these two readonly floats decide at what point the golfball is considered to be done moving
    private readonly float stationaryVelocityThreshold = 0.01f;

    private int currentPlayerIndex = 0;

    private int currentLevelIndex = 0;

    private float initialGolfballHeight = 0;
    
    private bool checkingGolfBallSpeed = false;

    private bool playing = false;

    private void Start()
    {
        golfBall = golfBallRigid.GetComponent<GolfBall>();
    }

    private void Update()
    {
        if (!checkingGolfBallSpeed) return;
        if (golfBallRigid.linearVelocity.magnitude <= stationaryVelocityThreshold)
        {
            checkingGolfBallSpeed = false;
            if (CurrentPlayer.CurrentHits >= GolfPlayer.SCORE_PER_COURSE_MAX)
            {
                PlayerScored();
                return;
            }
            if (golfBall.PoseInvalid) ResetBallInvalidPose();
            PrepPlayerForNextHit();
        }
    }

    public void StartMatch(GolfPlayer[] players)
    { 
        Players = players;
        currentLevelIndex = 0;
        currentPlayerIndex = 0;
        playing = true;
        foreach (GolfPlayer player in Players)
        { 
            player.ResetTotalHits();
            if (player.IsHuman) continue;
            if (!player.TryGetComponent(out GolfAI ai))
            {
                Debug.LogWarning("Non human player did not have attached ai script");
                continue;
            }
            ai.SetGolfballInfo(golfBallRigid, GetGolfballHeightDelta);
        }
        PlayerScoreChanged?.Invoke(GetPlayerScores());
        ResetBallLocation();
        PrepPlayerForNextHit();
        CurrentPlayer.StartTurn(CurrentLevel);
    }

    public void PlayerScored()
    {
        if(!playing) return;
        StopAllCoroutines();
        PlayerScoreChanged?.Invoke(GetPlayerScores());
        NextTurn();
    }

    public void IncrementGolfHits()
    {
        CurrentPlayer.IncrementCurrentHits();
        PlayerScoreChanged?.Invoke(GetPlayerScores());
        StartCoroutine(StartTeleportCheckAfterDelay(teleportToBallMinimumDelay));
    }

    public void ResetBallLocation() => golfBallRigid.transform.SetWorldPose(golfBallSpawnLocation.GetWorldPose());

    public void ResetBallInvalidPose() => golfBallRigid.transform.SetWorldPose(golfBall.PrevPose);

    private void NextTurn()
    {
        CurrentPlayer.EndTurn();
        if (currentPlayerIndex + 1 >= Players.Length)
        {
            currentPlayerIndex = 0;
            if (!TryPlayNextLevel())
            {
                playing = false;
                return;
            }
        }
        else
        {
            currentPlayerIndex++;
            ResetBallLocation();
        }
        PrepPlayerForNextHit();
        CurrentPlayer.StartTurn(CurrentLevel);
    }

    private bool TryPlayNextLevel()
    {
        if (currentLevelIndex + 1 >= levelArray.Length)
        {
            GameFinished?.Invoke();
            checkingGolfBallSpeed = false;
            return false;
        }
        else 
        {
            currentLevelIndex++;
            ResetCurrentHits();
            ResetBallLocation();
            initialGolfballHeight = golfBallRigid.position.y;
            return true;
        }
    }

    private void ResetCurrentHits()
    {
        foreach (GolfPlayer golfPlayer in Players)
        { 
            golfPlayer.ResetCurrentHits();
        }
        PlayerScoreChanged?.Invoke(GetPlayerScores());
    }

    private GolfPlayerScoreInfo[] GetPlayerScores()
    {
        List<GolfPlayerScoreInfo> playerScores = new();
        foreach (GolfPlayer golfPlayer in Players)
        {
            playerScores.Add(golfPlayer.PlayerScoreInfo);
        }
        return playerScores.ToArray();
    }

    private void PrepPlayerForNextHit()
    {
        if (CurrentPlayer.IsHuman)
        {
            TeleportRequest request = new()
            {
                destinationPosition = new(golfBallRigid.position.x, CurrentPlayer.InitialHeight + GetGolfballHeightDelta(), golfBallRigid.position.z),
                matchOrientation = MatchOrientation.None
            };
            playerTeleportationProvider.QueueTeleportRequest(request);
        }
        else CurrentPlayer.transform.position = new(golfBallRigid.position.x, CurrentPlayer.InitialHeight + GetGolfballHeightDelta(), golfBallRigid.position.z);
        CurrentPlayer.StartHit();
    }

    private float GetGolfballHeightDelta() => golfBallRigid.position.y - initialGolfballHeight;

    private IEnumerator StartTeleportCheckAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        checkingGolfBallSpeed = true;
    }

    [Serializable]
    public class CourseObjAndBallSpawnLocation
    {
        public GameObject Level;

        public Transform GolfBallSpawnLocation;
    }

}


