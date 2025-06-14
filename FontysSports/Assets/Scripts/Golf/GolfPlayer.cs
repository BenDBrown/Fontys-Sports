using System;
using UnityEngine;

public class GolfPlayer : ICloneable
{
    public string Name { get; protected set; }

    public int TotalHits { get; private set; } = 0;

    public int CurrentHits { get; private set; } = 0;

    private GameObject golfClub;

    public GolfPlayer(string name, GameObject golfClub)
    {
        Name = name;
        this.golfClub = golfClub;
        TotalHits = 0;
        CurrentHits = 0;
    }

    public void IncrementCurrentHits()
    {
        TotalHits++;
        CurrentHits++;
    }

    public void SetGolfClubActive(bool active)
    {
        if (golfClub == null) Debug.LogWarning("tried affecting golf club which was null");
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

    public void FinishTurn() => CurrentHits = 0;

    public virtual object Clone()
    {
        return new GolfPlayer(this.Name, null);
    }
}
