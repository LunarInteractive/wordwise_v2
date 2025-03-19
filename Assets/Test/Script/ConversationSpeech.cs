using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Crosstales.RTVoice;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.Networking;
using System.Linq;

public class ConversationSpeech : MonoBehaviour
{
    [Header("Conversation XML")]
    public ConversationXML conversationXML;

    [Header("AudioRecorder Script")]
    public AudioRecorder audioRecorder;

    [Header("UI Elements")]
    public TMP_Text characterNameText;
    public TMP_Text questionText;
    public Image imageplaceholder;
    public Button optionAButton;
    public Button optionBButton;
    public Button optionCButton;
    public Button pushToTalkButton;

    public GameObject panelBlur;

    [Header("Speech Elements")]
    public TMP_InputField logText;
    public TMP_InputField detectedText;
    public int SimilarityIndex = 80;

    [Header("Speech Checker UI")]
    public GameObject panelSpeechChecker;
    public TMP_Text similarityText;
    public TMP_Text recognizedColoredText;
    public TMP_Text targetColoredText;

    [Header("Text to Speech")]
    //public Speaker speaker;
    public PlayVoice speaker;

    [Header("Level Name")]
    public TMP_Text levelText;


    private int similarityScore;
    private float totalSimilarityScore = 0;
    private int similarityCount = 0;
    public ResultPanel ResultPanel;

    private int wordErrors = 0;

    private float startTime;
    private bool isDialogActive = false;

    private System.Action nextDialogAction;


    private void Start()
    {
        SetupPushToTalk();

        startTime = Time.time;
        isDialogActive = true;

        panelBlur.GetComponent<Animator>().SetBool("Blur", false);

        string levelName = UserDataSession.levelName;
        levelText.text = levelName;

        //Legacy jika ingin mencoba RT voice manual di scene gameplay
        //speaker.Speak(speechTextTest);
        //speaker.Speak(questionText.text, null, speaker.Voices[1], true, Random.Range(0.9f, 1.5f), 2.0f);
        //speaker.OnVoicesReady += LoadMainDialog;
        //speaker.OnVoicesReady += CheckAndShowVoices;

        //LoadMainDialog();

        //print(speaker.areVoicesReady);
    }

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
        // Jika -1 => Load main
        if (dialogIndex == -1)
        {
            LoadMainDialog();
            return;
        }

        // Jika >= 0 => AdditionalDialogs
        if (dialogIndex < 0 || dialogIndex >= conversationXML.AdditionalDialogs.Count)
        {
            Debug.LogError("Invalid dialog index: " + dialogIndex);
            return;
        }

        Dialog dialog = conversationXML.AdditionalDialogs[dialogIndex];
        LoadDialog(dialog);
    }


    private void LoadMainDialog(MainDialog dialog)
    {
        if (dialog != null)
        {
            // Populate character name and question
            characterNameText.text = dialog.Character;
            questionText.text = dialog.Question.Text;

            //Menjalankan voice TTS setiap kali dialog dipangggil
            speaker.PlaySpeech(questionText.text);

            // Populate options
            optionAButton.GetComponentInChildren<TMP_Text>().text = dialog.OptionA.Text;
            optionBButton.GetComponentInChildren<TMP_Text>().text = dialog.OptionB.Text;
            optionCButton.GetComponentInChildren<TMP_Text>().text = dialog.OptionC.Text;

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

            // Load Image (if available)
            string imageUrl = dialog.Question?.Image;
            Debug.Log(imageUrl);
            if (!string.IsNullOrEmpty(imageUrl))
            {
                imageplaceholder.gameObject.SetActive(true);
                StartCoroutine(DownloadAndSetImage(imageUrl));
            }
            else
            {
                imageplaceholder.gameObject.SetActive(false);
                Debug.Log("No image found");
            }

            // Play TTS for the question
            if (speaker != null)
            {
                speaker.PlaySpeech(questionText.text);
            }
            else
            {
                Debug.Log("Speaker has not been set");
            }

            // Handle Option A
            if (dialog.OptionA != null)
            {
                optionAButton.gameObject.SetActive(true);
                optionAButton.GetComponentInChildren<TMP_Text>().text = dialog.OptionA.Text;
                optionAButton.onClick.RemoveAllListeners();
                optionAButton.onClick.AddListener(() => HandleOptionClick(dialog.OptionA.Action));
            }
            else
            {
                optionAButton.gameObject.SetActive(false);
            }

            // Handle Option B
            if (dialog.OptionB != null)
            {
                optionBButton.gameObject.SetActive(true);
                optionBButton.GetComponentInChildren<TMP_Text>().text = dialog.OptionB.Text;
                optionBButton.onClick.RemoveAllListeners();
                optionBButton.onClick.AddListener(() => HandleOptionClick(dialog.OptionB.Action));
            }
            else
            {
                optionBButton.gameObject.SetActive(false);
            }

            // Handle Option C
            if (dialog.OptionC != null)
            {
                optionCButton.gameObject.SetActive(true);
                optionCButton.GetComponentInChildren<TMP_Text>().text = dialog.OptionC.Text;
                optionCButton.onClick.RemoveAllListeners();
                optionCButton.onClick.AddListener(() => HandleOptionClick(dialog.OptionC.Action));
            }
            else
            {
                optionCButton.gameObject.SetActive(false);  // **Explicitly deactivate button**
            }
        }
        else
        {
            Debug.LogError("Dialog is null. Cannot populate UI.");
        }
    }



    private async void EndDialog()
    {
        panelSpeechChecker.SetActive(false);

        float averageScore = (similarityCount > 0) ? (totalSimilarityScore / similarityCount) : 0f;

        // Berhenti timer
        float timeTaken = 0f;
        if (isDialogActive)
        {
            timeTaken = Time.time - startTime;
            isDialogActive = false;
        }

        ResultPanel.ShowResult(averageScore, wordErrors, timeTaken);

        // Jalankan Coroutine yang mengatur delay sebelum menampilkan Blur
        StartCoroutine(ShowBlurWithDelay(0.2f)); // Contoh 1 detik
    }

    private IEnumerator ShowBlurWithDelay(float delay)
    {
        // Tunggu selama 'delay' detik
        yield return new WaitForSeconds(delay);

        // Setelah menunggu, baru jalankan animasi blur
        panelBlur.GetComponent<Animator>().SetBool("Blur", true);
    }


    private void HandleOptionClick(string action)
    {
        Debug.Log("Option clicked. Action: " + action);

        if (!string.IsNullOrEmpty(action) && int.TryParse(action, out int index))
        {
            if (index == -1)
            {
                EndDialog();
            }
            else
            {
                LoadDialog(index);
            }
        }
        else
        {
            Debug.Log("No action associated with this option.");
        }
    }

    public void LoadBySimilarity()
    {
        if (detectedText == null || string.IsNullOrEmpty(detectedText.text))
        {
            Debug.LogError("DetectedText is null or empty.");
            return;
        }

        string detected = detectedText.text.ToLower();
        string optionA = optionAButton.GetComponentInChildren<TMP_Text>().text.ToLower();
        string optionB = optionBButton.GetComponentInChildren<TMP_Text>().text.ToLower();
        string optionC = optionCButton.GetComponentInChildren<TMP_Text>().text.ToLower();

        int simA = CalculateSimilarity(detected, optionA);
        int simB = CalculateSimilarity(detected, optionB);
        int simC = CalculateSimilarity(detected, optionC);

        if (simA >= SimilarityIndex && simA >= simB && simA >= simC)
        {
            similarityScore = simA;

            nextDialogAction = () => optionAButton.onClick.Invoke();

            int currentWordError = GetWordErrorsAlignment(detected, optionA);
            wordErrors += currentWordError;

            //Set tombol TTS agar mengeluarkan suara sesuai opsi
            speaker.SetTTSButton(optionA);

            ShowSpeechCheckerPanel(similarityScore, detected, optionA);
        }

        else if (simB >= SimilarityIndex && simB >= simA && simB >= simC)
        {
            similarityScore = simB;

            nextDialogAction = () => optionBButton.onClick.Invoke();

            int currentWordError = GetWordErrorsAlignment(detected, optionB);
            wordErrors += currentWordError;

            speaker.SetTTSButton(optionB);

            ShowSpeechCheckerPanel(similarityScore, detected, optionB);
        }
        else if (simC >= SimilarityIndex && simC >= simA && simC >= simB)
        {
            similarityScore = simC;

            nextDialogAction = () => optionCButton.onClick.Invoke();

            int currentWordError = GetWordErrorsAlignment(detected, optionC);
            wordErrors += currentWordError;

            speaker.SetTTSButton(optionC);

            ShowSpeechCheckerPanel(similarityScore, detected, optionC);
        }
        else
        {
            Debug.Log("No options match detectedText with sufficient similarity.");

            //Sembunyikan tombol TTS kalau tidak ada opsi yang sesuai
            speaker.HideTTSButton();
            return;
        }

            totalSimilarityScore += similarityScore;
            similarityCount++;
        }


    private IEnumerator DownloadAndSetImage(string imageUrl)
    {
        // Buat request untuk mendownload texture dari URL
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);

        // Mulai download
        yield return request.SendWebRequest();

        // Cek apakah terjadi error
        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Gagal mendownload gambar: {request.error}");
        }
        else
        {
            // Jika berhasil, dapatkan Texture2D
            Texture2D downloadedTexture = DownloadHandlerTexture.GetContent(request);

            // Buat sprite dari Texture2D
            Sprite newSprite = Sprite.Create(
                                downloadedTexture,
                                new Rect(0, 0, downloadedTexture.width, downloadedTexture.height),
                                Vector2.zero
                                );

            // Tampilkan sprite pada UI Image
            imageplaceholder.sprite = newSprite;
        }
    }

    public void OnContinueClicked()
        {

            // Panggil action yang sudah disimpan 
            if (nextDialogAction != null)
            {
                nextDialogAction.Invoke();
                nextDialogAction = null; // opsional, untuk “mengosongkan” agar tak dipanggil dua kali
            }

            // Tutup panel
            panelBlur.GetComponent<Animator>().SetBool("Blur", false);

            panelSpeechChecker.SetActive(false);
        }
        public void OnRestartClicked()
        {
        panelBlur.GetComponent<Animator>().SetBool("Blur", false);
        panelSpeechChecker.SetActive(false);
        }


        public void ShowSpeechCheckerPanel(float score, string recognized, string target)
        {



            if (panelSpeechChecker != null && similarityText != null)
            {
                //Menambahkan panel blur di belakang panel SpeechChecker
                panelBlur.GetComponent<Animator>().SetBool("Blur", true);
                panelSpeechChecker.SetActive(true);
                similarityText.text = $"Similarity Score: {score}%";

                // Warna hanya recognized text
                if (recognizedColoredText != null)
                    recognizedColoredText.text = "recognized : " + BuildColoredRecognizedString(recognized, target);

                // Target text apa adanya (tidak diwarnai)
                if (targetColoredText != null)
                    targetColoredText.text = "Target : " + target;
            }
            else
            {
                Debug.LogError("panelSpeechChecker atau similarityText belum di-assign.");
            }
        }


    private string BuildColoredRecognizedString(string recognized, string target)
    {
        // Remove symbols for comparison but keep them in the original text
        string cleanRecognized = RemoveSymbols(recognized);
        string cleanTarget = RemoveSymbols(target);

        string[] recognizedWords = cleanRecognized.Split(
            new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
        string[] targetWords = cleanTarget.Split(
            new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

        int m = recognizedWords.Length;
        int n = targetWords.Length;

        int[,] cost = new int[m + 1, n + 1];
        int[,] from = new int[m + 1, n + 1];

        for (int i = 0; i <= m; i++) { cost[i, 0] = i; from[i, 0] = 1; }
        for (int j = 0; j <= n; j++) { cost[0, j] = j; from[0, j] = 2; }
        from[0, 0] = 0;

        for (int i = 1; i <= m; i++)
        {
            for (int j = 1; j <= n; j++)
            {
                if (recognizedWords[i - 1].Equals(targetWords[j - 1], System.StringComparison.OrdinalIgnoreCase))
                {
                    cost[i, j] = cost[i - 1, j - 1];
                    from[i, j] = 0;
                }
                else
                {
                    int substCost = cost[i - 1, j - 1] + 1;
                    int deleteCost = cost[i - 1, j] + 1;
                    int insertCost = cost[i, j - 1] + 1;

                    int minCost = substCost;
                    int dir = 0;
                    if (deleteCost < minCost) { minCost = deleteCost; dir = 1; }
                    if (insertCost < minCost) { minCost = insertCost; dir = 2; }

                    cost[i, j] = minCost;
                    from[i, j] = dir;
                }
            }
        }

        int a = m, b = n;
        List<string> aligned = new List<string>();

        while (a > 0 || b > 0)
        {
            if (a > 0 && b > 0 && from[a, b] == 0)
            {
                if (recognizedWords[a - 1].Equals(targetWords[b - 1], System.StringComparison.OrdinalIgnoreCase))
                {
                    aligned.Add(recognizedWords[a - 1]);
                }
                else
                {
                    aligned.Add($"<color=red>{recognizedWords[a - 1]}</color>");
                }
                a--; b--;
            }
            else if (a > 0 && from[a, b] == 1)
            {
                aligned.Add($"<color=red>{recognizedWords[a - 1]}</color>");
                a--;
            }
            else
            {
                b--;
            }
        }

        aligned.Reverse();
        return string.Join(" ", aligned);
    }

    /// <summary>
    /// Removes symbols from a string for comparison purposes.
    /// </summary>
    private string RemoveSymbols(string input)
    {
        return new string(input.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)).ToArray());
    }


    private int GetWordErrorsAlignment(string recognized, string target)
    {
        recognized = CleanString(recognized);
        target = CleanString(target);

        string[] rWords = recognized.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
        string[] tWords = target.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

        int m = rWords.Length;
        int n = tWords.Length;

        int[,] cost = new int[m + 1, n + 1];

        for (int i = 0; i <= m; i++) cost[i, 0] = i;
        for (int j = 0; j <= n; j++) cost[0, j] = j;

        for (int i = 1; i <= m; i++)
        {
            for (int j = 1; j <= n; j++)
            {
                int matchCost = (rWords[i - 1].Equals(tWords[j - 1], System.StringComparison.OrdinalIgnoreCase)) ? 0 : 1;
                int substitution = cost[i - 1, j - 1] + matchCost;
                int deletion = cost[i - 1, j] + 1;
                int insertion = cost[i, j - 1] + 1;

                cost[i, j] = Mathf.Min(substitution, Mathf.Min(deletion, insertion));
            }
        }

        return cost[m, n];
    }



    public void SetupPushToTalk()
        {
            EventTrigger trigger = pushToTalkButton.gameObject.AddComponent<EventTrigger>();

            EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
            pointerDownEntry.eventID = EventTriggerType.PointerDown;
            pointerDownEntry.callback.AddListener((data) => { OnPushToTalkPressed(); });
            trigger.triggers.Add(pointerDownEntry);

            EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
            pointerUpEntry.eventID = EventTriggerType.PointerUp;
            pointerUpEntry.callback.AddListener((data) => { OnPushToTalkReleased(); });
            trigger.triggers.Add(pointerUpEntry);
        }

        private void OnPushToTalkPressed()
        {
            if (audioRecorder != null)
            {
                audioRecorder.OnStart();
                Debug.Log("Audio recording started.");
            }
            else
            {
                Debug.LogError("AudioRecorder is not assigned.");
            }
        }

        private void OnPushToTalkReleased()
        {
            if (audioRecorder != null)
            {
                audioRecorder.OnStop();
                LoadBySimilarity();
                Debug.Log("Audio recording stopped.");
            }
            else
            {
                Debug.LogError("AudioRecorder is not assigned.");
            }
        }

    private string CleanString(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;

        return new string(input.Trim().Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)).ToArray());
    }

    /// <summary>
    /// Menghitung tingkat kesamaan antara dua string.
    /// </summary>
    private int CalculateSimilarity(string source, string target)
    {
        source = CleanString(source);
        target = CleanString(target);

        if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
            return 0;

        int[,] dp = new int[source.Length + 1, target.Length + 1];

        for (int i = 0; i <= source.Length; i++)
            dp[i, 0] = i;
        for (int j = 0; j <= target.Length; j++)
            dp[0, j] = j;

        for (int i = 1; i <= source.Length; i++)
        {
            for (int j = 1; j <= target.Length; j++)
            {
                if (source[i - 1] == target[j - 1])
                    dp[i, j] = dp[i - 1, j - 1];
                else
                    dp[i, j] = Mathf.Min(dp[i - 1, j], Mathf.Min(dp[i, j - 1], dp[i - 1, j - 1])) + 1;
            }
        }

        int editDistance = dp[source.Length, target.Length];
        int maxLen = Mathf.Max(source.Length, target.Length);
        return (int)((1.0f - (float)editDistance / maxLen) * 100);
    }
}
