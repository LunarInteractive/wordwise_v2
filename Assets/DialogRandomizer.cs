using System;
using System.Collections;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using UnityEditor;
using UnityEngine;

public class DialogRandomizer : MonoBehaviour
{
    public DialogueSystemTrigger dialogueSystemTrigger;
    private DialogueDatabase dialogueDatabase;
    private List<string> blacklistedDialogues = new List<string>();

    // Start is called before the first frame update
    void Start()
    {
        dialogueSystemTrigger = GetComponent<DialogueSystemTrigger>();
        dialogueDatabase = dialogueSystemTrigger.selectedDatabase;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "npc")
        {
            print(other);
        }
    }

    public void Randomize_Dialog()
    {
        List<Conversation> m_conversations = dialogueDatabase.conversations;
        Debug.Log("Randomizing Dialogue");

        // Filter out blacklisted dialogues
        List<Conversation> availableConversations = m_conversations.FindAll(c => !blacklistedDialogues.Contains(c.Title));

        if (availableConversations.Count == 0)
        {
            Debug.LogWarning("All dialogues have been used. Resetting blacklist.");
            blacklistedDialogues.Clear();
            availableConversations = new List<Conversation>(m_conversations);
        }

        string selectedConversation = availableConversations[UnityEngine.Random.Range(0, availableConversations.Count)].Title;

        // Add to blacklist
        blacklistedDialogues.Add(selectedConversation);

        print(selectedConversation);
        DialogueSystemController dialogueManager = DialogueManager.instance;
        dialogueManager.StartConversation(selectedConversation);
    }

    public void Clear(){
        blacklistedDialogues.Clear();
    }
}
