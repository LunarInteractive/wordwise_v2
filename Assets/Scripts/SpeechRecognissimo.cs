using System.Collections.Generic;
using UnityEngine;
using Recognissimo.Components;
using PixelCrushers.DialogueSystem;
using System;
using System.Linq;

public class SpeechRecognissimo : StandardDialogueUI
{
    public float speechRecognitionTimeout = 5;

    private VoiceControl m_voiceControl;
    private Response[] m_responses;
    private Response m_timeoutResponse;
    private float m_timeLeft;
    private bool m_isWaitingForResponse;

    public override void ShowResponses(Subtitle subtitle, Response[] responses, float timeout)
    {
        // Remember the responses to check when we recognize a keyword:
        if (responses.Length > 1)
        {
            // If we have more than one, assume the last response is the "I don't understand"/timeout special response.
            // Record it and remove it from the array of regular responses:
            m_timeoutResponse = responses[responses.Length - 1];
            var responsesExceptLast = new Response[responses.Length - 1];
            for (int i = 0; i < responses.Length - 1; i++)
            {
                responsesExceptLast[i] = responses[i];
            }
            responses = responsesExceptLast;
        }
        else
        {
            m_timeoutResponse = null;
        }
        m_responses = responses;
        m_timeLeft = speechRecognitionTimeout;
        m_isWaitingForResponse = true;

        // Show the responses:
        base.ShowResponses(subtitle, responses, timeout);

        // Identify the keywords to recognize:
        // (Each response's menu text can have keywords separated by pipe characters.)
        var allKeywords = new List<string>();
        foreach (var response in responses)
        {
            var responseKeywords = response.formattedText.text.Split('|');
            allKeywords.AddRange(responseKeywords);
        }

        // Set up the voice control recognizer:
        m_voiceControl = GetComponent<VoiceControl>();
        if (m_voiceControl == null)
        {

            m_voiceControl = gameObject.AddComponent<VoiceControl>();

        }
        m_voiceControl.AsapMode = true;

        foreach (var keyword in allKeywords)
        {
            string m_keyword = new string(keyword.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)).ToArray()).ToLower();
            Debug.Log(m_keyword);
            m_voiceControl.Commands.Add(new VoiceControlCommand(keyword, () => OnPhraseRecognized(keyword)));
        }

        m_voiceControl.InitializationFailed.AddListener(e => Debug.LogError("Voice Control initialization failed: " + e.Message));
        m_voiceControl.StartProcessing();
    }

    public override void HideResponses()
    {
        base.HideResponses();

        // Stop speech recognition when we hide the menu:
        if (m_voiceControl != null)
        {
            m_voiceControl.StopProcessing();
            m_voiceControl.Commands.Clear();
        }
        m_timeoutResponse = null;
        m_isWaitingForResponse = false;
    }

    public override void Update()
    {
        base.Update();

        // Update speech recognition timer:
        if (m_isWaitingForResponse && m_timeoutResponse != null)
        {
            m_timeLeft -= Time.deltaTime;
            if (m_timeLeft <= 0)
            {
                // If time runs out, use the timeout response:
                OnClick(m_timeoutResponse);
                m_isWaitingForResponse = false;
            }
        }
    }

    private void OnPhraseRecognized(string phrase)
    {
        Debug.Log("Recognized: '" + phrase + "'");
        // Match the user's spoken phrase with one of the responses:
        foreach (var response in m_responses)
        {
            var responseKeywords = response.destinationEntry.DialogueText.Split('|');
            foreach (var responseKeyword in responseKeywords)
            {
                if (string.Equals(phrase, responseKeyword, System.StringComparison.OrdinalIgnoreCase))
                {
                    OnClick(response);
                    m_isWaitingForResponse = false;
                    return; // Exit the loop once the response is found and clicked.
                }
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