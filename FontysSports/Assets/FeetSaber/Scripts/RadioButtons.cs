using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.Events;
using System.Collections.Generic;

public class RadioButton : MonoBehaviour
{
    [Header("References")]
    public string folderPath = "Assets/FeetSaber/Songs";  // Path to the folder
    public GameObject radioButtonPrefab;  // Prefab for radio buttons (UI Toggle)
    public Transform parentPanel;         // Parent UI Panel to hold the radio buttons
    public AudioSource audioSource;       // Reference to the AudioSource to play songs
    public Canvas canvas;
    public Spawner spawner;

    private List<AudioClip> songClips = new List<AudioClip>();  // List of audio clips

    void Start()
    {
        canvas = GetComponent<Canvas>();

        GenerateRadioButtons();
    }

    void GenerateRadioButtons()
    {
        // Get all assets in the specified folder using AssetDatabase
        string[] assetPaths = AssetDatabase.FindAssets("", new[] { folderPath });

        int index = 0;  // Index to associate each song with the toggle button

        foreach (string guid in assetPaths)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // Only consider audio files (you can extend this to other types if needed)
            if (assetPath.EndsWith(".mp3") || assetPath.EndsWith(".ogg"))
            {
                // Create a new radio button for each audio file
                string fileName = System.IO.Path.GetFileNameWithoutExtension(assetPath); // Get file name without extension
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);  // Load the audio clip
                songClips.Add(clip);  // Add clip to the list

                // Create radio button
                CreateRadioButton(fileName, index, clip);
                index++;  // Increment index for next song
            }
        }
    }

    void CreateRadioButton(string fileName, int index, AudioClip clip)
    {
        // Instantiate the radio button prefab
        GameObject radioButton = Instantiate(radioButtonPrefab, parentPanel);

        // Set the label of the radio button
        Text buttonText = radioButton.GetComponentInChildren<Text>();
        if (buttonText != null)
        {
            buttonText.text = fileName; // Set the name of the file as the button label
        }

        // Get the Toggle component and assign the listener
        Toggle toggle = radioButton.GetComponent<Toggle>();
        if (toggle != null)
        {
            // Add a listener to the toggle to handle the click event
            toggle.onValueChanged.AddListener((isOn) => OnRadioButtonClicked(isOn, index));
        }
    }

    // Called when a radio button is clicked
    void OnRadioButtonClicked(bool isOn, int index)
    {
        if (isOn)
        {
            // Play the corresponding song based on the selected radio button
            if (index >= 0 && index < songClips.Count)
            {
                audioSource.clip = songClips[index];  // Set the audio clip to the selected song
                audioSource.Play();  // Play the audio
            }
        }
    }

    public void OnPlayButtonClicked(){
        canvas.enabled = false;
        
        if(audioSource.clip != null)
        {
            audioSource.Stop();  // Stop the current song if it's playing
        }

        StartCoroutine(DelayedStart());
    }

    IEnumerator DelayedStart()
    {
        yield return new WaitForSeconds(2); // Wait for 2 seconds before playing

        if (audioSource.clip != null)
        {
            audioSource.Play();  // Restart the song from the beginning
        }

        if (spawner != null)
        {
            spawner.StartSpawning();  // Start spawning after delay
        }
    }
}
