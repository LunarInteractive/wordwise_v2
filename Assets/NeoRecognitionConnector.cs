using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using PixelCrushers.DialogueSystem;
using System;
using System.Linq;
using Unity.VisualScripting;

public class NeoRecognitionConnector : MonoBehaviour
{
    public string log;
    public string partial_result;
    private string _result;
    public string result;

    public DialogueSystemController dialogueSystemController;
    public StandardDialogueUI standardDialogueUI;

    public AudioRecorder audioRecorder;

    private List<string> m_options = new List<string>();
    private Response[] responses;

    // Start is called before the first frame update
    private void OnEnable()
    {
        standardDialogueUI = dialogueSystemController.dialogueUI as StandardDialogueUI;
        standardDialogueUI.showResponseOptionsEvent.AddListener(OnShowResponseOptions);
        _result = result;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (result != _result)
        {
            checkResult();
            _result = result;
        }
    }

    public void set_log(string log)
    {
        this.log = log;
    }

    public void set_partial_result(string partial_result)
    {
        this.partial_result = partial_result;
    }

    public void set_result(string result)
    {
        this.result = result;
    }

    public void OnShowResponseOptions(Subtitle subtitle, Response[] responses, float timeout)
    {
        this.responses = responses;
        audioRecorder.OnStart();
    }

    void checkResult()
    {
        foreach (var response in responses)
        {
            string temp_string = new string(response.formattedText.text.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)).ToArray()).ToLower();
            int scoreError = Compute(result, temp_string);
            Debug.Log(scoreError);
            if (scoreError < 80 / 100f * result.Length)
            {
                standardDialogueUI.OnClick(response);
                return;
            }
        }
    }
    public int Compute(string s, string t)
    {
        if (string.IsNullOrEmpty(s))
        {
            if (string.IsNullOrEmpty(t))
                return 0;
            return t.Length;
        }

        if (string.IsNullOrEmpty(t))
        {
            return s.Length;
        }

        int n = s.Length;
        int m = t.Length;
        int[,] d = new int[n + 1, m + 1];

        for (int i = 0; i <= n; d[i, 0] = i++) ;
        for (int j = 1; j <= m; d[0, j] = j++) ;

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                int min1 = d[i - 1, j] + 1;
                int min2 = d[i, j - 1] + 1;
                int min3 = d[i - 1, j - 1] + cost;
                d[i, j] = Math.Min(Math.Min(min1, min2), min3);
            }
        }
        return d[n, m];
    }
}
