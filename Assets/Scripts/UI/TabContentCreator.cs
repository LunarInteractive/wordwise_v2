using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabContentCreator : MonoBehaviour
{
    [Header("UI References")]
    public GameObject linePrefab;
    public GameObject levelPrefab;
    public Transform container;
    // Tambahkan reference ke DialogManager
    public DialogManager dialogManager;

    [HideInInspector]
    public string chapterName;

    public void CreateLevelButtons(List<LevelData> levels, GameObject transition)
    {
        // Remove old children
        foreach (Transform child in container)
        {
            Destroy(child.gameObject);
        }

        // Create a button for each level
        foreach (var lvl in levels)
        {
            GameObject newButtonObj = Instantiate(levelPrefab, container);
            newButtonObj.name = $"Level_{lvl.level_name}";

            // Get all TextMeshProUGUI components in the new button
            TextMeshProUGUI[] textComponents = newButtonObj.GetComponentsInChildren<TextMeshProUGUI>();

            if (textComponents.Length > 0)
            {
                // Assume the first text component is the main text.
                textComponents[0].text = lvl.level_name;

                // If there's a second text component, treat it as the shadow.
                if (textComponents.Length > 1)
                {
                    textComponents[1].text = lvl.level_name;
                }
                else
                {
                    Debug.Log("No shadow text detected.");
                }
            }
            else
            {
                Debug.LogError("No TextMeshProUGUI component found in children!");
            }

            Button btn = newButtonObj.GetComponent<Button>();
            if (btn != null)
            {
                // Pass 'lvl' to the event handler
                btn.onClick.AddListener(() => OnLevelButtonClicked(transition, lvl));
            }
        }
    }


    private void OnLevelButtonClicked(GameObject transition, LevelData level)
    {
        transition.SetActive(true);

        // Memanggil method di DialogManager
        if (dialogManager != null)
        {
            dialogManager.FetchDialogueAndCreateXML(level.class_id, level.level_name);
            UserDataSession.levelName = level.level_name;
            UserDataSession.levelID = level.id.ToString();
        }
        else
        {
            Debug.LogError("DialogManager reference not assigned!");
        }
    }
}
