using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Question
{
    public string Text;
    public string Image;
    public string Translate;
}

[System.Serializable]
public class Option
{
    public string Text;
    public string Image;
    public string Translate;
    public string Action;
}

[System.Serializable]
public class MainDialog
{
    public string Character;
    public Question Question;
    public Option OptionA;
    public Option OptionB;
    public Option OptionC;
}

[System.Serializable]
public class Dialog
{
    public string Character;
    public Question Question;
    public Option OptionA;
    public Option OptionB;
    public Option OptionC;
}

public class ConversationXML : MonoBehaviour
{
    [Header("Configuration")]
    public string FolderName = "DialogueFiles";
    public string FileName = "CurrentDialogue.xml";

    [Header("Main Data")]
    public MainDialog MainDialog = new MainDialog();

    [Header("Additional Data")]
    public List<Dialog> AdditionalDialogs = new List<Dialog>();

    public UnityEvent AfterLoad;

    private void Start()
    {
        LoadConversation();
    }

    public void LoadConversationFromString(string xmlData)
    {
        if (string.IsNullOrEmpty(xmlData))
        {
            Debug.LogError("XML Data is null or empty.");
            return;
        }

        XDocument xmlDoc;
        try
        {
            xmlDoc = XDocument.Parse(xmlData);
        }
        catch (System.Xml.XmlException ex)
        {
            Debug.LogError($"Failed to parse XML: {ex.Message}");
            return;
        }

        LoadFromXDocument(xmlDoc);
    }

    public void LoadConversation()
    {
        string folderPath = Path.Combine(Application.persistentDataPath, FolderName);
        string filePath = Path.Combine(folderPath, FileName);

        if (!File.Exists(filePath))
        {
            Debug.LogError($"File not found: {filePath}");
            return;
        }

        XDocument xmlDoc;
        try
        {
            xmlDoc = XDocument.Load(filePath);
        }
        catch (System.Xml.XmlException ex)
        {
            Debug.LogError($"Failed to load XML: {ex.Message}");
            return;
        }

        LoadFromXDocument(xmlDoc);
    }

    private void LoadFromXDocument(XDocument xmlDoc)
    {
        AdditionalDialogs.Clear();
        MainDialog = new MainDialog();
        Dictionary<int, Dialog> dialogDict = new Dictionary<int, Dialog>();

        XElement root = xmlDoc.Root;

        foreach (XElement dialogNode in root.Elements())
        {
            string dialogKey = dialogNode.Name.LocalName;
            if (dialogKey.StartsWith("Dialog") && int.TryParse(dialogKey.Substring(6), out int dialogIndex))
            {
                Dialog dialog = new Dialog
                {
                    Character = dialogNode.Element("Character")?.Value,
                    Question = ParseQuestionNode(dialogNode.Element("Question"))
                };

                // Only add options if they exist in XML
                XElement optionANode = dialogNode.Element("OptionA");
                if (optionANode != null) dialog.OptionA = ParseOptionNode(optionANode);

                XElement optionBNode = dialogNode.Element("OptionB");
                if (optionBNode != null) dialog.OptionB = ParseOptionNode(optionBNode);

                XElement optionCNode = dialogNode.Element("OptionC");
                if (optionCNode != null) dialog.OptionC = ParseOptionNode(optionCNode);

                dialogDict[dialogIndex] = dialog;
            }
        }

        // Sort dictionary keys and add dialogs in correct order
        foreach (var key in dialogDict.Keys.OrderBy(k => k))
        {
            AdditionalDialogs.Add(dialogDict[key]);
        }

        Debug.Log($"Total Additional Dialogs Loaded: {AdditionalDialogs.Count}");
        AfterLoad?.Invoke();
    }


    private Question ParseQuestionNode(XElement questionNode)
    {
        if (questionNode == null) return null;

        return new Question
        {
            Image = questionNode.Element("PhotoPath")?.Value,
            Text = ExtractInnerText(questionNode),
            Translate = questionNode.Attribute("Translate")?.Value
        };
    }

    private Option ParseOptionNode(XElement optionNode)
    {
        if (optionNode == null) return null;

        return new Option
        {
            Image = optionNode.Element("PhotoPath")?.Value,
            Text = ExtractInnerText(optionNode),
            Translate = optionNode.Attribute("Translate")?.Value,
            Action = optionNode.Attribute("Action")?.Value
        };
    }

    private string ExtractInnerText(XElement element)
    {
        if (element == null) return string.Empty;

        return element.Nodes()
                     .OfType<XText>()
                     .FirstOrDefault()?.Value.Trim() ?? string.Empty;
    }
}
