using UnityEngine;

public struct GolfPlayerScoreInfo
{
    public string Name { get; private set; }
    
    public int TotalHits { get; private set; }

    public int CurrentHits { get; private set; }

    public GolfPlayerScoreInfo(string name, int totalHits, int currentHits)
    { 
        Name = name;
        TotalHits = totalHits;
        CurrentHits = currentHits;
    }
}
