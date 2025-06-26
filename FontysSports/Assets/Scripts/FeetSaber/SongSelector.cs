using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SongSelector : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("Parent GameObject with ToggleGroup component")]
    public GameObject toggleGroupObject;

    [Tooltip("Toggle prefab with Toggle component and a child Text component for label")]
    public Toggle togglePrefab;

    [Header("Audio")]
    [Tooltip("AudioSource used to play selected songs")]
    public AudioSource audioSource;

    [Header("Songs")]
    [Tooltip("Assign AudioClips manually here")]
    public List<AudioClip> songClips = new List<AudioClip>();

    private ToggleGroup toggleGroup;

    void Start()
    {
        if (toggleGroupObject == null)
        {
            Debug.LogError("Toggle Group Object is not assigned!");
            return;
        }

        toggleGroup = toggleGroupObject.GetComponent<ToggleGroup>();
        if (toggleGroup == null)
        {
            Debug.LogError("Toggle Group Object does not have a ToggleGroup component!");
            return;
        }

        if (togglePrefab == null)
        {
            Debug.LogError("Toggle Prefab is not assigned!");
            return;
        }

        if (audioSource == null)
        {
            Debug.LogError("AudioSource is not assigned!");
            return;
        }

        CreateToggles();
    }

    void CreateToggles()
    {
        // Clear existing toggles
        foreach (Transform child in toggleGroupObject.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < songClips.Count; i++)
        {
            AudioClip clip = songClips[i];
            Toggle toggle = Instantiate(togglePrefab, toggleGroupObject.transform);
            toggle.group = toggleGroup;

            // Set the label text to song name
            Text label = toggle.GetComponentInChildren<Text>();
            if (label != null)
                label.text = clip.name;
            else
                Debug.LogWarning("Toggle prefab missing a child Text component for label!");

            int index = i;  // capture loop variable for closure
            toggle.onValueChanged.AddListener((isOn) =>
            {
                if (isOn)
                {
                    PlaySong(songClips[index]);
                }
            });

            if (i == 0)
                toggle.isOn = true;  // Activate first toggle by default
        }
    }

    void PlaySong(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }
}
