using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SpeechCheckerPanel : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject panelSpeechChecker;
    public TMP_Text similarityText;

    public int SimilarityScore;



    public void ShowSpeechCheckerPanel()
    {
        if (panelSpeechChecker != null && similarityText != null)
        {
            panelSpeechChecker.SetActive(true);
            similarityText.text = $"Similarity Score: {SimilarityScore}%";
        }
        else
        {
            Debug.LogError("panelSpeechChecker or similarityText is not assigned.");
        }
    }
}
