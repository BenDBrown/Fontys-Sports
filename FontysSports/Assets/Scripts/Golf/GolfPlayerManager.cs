using System;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class GolfPlayerManager : MonoBehaviour
{
    public UnityEvent<int> PlayerScoredWithHits = new();

    public UnityEvent<GolfPlayerScoreInfo[]> LevelFinished = new();

    public UnityEvent<GolfPlayerScoreInfo[]> GameFinished = new();

    [SerializeField]
    private Rigidbody golfBall;

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

    private int currentPlayerIndex = 0;

    private int currentLevelIndex = 0;
    
    private bool checkingGolfBallSpeed = false;

    private void Update()
    {
        if (!checkingGolfBallSpeed) return;
        if (golfBall.linearVelocity.magnitude <= 0.01f)
        {
            InputDevice device = InputDevices.GetDeviceAtXRNode(XRNode.Head);
            if (!device.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 devicePos))
            {
                Debug.LogWarning("could not get device postion when teleporting player");
                checkingGolfBallSpeed = false;
                return;
            }
            TeleportRequest request = new()
            {
                destinationPosition = new(golfBall.position.x, devicePos.y, golfBall.position.z),
                matchOrientation = MatchOrientation.None
            };
            playerTeleportationProvider.QueueTeleportRequest(request);
            checkingGolfBallSpeed = false;
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
        }
        CurrentPlayer.StartTurn();
        ResetBallLocation();
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

    public void ResetBallLocation() => golfBall.transform.SetWorldPose(golfBallSpawnLocation.GetWorldPose());

    private void NextTurn()
    {
        CurrentPlayer.EndTurn();
        if (currentPlayerIndex + 1 >= Players.Length)
        {
            currentPlayerIndex = 0;
            NextLevel();
        }
        else currentPlayerIndex++;
        CurrentPlayer.StartTurn();
        ResetBallLocation();
    }

    private void NextLevel()
    {
        currentLevel.SetActive(false);
        if (currentLevelIndex + 1 >= levelArray.Length)
        {
            GameFinished?.Invoke(GetPlayerScores());
        }
        else 
        {
            currentLevelIndex++;
            LevelFinished?.Invoke(GetPlayerScores());
            ResetCurrentHits();
            currentLevel.SetActive(true);
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


