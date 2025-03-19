using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Recognissimo.Components;
using UnityEngine.UI;
using System.Linq;
using PixelCrushers.DialogueSystem;
using PlayFab.Internal;

public class RecognitionConnector : MonoBehaviour
{
    public string _string;
    private string _string_old;

    public int tolerance = 80;

    public DialogueSystemController dialogueSystemController;
    private StandardDialogueUI standardDialogueUI;

    private const string LoadingMessage = "Loading...";

    [SerializeField]

    private Recognissimo.Components.SpeechRecognizer recognizer;

    [SerializeField]
    private StreamingAssetsLanguageModelProvider languageModelProvider;

    private readonly RecognizedText _recognizedText = new();

    private List<SystemLanguage> _availableLanguages;

    private List<string> m_options = new List<string>();

    private Response[] responses;

    private bool m_isWaitingForResponse;

    private AudioClip micClip;
    private bool isRecognizing = false;

    public float soundThreshold = 0.1f;
    public int sampleWindow = 128;

    [SerializeField]
    private Image uiImage;

    [SerializeField]
    private Sprite originalSprite;

    [SerializeField]
    private Sprite recognizedSprite;

    private void OnEnable()
    {
        standardDialogueUI = dialogueSystemController.dialogueUI as StandardDialogueUI;
        standardDialogueUI.showResponseOptionsEvent.AddListener(OnShowResponseOptions);

        // Make sure language models exist.
        if (languageModelProvider.languageModels.Count == 0)
        {
            throw new InvalidOperationException("No language models.");
        }

        // Set default language.
        languageModelProvider.language = SystemLanguage.English;

        // Initialize microphone
        InitializeMicrophone();

        // Initialize UI.
        UpdateStatus("");

        // Bind recognizer to event handlers.
        recognizer.Started.AddListener(() =>
        {
            _recognizedText.Clear();
            UpdateStatus("");
        });

        recognizer.Finished.AddListener(() => Debug.Log("Finished"));

        recognizer.PartialResultReady.AddListener(OnPartialResult);
        recognizer.ResultReady.AddListener(OnResult);
    }

    private void InitializeMicrophone()
    {
        if (Microphone.devices.Length > 0)
        {
            micClip = Microphone.Start(null, true, 10, 44100);
        }
        else
        {
            Debug.LogError("No microphone found!");
        }
    }

    private void UpdateStatus(string text)
    {
        _string = text;
        if (_string.Length > 1)
        {
            if (Char.IsWhiteSpace(_string[0]))
            {
                _string = _string.Substring(1);
            }
            if (Char.IsWhiteSpace(_string[_string.Length - 1]))
            {
                _string = _string.Substring(0, _string.Length - 1);
            }
        }

    }

    private void Update()
    {
        float volumeLevel = GetMaxVolume();

        if (volumeLevel > soundThreshold && !isRecognizing)
        {
            StartRecognition();

        }
    }

    private float GetMaxVolume()
    {
        float maxVolume = 0f;
        float[] waveData = new float[sampleWindow];
        int micPosition = Microphone.GetPosition(null) - (sampleWindow + 1);

        if (micPosition < 0)
            return 0;

        micClip.GetData(waveData, micPosition);

        for (int i = 0; i < sampleWindow; i++)
        {
            float wavePeak = waveData[i] * waveData[i];
            if (maxVolume < wavePeak)
            {
                maxVolume = wavePeak;
            }
        }

        return maxVolume;
    }

    private void StartRecognition()
    {
        UpdateStatus(LoadingMessage);
        recognizer.StartProcessing();
        isRecognizing = true;
    }

    private void OnPartialResult(PartialResult partial)
    {
        _recognizedText.Append(partial);
        UpdateStatus(_recognizedText.CurrentText);
        ChangeSpriteToRecognized();
    }

    private void OnResult(Result result)
    {
        _recognizedText.Append(result);
        UpdateStatus(_recognizedText.CurrentText);
        foreach (var response in responses)
        {
            string temp_string = new string(response.formattedText.text.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)).ToArray()).ToLower();
            int scoreError = Compute(_string, temp_string);
            Debug.Log(scoreError);
            if (scoreError < tolerance / 100f * _string.Length)
            {
                standardDialogueUI.OnClick(response);
                break;
            }
        }
        StartCoroutine(ResetRecognitionAfterDelay(0.5f));
    }

    private IEnumerator ResetRecognitionAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        _recognizedText.Clear();
        UpdateStatus("");
        isRecognizing = false;
        ResetSprite();
    }

    private void OnShowResponseOptions(Subtitle subtitle, Response[] responses, float timeout)
    {
        this.responses = responses;
    }

    private class RecognizedText
    {
        private string _changingText;
        private string _stableText;

        public string CurrentText => $"{_stableText} {_changingText}";

        public void Append(Result result)
        {
            _changingText = "";
            _stableText = $"{_stableText} {result.text}";
        }

        public void Append(PartialResult partialResult)
        {
            _changingText = partialResult.partial;
        }

        public void Clear()
        {
            _changingText = "";
            _stableText = "";
        }
    }

    void Start()
    {
        _string_old = _string;
        InitializeMicrophone();
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

    private void ChangeSpriteToRecognized()
    {
        if (uiImage != null && recognizedSprite != null)
        {
            uiImage.sprite = recognizedSprite;
            Debug.Log("record voice");
        }
    }

    private void ResetSprite()
    {
        if (uiImage != null && originalSprite != null)
        {
            uiImage.sprite = originalSprite;
            Debug.Log("unrecord voice");
        }
    }
}