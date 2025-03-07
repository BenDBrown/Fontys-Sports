using UnityEngine;
using UnityEngine.UI;
using UnityEditor; // Only required for AssetDatabase (optional for loading files at runtime)
using System.Collections.Generic;

public class SongSelector : MonoBehaviour
{
    public ToggleGroup toggleGroup;  // Reference to the ToggleGroup containing all the radio buttons
    public AudioSource audioSource;  // Reference to the AudioSource that will play the songs

    // List of songs you want to use in your game (can be manually assigned or loaded dynamically)
    public List<AudioClip> songClips = new List<AudioClip>();

    void Start()
    {
        songClips = LoadSongs();  // Load your song clips if using AssetDatabase or Resources folder

        // Add listeners to each toggle in the ToggleGroup
        foreach (Toggle toggle in toggleGroup.ActiveToggles())
        {
            toggle.onValueChanged.AddListener((isOn) => OnToggleChanged(toggle, isOn));
        }
    }

    // Called when a toggle is clicked
    void OnToggleChanged(Toggle toggle, bool isOn)
    {
        if (isOn)
        {
            // Get the song index based on the toggle clicked (index should match the order of toggles)
            int index = toggle.transform.GetSiblingIndex();  // Get the index of the clicked toggle in the hierarchy
            
            if (index >= 0 && index < songClips.Count)
            {
                // Play the selected song
                PlaySong(songClips[index]);
            }
        }
    }

    // Play the song by setting the AudioClip in the AudioSource and playing it
    void PlaySong(AudioClip clip)
    {
        if (clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    // Optional: Dynamically load songs using AssetDatabase or Resources
    List<AudioClip> LoadSongs()
    {
        // For example, loading all audio clips in a Resources folder
        return new List<AudioClip>(Resources.LoadAll<AudioClip>("Songs"));
    }
}
