using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class GolfPlayerManager : MonoBehaviour
{
    public UnityEvent<int> PlayerScoredWithHits = new();

    public UnityEvent<GolfPlayerScoreInfo[]> LevelFinished = new();

    public UnityEvent<GolfPlayerScoreInfo[]> GameFinished = new();

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

    private Transform golfBallSpawnLocation => levelArray[currentLevelIndex].GolfBallSpawnLocation;

    private GameObject currentLevel => levelArray[currentLevelIndex].Level;

    private GolfBall golfBall;

    // these two readonly floats decide at what point the golfball is considered to be done moving
    private readonly float stationaryVelocityThreshold = 0.01f;

    private int currentPlayerIndex = 0;

    private int currentLevelIndex = 0;

    private float initialGolfballHeight = 0;
    
    private bool checkingGolfBallSpeed = false;

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
            if (!golfBall.PoseInvalid) PrepPlayerForNextHit();
            else ResetBallInvalidPose();
        }
    }

    public void StartMatch(GolfPlayer[] players)
    { 
        Players = players;
        currentLevelIndex = 0;
        currentPlayerIndex = 0;
        currentLevel.SetActive(true);
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
        ResetBallLocation();
        PrepPlayerForNextHit();
        CurrentPlayer.StartTurn();
    }

    public void PlayerScored()
    {
        StopAllCoroutines();
        PlayerScoredWithHits?.Invoke(CurrentPlayer.CurrentHits);
        NextTurn();
    }

    public void IncrementGolfHits()
    {
        CurrentPlayer.IncrementCurrentHits();
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
            if(!TryPlayNextLevel()) return;
        }
        else
        {
            currentPlayerIndex++;
            ResetBallLocation();
        }
        PrepPlayerForNextHit();
        CurrentPlayer.StartTurn();
    }

    private bool TryPlayNextLevel()
    {
        currentLevel.SetActive(false);
        if (currentLevelIndex + 1 >= levelArray.Length)
        {
            GameFinished?.Invoke(GetPlayerScores());
            checkingGolfBallSpeed = false;
            return false;
        }
        else 
        {
            currentLevelIndex++;
            LevelFinished?.Invoke(GetPlayerScores());
            ResetCurrentHits();
            currentLevel.SetActive(true);
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


