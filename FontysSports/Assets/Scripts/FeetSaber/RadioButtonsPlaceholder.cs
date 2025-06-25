using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.Events;
using System.Collections.Generic;

public class RadioButtonsPlaceholder : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private GameObject radioButtonPrefab;  // Prefab for radio buttons (UI Toggle)
    [SerializeField]
    private Image selectButtonPrefab;  // Prefab for the select button (RadioButton)
    [SerializeField]
    private Transform parentPanel;         // Parent UI Panel to hold the radio buttons
    [SerializeField]
    private AudioSource audioSource;       // Reference to the AudioSource to play songs
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private Spawner spawner;
    [SerializeField]
    private List<AudioClip> songClips = new List<AudioClip>();  // List of audio clips

    void Start()
    {
        canvas = GetComponent<Canvas>();
        GenerateRadioButtons();
    }

    void GenerateRadioButtons()
    {
        foreach (Transform child in parentPanel)
        {
            Destroy(child.gameObject);
        }

        for (int index = 0; index < songClips.Count; index++)
        {
            AudioClip clip = songClips[index];
            string fileName = clip.name;
            Debug.Log($"Creating radio button for: {fileName} at index {index}");
            CreateRadioButton(fileName, index, clip);
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
            if (index >= 0 && index < songClips.Count)
            {
                audioSource.clip = songClips[index];  // Set the audio clip to the selected song
                audioSource.Play();  // Play the audio
                selectButtonPrefab.enabled = true; // Enable the select button if the radio button is selected
            }
        }
        else
        {
            selectButtonPrefab.enabled = false;  // Disable the select button if the radio button is not selected
        }
    }

    public void OnPlayButtonClicked()
    {
        canvas.enabled = false;

        if (audioSource.clip != null)
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
