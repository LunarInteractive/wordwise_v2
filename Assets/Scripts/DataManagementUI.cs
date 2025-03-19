using UnityEngine;
using TMPro;
using static PlayerProgressData;
using System.Collections.Generic;

public class DataManagementUI : MonoBehaviour
{
    public PlayFabManager playFabManager;
    public TMP_Dropdown chapterDropdown;
    public Transform contentPanel; // Reference to the Scroll View's Content panel
    public GameObject levelScorePrefab; // Prefab for the Level and Score display

    void Start()
    {
        // Initialize the UI
        InitializeChapterDropdown();
    }

    void InitializeChapterDropdown()
    {
        // Clear existing options
        chapterDropdown.ClearOptions();

        // Populate dropdown with chapter names
        List<string> chapterNames = new List<string>();
        foreach (var chapter in playFabManager.playerProgress.Chapters)
        {
            chapterNames.Add(chapter.ChapterName);
        }

        chapterDropdown.AddOptions(chapterNames);

        // Set the first chapter as default and display its levels
        chapterDropdown.onValueChanged.AddListener(delegate { OnChapterSelected(chapterDropdown); });
        OnChapterSelected(chapterDropdown);
    }

    void OnChapterSelected(TMP_Dropdown dropdown)
    {
        // Get the selected chapter name
        string selectedChapter = dropdown.options[dropdown.value].text;

        // Clear previous level-score entries
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }

        // Find the selected chapter data
        ChapterData chapterData = playFabManager.playerProgress.Chapters
                                      .Find(c => c.ChapterName == selectedChapter);

        // Populate the Scroll View with levels and scores
        foreach (var level in chapterData.Levels)
        {
            GameObject newEntry = Instantiate(levelScorePrefab, contentPanel);
            TMP_Text[] texts = newEntry.GetComponentsInChildren<TMP_Text>();
            texts[0].text = level.LevelName; // Level number
            texts[1].text = level.Score.ToString(); // Score
        }
    }
}
