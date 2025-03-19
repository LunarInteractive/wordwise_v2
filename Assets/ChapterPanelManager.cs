using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using TMPro;

public class ChapterPanelManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject tabContainer; // Tab container
    public GameObject chapterTemplate; // Prefab for chapter panel
    public GameObject levelButtonPrefab; // Prefab for level button

    [Header("JSON Settings")]
    public string jsonFileName = "tes_level_sekolah"; // Nama file JSON (tanpa ekstensi)
    public float containerPadding = 50f; // Padding kiri dan kanan untuk LevelsContainer

    void Start()
    {
        List<Dictionary<string, object>> data = LoadFilteredData();
        if (data != null && data.Count > 0)
        {
            Dictionary<string, ChapterData> organizedData = OrganizeDataByChapters(data);
            CreateChapterPanels(organizedData);
        }
        else
        {
            Debug.LogError("No data available.");
        }
    }

    List<Dictionary<string, object>> LoadFilteredData()
    {
        TextAsset jsonFile = Resources.Load<TextAsset>(jsonFileName);
        if (jsonFile == null)
        {
            Debug.LogError("JSON file not found: " + jsonFileName);
            return null;
        }

        string jsonString = jsonFile.text;
        try
        {
            JArray jsonArray = JArray.Parse(jsonString);
            var data = new List<Dictionary<string, object>>();

            foreach (var item in jsonArray)
            {
                Dictionary<string, object> entry = item.ToObject<Dictionary<string, object>>();
                data.Add(entry);
            }

            return data;
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to parse JSON: " + e.Message);
            return null;
        }
    }

    Dictionary<string, ChapterData> OrganizeDataByChapters(List<Dictionary<string, object>> data)
    {
        Dictionary<string, ChapterData> chaptersData = new Dictionary<string, ChapterData>();

        foreach (var entry in data)
        {
            string chapter = entry["chapter"].ToString();
            string level = entry["level"].ToString();
            string dialogueData = entry["dialogue_data"].ToString();
            string chapterName = "Chapter " + chapter;

            if (!chaptersData.ContainsKey(chapter))
            {
                chaptersData[chapter] = new ChapterData(chapterName);
            }

            chaptersData[chapter].AddLevel(level, dialogueData);
        }

        return chaptersData;
    }

    void CreateChapterPanels(Dictionary<string, ChapterData> chaptersData)
    {
        foreach (var chapter in chaptersData)
        {
            // Instantiate chapter panel
            GameObject chapterPanel = Instantiate(chapterTemplate, tabContainer.transform);
            chapterPanel.name = chapter.Key;

            // Get levels container
            Transform levelsContainer = chapterPanel.transform.Find("LevelsContainer");
            if (levelsContainer != null)
            {
                // Clear existing children (optional, in case this is reused)
                foreach (Transform child in levelsContainer)
                {
                    Destroy(child.gameObject);
                }

                GridLayoutGroup gridLayout = levelsContainer.GetComponent<GridLayoutGroup>();
                RectTransform containerRect = levelsContainer.GetComponent<RectTransform>();

                if (gridLayout == null || containerRect == null)
                {
                    Debug.LogError("LevelsContainer must have a GridLayoutGroup and RectTransform!");
                    continue;
                }

                // Set GridLayoutGroup constraints for 1 row
                int totalLevels = chapter.Value.levels.Count;
                gridLayout.constraint = GridLayoutGroup.Constraint.FixedRowCount;
                gridLayout.constraintCount = 1; // Only one row

                // Calculate the number of columns dynamically
                int columns = totalLevels;
                float buttonWidth = gridLayout.cellSize.x;
                float buttonSpacing = gridLayout.spacing.x;

                // Calculate total width for LevelsContainer
                float totalWidth = (columns * buttonWidth) + ((columns - 1) * buttonSpacing);
                totalWidth = totalWidth / 10; 

                // Adjust container size
                containerRect.sizeDelta = new Vector2(totalWidth + (2 * containerPadding), containerRect.sizeDelta.y);

                // Adjust padding
                gridLayout.padding.left = Mathf.RoundToInt(containerPadding);
                gridLayout.padding.right = Mathf.RoundToInt(containerPadding);

                // Add buttons in correct order
                foreach (var level in chapter.Value.levels)
                {
                    // Create level button
                    GameObject levelButton = Instantiate(levelButtonPrefab, levelsContainer);
                    levelButton.GetComponentInChildren<TMP_Text>().text = "Level " + level.Key;

                    // Add listener to button
                    Button button = levelButton.GetComponent<Button>();
                    if (button != null)
                    {
                        string dialogueData = level.Value; // Capture dialogue data for the level
                        button.onClick.AddListener(() => OnLevelButtonPressed(chapter.Key, level.Key, dialogueData));
                    }
                }
            }
            else
            {
                Debug.LogError("LevelsContainer not found in Chapter Panel: " + chapter.Key);
            }
        }

        TabContainerManager tabContainerManager = FindObjectOfType<TabContainerManager>();
        if (tabContainerManager != null)
        {
            tabContainerManager.InitializeTabs();
        }
        else
        {
            Debug.LogError("TabContainerManager not found in the scene.");
        }
    }

    void OnLevelButtonPressed(string chapter, string level, string dialogueData)
    {
        GlobalVariables.Chapter = chapter;
        GlobalVariables.Level = level;
        GlobalVariables.DialogueData = dialogueData;

        Debug.Log($"Chapter {chapter}, Level {level} selected!");
        Debug.Log($"Dialogue Data: {dialogueData}");

        // Load the level scene
        UnityEngine.SceneManagement.SceneManager.LoadScene("Scenes/Level/Chapter_1/Level_School");
    }

    class ChapterData
    {
        public string chapterName;
        public SortedDictionary<string, string> levels; // Key: Level, Value: Dialogue Data (sorted by level)

        public ChapterData(string name)
        {
            chapterName = name;
            levels = new SortedDictionary<string, string>(); // Sorted to ensure correct order
        }

        public void AddLevel(string level, string dialogueData)
        {
            levels[level] = dialogueData;
        }
    }
}
