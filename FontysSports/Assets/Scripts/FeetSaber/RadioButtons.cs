using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor;
using UnityEngine.Events;
using System.Collections.Generic;

public class RadioButton : MonoBehaviour
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
    private TextMeshProUGUI[] texts;  // Array to hold TextMeshProUGUI components for labels
    private GameObject radioButton;

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

            CreateRadioButton(fileName, index, clip);
        }
    }

    void CreateRadioButton(string songName, int index, AudioClip clip)
    {
        GameObject radioButton = Instantiate(radioButtonPrefab, parentPanel);

        var texts = radioButton.GetComponentsInChildren<TextMeshProUGUI>(true);

        foreach (var text in texts)
        {
            Debug.Log("Found TMP text: " + text.gameObject.name);
            if (text.gameObject.name == "Label")
            {
                text.text = songName;
                text.ForceMeshUpdate();
                Debug.Log("Set text to: " + songName);
                break;
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)radioButton.transform);

        Toggle toggle = radioButton.GetComponent<Toggle>();
        if (toggle != null)
        {
            int capturedIndex = index;
            toggle.onValueChanged.AddListener((isOn) => OnRadioButtonClicked(isOn, capturedIndex));
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
