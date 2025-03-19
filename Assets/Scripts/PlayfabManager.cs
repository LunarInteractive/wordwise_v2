using PlayFab;
using PlayFab.ClientModels;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PixelCrushers;
using UnityEngine.Events;
using Newtonsoft.Json;
using static PlayerProgressData;

public class PlayFabManager : MonoBehaviour
{
    public PlayerProgress playerProgress = new PlayerProgress();

    [Header("Login & Register Input")]
    public TextMeshProUGUI Pesan;
    public TMP_InputField emailInput;
    public TMP_InputField passwordInput;

    [Header("Username")]
    public GameObject UsernameDisplay;
    public TMP_InputField UsernameInput;
    public TextMeshProUGUI UsernamePlayer;

    [Header("Sukses Login & Register Event")]
    [Space]
    public UnityEvent SuksesLogin;
    public UnityEvent SuksesRegist;


    void Start()
    {

    }

    public void TombolRegister()
    {
        if (passwordInput.text.Length < 6)
        {
            Pesan.text = "Password Terlalu Pendek";
            return;
        }
        var request = new RegisterPlayFabUserRequest
        {
            Email = emailInput.text,
            Password = passwordInput.text,
            RequireBothUsernameAndEmail = false
        };
        PlayFabClientAPI.RegisterPlayFabUser(request, SuksesRegister, OnError);
    }
    void SuksesRegister(RegisterPlayFabUserResult result)
    {
        Pesan.text = "Sudah Register dan Login";
        var request = new UpdateUserTitleDisplayNameRequest
        {
            DisplayName = UsernameInput.text
        };
        PlayFabClientAPI.UpdateUserTitleDisplayName(request, OnDisplayNameUpdate, OnError);
        SuksesRegist?.Invoke();
    }
    void OnDisplayNameUpdate(UpdateUserTitleDisplayNameResult result)
    {
        Debug.Log("Sudah Update Display Name!");
        SuksesLogin?.Invoke();
        UsernamePlayer.text = result.DisplayName;
    }

    public void TombolReset()
    {
        var request = new SendAccountRecoveryEmailRequest
        {
            Email = emailInput.text,
            TitleId = "2C60B"
        };
        PlayFabClientAPI.SendAccountRecoveryEmail(request, ResetPassword, OnError);
    }
    void ResetPassword(SendAccountRecoveryEmailResult result)
    {
        Pesan.text = "Reset Password Telah dikirimkan ke Email anda";
    }

    public void TombolLogin()
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = emailInput.text,
            Password = passwordInput.text,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithEmailAddress(request, OnSuccessLogin, OnError);
    }
    void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            CustomId = "WordWise",
            CreateAccount = true,
            InfoRequestParameters = new GetPlayerCombinedInfoRequestParams
            {
                GetPlayerProfile = true
            }
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnSuccessLogin, OnError);
    }
    void OnSuccessLogin(LoginResult result)
    {
        Debug.Log("Sukses Login");
        string name = null;
        if (result.InfoResultPayload.PlayerProfile != null)
        {
            name = result.InfoResultPayload.PlayerProfile.DisplayName;
        }
        UsernamePlayer.text = name;
        SuksesLogin?.Invoke();
    }

    public void SaveDataToPlayFab()
    {
        string jsonData = JsonConvert.SerializeObject(playerProgress);

        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>
            {
                { "PlayerProgress", jsonData }
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataSaved, OnError);
    }
    public void LoadDataFromPlayFab()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataReceived, OnError);
    }

    void OnDataSaved(UpdateUserDataResult result)
    {
        Debug.Log("Player data saved successfully!");
    }
    void OnDataReceived(GetUserDataResult result)
    {
        if (result.Data != null && result.Data.ContainsKey("PlayerProgress"))
        {
            string jsonData = result.Data["PlayerProgress"].Value;
            playerProgress = JsonConvert.DeserializeObject<PlayerProgress>(jsonData);
            Debug.Log("Player data loaded successfully!");
        }
        else
        {
            Debug.Log("No player data found.");
        }
    }

    /*
    public LevelData GetLevelData(string chapterName, string levelName)
    {
        ChapterData chapter = playerProgress.Chapters.Find(c => c.ChapterName == chapterName);
        if (chapter != null)
        {
            return chapter.Levels.Find(l => l.LevelName == levelName);
        }
        return null;
    }
    */

    void OnError(PlayFabError error)
    {
        Pesan.text = error.ErrorMessage;
        Debug.Log(error.GenerateErrorReport());
    }
}