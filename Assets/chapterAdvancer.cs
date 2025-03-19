using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;

public class chapterAdvancer : MonoBehaviour
{
    public GameObject npcPrefab;
    public int dialogDone = 0;
    public int DialogMaxs;
    public Transform parent;
    public Transform spawnPosition;
    public Transform target;
    public GameObject CompleteMenu;
    public GameObject DialogueManager;


    public Text scoreText;

    private int score;

    void Start()
    {
        UpdateScoreFromDialogueDatabase();
    }

    public void addDialogDone(int value = 1)
    {
        dialogDone += value;
        if (dialogDone < DialogMaxs)
        {
            GameObject rawr = Instantiate(npcPrefab, spawnPosition.position, Quaternion.identity, parent);
            npc_wise npc = rawr.GetComponent<npc_wise>();
            npc.target = target;
        }
        else
        {
            Destroy(DialogueManager);
            CompleteMenu.SetActive(true);
        }
    }

    public void showScore()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score.ToString();
        }
    }

    public void UpdateScoreFromDialogueDatabase()
    {
        score = DialogueLua.GetVariable("ptsTemp").AsInt;
        showScore();
    }

    public void destroyDialogue()
    {
        Destroy(DialogueManager);
    }
}
