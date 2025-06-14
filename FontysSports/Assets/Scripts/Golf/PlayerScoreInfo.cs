using UnityEngine;

public struct PlayerScoreInfo
{
    public string Name { get; private set; }
    
    public int TotalHits { get; private set; }

    public int CurrentHits { get; private set; }

    public PlayerScoreInfo(string name, int totalHits, int currentHits)
    { 
        Name = name;
        TotalHits = totalHits;
        CurrentHits = currentHits;
    }
}
