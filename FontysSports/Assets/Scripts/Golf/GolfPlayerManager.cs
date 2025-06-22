using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;

public class GolfPlayerManager : MonoBehaviour
{
    public UnityEvent<int> PlayerScoredWithHits = new();

    public UnityEvent<GolfPlayerScoreInfo[]> LevelFinished = new();

    public UnityEvent<GolfPlayerScoreInfo[]> GameFinished = new();

    [SerializeField]
    private Transform golfBallSpawnLocation; // assumed to be the same between levels, move the levels instead of wanting multiple spawn points

    [SerializeField]
    private Transform golfBall;

    [SerializeField]
    private List<GameObject> levels = new(); // first item in list will be first level 2nd item will be 2nd lvl etc

    public GolfPlayer CurrentPlayer => Players[currentPlayerIndex];

    public GolfPlayer[] Players { get; private set; }

    private int currentPlayerIndex = 0;

    private GameObject currentLevel => levels[currentLevelIndex];

    private int currentLevelIndex = 0;

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
        golfBall.SetWorldPose(golfBallSpawnLocation.GetWorldPose());
    }

    public void PlayerScored()
    {
        PlayerScoredWithHits?.Invoke(CurrentPlayer.CurrentHits);
        NextTurn();
    }

    public void IncrementGolfHits() => CurrentPlayer.IncrementCurrentHits();

    public void ResetBallLocation() => golfBall.SetWorldPose(golfBallSpawnLocation.GetWorldPose());

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
        golfBall.SetWorldPose(golfBallSpawnLocation.GetWorldPose());
    }

    private void NextLevel()
    {
        currentLevel.SetActive(false);
        if (currentLevelIndex + 1 >= levels.Count)
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
}
