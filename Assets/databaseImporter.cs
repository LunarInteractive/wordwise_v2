using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using System.IO;
using UnityEditor;

public class databaseImporter : MonoBehaviour
{
    [Header("Selected Source File")]
    public string sourceFilePath;

    [ContextMenu("rawr")]
    public void SelectSourceFile()
    {
        // Open a file panel for selecting a source file

        //string path = EditorUtility.OpenFilePanel("Select Source File", Application.dataPath, "json");
        string path = "";

#if UNITY_EDITOR
        path = EditorUtility.OpenFilePanelWithFilters("Select Image File", "", new string[] { "Image files", "png,jpg,bmp" });

#elif UNITY_ANDROID

        NativeFilePicker.PickFile((filePath) => { path = filePath; },("json"));
            
#endif


        if (!string.IsNullOrEmpty(path))
        {
            sourceFilePath = path;
            Debug.Log("Selected Source File: " + sourceFilePath);

            // Optionally, you can process the source file
            ProcessSourceFile(sourceFilePath);
        }
        else
        {
            Debug.LogWarning("No file selected.");
        }
    }

    private void ProcessSourceFile(string path)
    {
        try
        {
            json = System.IO.File.ReadAllText(path);
            Debug.Log("File Content:\n" + json);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to read the file: " + e.Message);
        }
    }

     
    private string json;
    public DialogueDatabase database;
    public DialogueSystemController controller;
    public void Start()
    {
        SelectSourceFile();
        JsonUtility.FromJsonOverwrite(json, database);
        controller.DatabaseManager.defaultDatabase = database;

        controller.StartConversation(controller.databaseManager.defaultDatabase.conversations[0].Title);
    }


}
