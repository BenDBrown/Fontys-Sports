using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GolfPlayerManager : MonoBehaviour
{
    public UnityEvent<int> PlayerScoredWithHits = new();

    public UnityEvent<GolfPlayer[]> LevelFinished = new();

    public UnityEvent<GolfPlayer[]> GameFinished = new();

    [SerializeField]
    private Transform golfBallSpawnLocation; // assumed to be the same between levels, move the levels instead of wanting multiple spawn points

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
        CurrentPlayer.SetGolfClubActive(true);
    }

    public void PlayerScored()
    {
        PlayerScoredWithHits?.Invoke(CurrentPlayer.CurrentHits);
        NextTurn();
    }

    public void IncrementGolfHits() => CurrentPlayer.IncrementCurrentHits();

    private void NextTurn()
    {
        CurrentPlayer.SetGolfClubActive(false);
        if (currentPlayerIndex + 1 >= Players.Length)
        {
            currentPlayerIndex = 0;
            NextLevel();
        }
        else currentPlayerIndex++;
        CurrentPlayer.SetGolfClubActive(true);
    }

    private void NextLevel()
    {
        currentLevel.SetActive(false);
        if (currentLevelIndex + 1 >= levels.Count)
        {
            GameFinished?.Invoke(CloningUtils.GetCloneOf<GolfPlayer>(Players));
        }
        else 
        {
            currentLevelIndex++;
            LevelFinished?.Invoke(CloningUtils.GetCloneOf<GolfPlayer>(Players));
            currentLevel.SetActive(true);
        }
    }
}
