using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit.Inputs.Simulation;

public class GolfPlayer : MonoBehaviour
{
    public delegate void TurnStartedEventHandler(GameObject currentLevel);

    public delegate void TurnStatusChangeEventHandler();

    public TurnStartedEventHandler TurnStarted;

    public TurnStatusChangeEventHandler HitStarted;

    public TurnStatusChangeEventHandler TurnEnded;

    public const int SCORE_PER_COURSE_MAX = 7;

    [SerializeField]
    private GameObject golfClub;

    [SerializeField]
    private string playerName;

    [SerializeField]
    private bool isHuman = false; // this will probably need replacing with an enum if we want to do multiplayer and then a seperate logic flow will be needed for non-host players

    public GameObject GolfClub => golfClub;

    public string Name => playerName;

    public bool IsHuman => isHuman;

    public int TotalHits { get; private set; } = 0;

    public int CurrentHits { get; private set; } = 0;

    public float InitialHeight { get; private set; } = 0;

    public GolfPlayerScoreInfo PlayerScoreInfo => new(Name, TotalHits, CurrentHits);

    private void Start()
    {
        if (IsHuman)
        {
            InputDevice device = InputDevices.GetDeviceAtXRNode(XRNode.Head);
            if (!device.TryGetFeatureValue(CommonUsages.devicePosition, out Vector3 devicePos))
            {
                Debug.Log("could not get device postion when teleporting player, trying to find device simulator");
                XRDeviceSimulator xRDeviceSimulator = FindFirstObjectByType<XRDeviceSimulator>(FindObjectsInactive.Include);
                if (xRDeviceSimulator == null)
                {
                    Debug.LogWarning("No device simulator found and cannot find headset device. This will probably lead to incorrect player teleportation.");
                    return;
                }
                if ((!xRDeviceSimulator.enabled) || (!xRDeviceSimulator.gameObject.activeInHierarchy))
                { 
                    bool componentEnabled = xRDeviceSimulator.enabled;
                    bool objEnabled = xRDeviceSimulator.gameObject.activeInHierarchy;
                    xRDeviceSimulator.enabled = true;
                    xRDeviceSimulator.gameObject.SetActive(true);
                    InitialHeight = xRDeviceSimulator.cameraTransform.position.y;
                    xRDeviceSimulator.enabled = componentEnabled;
                    xRDeviceSimulator.gameObject.SetActive(objEnabled);
                }
                else InitialHeight = xRDeviceSimulator.cameraTransform.position.y;
                Debug.Log("Using XrDeviceSimulator Camera as headset position");
            }
            InitialHeight = devicePos.y;
        }
        else InitialHeight = transform.position.y;
    }

    public void IncrementCurrentHits()
    {
        if(CurrentHits >= SCORE_PER_COURSE_MAX) return;
        TotalHits++;
        CurrentHits++;
    }

    public void StartTurn(GameObject currentLevel)
    {
        SetGolfClubActive(true);
        TurnStarted?.Invoke(currentLevel);
        HitStarted?.Invoke();
    }

    public void StartHit() => HitStarted?.Invoke();

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
