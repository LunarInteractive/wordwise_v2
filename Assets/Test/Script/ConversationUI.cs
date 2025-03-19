using UnityEngine;
using UnityEngine.UI;

public class ConversationUI : MonoBehaviour
{
    [Header("Conversation XML")]
    public ConversationXML conversationXML;

    [Header("UI Elements")]
    public Text characterNameText;
    public Text questionText;
    public Button optionAButton;
    public Button optionBButton;
    public Button optionCButton;


    public void LoadMainDialog()
    {
        if (conversationXML.MainDialog != null)
        {
            LoadMainDialog(conversationXML.MainDialog);
        }
        else
        {
            Debug.LogError("MainDialog is null in ConversationLoader.");
        }
    }

    public void LoadDialog(int dialogIndex)
    {
        if (dialogIndex == 0)
        {
            LoadMainDialog();
        }
        else
        {
            int arrayIndex = dialogIndex - 1;

            if (arrayIndex < 0 || arrayIndex >= conversationXML.AdditionalDialogs.Count)
            {
                Debug.LogError("Invalid dialog index: " + arrayIndex);
                return;
            }

            Dialog dialog = conversationXML.AdditionalDialogs[arrayIndex];
            LoadDialog(dialog);
        }
    }

    private void LoadMainDialog(MainDialog dialog)
    {
        if (dialog != null)
        {
            // Populate character name and question
            characterNameText.text = dialog.Character;
            questionText.text = dialog.Question.Text;

            // Populate options
            optionAButton.GetComponentInChildren<Text>().text = dialog.OptionA.Text;
            optionBButton.GetComponentInChildren<Text>().text = dialog.OptionB.Text;
            optionCButton.GetComponentInChildren<Text>().text = dialog.OptionC.Text;

            // Clear previous button actions
            optionAButton.onClick.RemoveAllListeners();
            optionBButton.onClick.RemoveAllListeners();
            optionCButton.onClick.RemoveAllListeners();

            // Assign button actions
            optionAButton.onClick.AddListener(() => HandleOptionClick(dialog.OptionA.Action));
            optionBButton.onClick.AddListener(() => HandleOptionClick(dialog.OptionB.Action));
            optionCButton.onClick.AddListener(() => HandleOptionClick(dialog.OptionC.Action));
        }
        else
        {
            Debug.LogError("Dialog is null. Cannot populate UI.");
        }
    }
    private void LoadDialog(Dialog dialog)
    {
        if (dialog != null)
        {
            // Populate character name and question
            characterNameText.text = dialog.Character;
            questionText.text = dialog.Question.Text;

            // Populate options
            optionAButton.GetComponentInChildren<Text>().text = dialog.OptionA.Text;
            optionBButton.GetComponentInChildren<Text>().text = dialog.OptionB.Text;
            optionCButton.GetComponentInChildren<Text>().text = dialog.OptionC.Text;

            // Clear previous button actions
            optionAButton.onClick.RemoveAllListeners();
            optionBButton.onClick.RemoveAllListeners();
            optionCButton.onClick.RemoveAllListeners();

            // Assign button actions
            optionAButton.onClick.AddListener(() => HandleOptionClick(dialog.OptionA.Action));
            optionBButton.onClick.AddListener(() => HandleOptionClick(dialog.OptionB.Action));
            optionCButton.onClick.AddListener(() => HandleOptionClick(dialog.OptionC.Action));
        }
        else
        {
            Debug.LogError("Dialog is null. Cannot populate UI.");
        }
    }

    private void HandleOptionClick(string action)
    {
        Debug.Log("Option clicked. Action: " + action);

        // Handle the action logic here (e.g., load the next dialog, trigger an event, etc.)
        if (!string.IsNullOrEmpty(action) && action != "None")
        {
            int index = int.Parse(action);
            LoadDialog(index);
        }
        else
        {
            Debug.Log("No action associated with this option.");
        }
    }
}
