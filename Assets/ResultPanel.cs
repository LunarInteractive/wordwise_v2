using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class ResultPanel : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject resultPanel;

    public TMP_Text scoreText;
    public GameObject passText;
    public GameObject failText;

    public TMP_Text wordErrorText;
    public TMP_Text timerText;

    [SerializeField] private string linkA = "https://wordwise.id/api/v1/results?user_id=1&level_id=5&score=80/100";
    [SerializeField] private string linkB = "https://wordwise.id/api/v1/results?user_id=1&level_id=5&score=81/100";

    private string userID = UserDataSession.id;
    private string levelID = UserDataSession.levelID;

    public void ShowResult(float score, int wordErrors, float timeTaken)
    {
        resultPanel.SetActive(true);

        string userScore = score + "/100";

        string classCode = UserDataSession.classCode;

        if (!string.IsNullOrEmpty(UserDataSession.classCode))
        {
            SendScoreData(userID, levelID, userScore);
        }
        else
        {
            Debug.Log("user didn't have class, not sending data");
        }


        // Tampilkan score
        scoreText.text = $"{score:F1}";

        // Tampilkan word errors
        if (wordErrorText != null)
            wordErrorText.text = $"{wordErrors}";

        // Tampilkan time
        if (timerText != null)
            timerText.text = $"{timeTaken:F1} s";

        // Pass or Fail
        if (score >= 80)
        {
            passText.SetActive(true);
            failText.SetActive(false);
        }
        else
        {
            passText.SetActive(false);
            failText.SetActive(true);
        }
    }

    void Start()
    {
        resultPanel.SetActive(false);
        passText.SetActive(false);
        failText.SetActive(false);
    }

    public void SendScoreData(string userId, string levelId, string score)
    {
        StartCoroutine(PostScoreDataCoroutine(userId, levelId, score));
    }

    private IEnumerator PostScoreDataCoroutine(string userId, string levelId, string score)
    {
        string levelKey = "Level_" + levelId;
        string urlToUse;

        Debug.Log(levelKey);

        // Cek apakah PlayerPrefs untuk level ini sudah ada
        if (!PlayerPrefs.HasKey(levelKey))
        {
            // Belum ada -> pakai link A dan buat PlayerPrefs menandakan level ini sudah tersimpan
            PlayerPrefs.SetInt(levelKey, 1);
            PlayerPrefs.Save();
            urlToUse = linkA;
        }
        else
        {
            // Sudah ada -> pakai link B
            urlToUse = linkB;
        }

        // Buat form data untuk dikirim via POST
        WWWForm form = new WWWForm();
        form.AddField("user_id", userId);
        form.AddField("level_id", levelId);
        form.AddField("score", score);

        using (UnityWebRequest webRequest = UnityWebRequest.Post(urlToUse, form))
        {
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                // Jika terjadi error, tampilkan di console
                Debug.LogError($"Error posting score: {webRequest.error}");
            }
            else
            {
                // Jika berhasil, tampilkan respon (opsional)
                Debug.Log("Data posted successfully. Response: " + webRequest.downloadHandler.text);
            }
        }
    }
}
