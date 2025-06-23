using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GolfSetupManager : MonoBehaviour
{
    [SerializeField]
    private GolfPlayerManager golfPlayerManager;

    [SerializeField]
    private GolfPlayer hostPlayer; // multiplayer compatible naming, not yet implemented

    [SerializeField]
    private GolfPlayer[] npcPlayerPool;

    [SerializeField]
    private TextMeshProUGUI nrOfNpcsIndicator;

    [SerializeField]
    private GameObject MenuObject;

    [SerializeField]
    private int maxNrOfNpcs = 3;

    [SerializeField]
    private bool includePlayer = true;

    private int nrOfNpcs = 0;

    private void Start()
    {
        if(maxNrOfNpcs <= npcPlayerPool.Length) return;
        Debug.LogWarning($"max nr of npcs was set to a value higher than the available amount of NPC players. Max: {maxNrOfNpcs}, Available: {npcPlayerPool.Length}");
        maxNrOfNpcs = npcPlayerPool.Length;
    }

    public void IncrementNrOfNpcs()
    { 
        nrOfNpcs++;
        if(nrOfNpcs > maxNrOfNpcs) nrOfNpcs = maxNrOfNpcs;
        nrOfNpcsIndicator.text = nrOfNpcs.ToString();
    }

    public void DecreaseNrOfNpcs()
    {
        nrOfNpcs--;
        if (nrOfNpcs < 0) nrOfNpcs = 0;
        nrOfNpcsIndicator.text = nrOfNpcs.ToString();
    }

    public void StartGame()
    {
        List<GolfPlayer> chosenPlayers = new();
        if (includePlayer) chosenPlayers.Add(hostPlayer);
        for (int i = 0; i < nrOfNpcs; i++)
        {
            chosenPlayers.Add(npcPlayerPool[i]);
        }
        if (chosenPlayers.Count <= 0)
        {
            Debug.Log("Must have at least one player. Did you mean to have player character disabled?");
            return;
        }
        MenuObject.SetActive(false);
        golfPlayerManager.StartMatch(chosenPlayers.ToArray());
    }

    public void EnableMenu() => MenuObject.SetActive(true);
}
