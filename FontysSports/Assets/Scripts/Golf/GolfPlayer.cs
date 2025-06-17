using System;
using UnityEngine;
using UnityEngine.Events;

public class GolfPlayer : MonoBehaviour
{
    public UnityEvent TurnStarted = new();

    public UnityEvent TurnEnded = new();

    [SerializeField]
    private GameObject golfClub;

    public string Name { get; protected set; }

    public int TotalHits { get; private set; } = 0;

    public int CurrentHits { get; private set; } = 0;

    public GolfPlayerScoreInfo PlayerScoreInfo => new(Name, TotalHits, CurrentHits);

    public void IncrementCurrentHits()
    {
        if(CurrentHits >= 7) return;
        TotalHits++;
        CurrentHits++;
    }

    public void StartTurn()
    {
        SetGolfClubActive(true);
        TurnStarted?.Invoke();
    }

    public void EndTurn()
    {
        SetGolfClubActive(false);
        TurnEnded?.Invoke();    
    }

    public void ResetCurrentHits() => CurrentHits = 0;

    public void ResetTotalHits()
    { 
        CurrentHits = 0;
        TotalHits = 0;
    }

    private void SetGolfClubActive(bool active)
    {
        if (golfClub == null)
        {
            Debug.LogWarning("tried affecting golf club which was null");
            return;
        }
        golfClub.SetActive(active);
        foreach (Component comp in golfClub.GetComponentsInChildren(typeof(Component)))
        {
            if (comp is Collider col) col.enabled = active;
            else if (comp is MeshRenderer mr) mr.enabled = active;
            else if (comp is MonoBehaviour mb)
            {
                mb.StopAllCoroutines();
                mb.enabled = active;
            }
        }
    }
}
