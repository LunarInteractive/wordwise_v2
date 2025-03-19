using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;

/// <summary> Fajar
/// Script penghubung ke database untuk fitur Edit Profile di Scene 'edit'. Saat ini hanya bisa 
/// edit Full Name dan School Name. Sebelumnya email bisa diedit tapi batasan database
/// tidak memungkinkannya. Kalau email bisa diedit lagi, mungkin tidak dari scene ini.
/// Agak jelimet karena ngirim pesan PUT dari Unity ini agak ribet (meski ada fungsi UnityWebRequest.Put())
/// Cek input dan pesan output di API: https://documenter.getpostman.com/view/20942971/2sAYJ9BK8j#0d03c40f-454e-405d-b87b-c72e01a5be01
/// </summary>

public class UserUpdate : MonoBehaviour
{
    [SerializeField] private FormChecker formChecker;

    //public void StartConnecting(string full_name, string email, string schoolName)
    //{
    //    StartCoroutine(UpdateUserData(full_name, email, schoolName));
    //}

    public void StartConnecting(string full_name, string schoolName)
    {
        StartCoroutine(UpdateUserData(full_name, schoolName));
    }

    //private IEnumerator UpdateUserData(string full_name, string email, string schoolName)
    private IEnumerator UpdateUserData(string full_name, string schoolName)
    {
        UpdateUserPayload payload = new UpdateUserPayload();
        payload.name = full_name;
        //payload.email = email;
        payload.school = schoolName;

        string jsonData = JsonUtility.ToJson(payload);

        string user_id = UserDataSession.id;
        string url = "https://wordwise.id/api/v1/users/" + user_id + "?name=admin1&school=Sekolah API 23";

        UnityWebRequest www = new UnityWebRequest(url, "PUT");

        byte[] rawJson = new UTF8Encoding().GetBytes(jsonData);

        www.uploadHandler = new UploadHandlerRaw(rawJson);
        www.downloadHandler = new DownloadHandlerBuffer();

        www.SetRequestHeader("Content-Type", "application/json");

        // (Opsional) Jika server butuh accept JSON
        // www.SetRequestHeader("Accept", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError ||
            www.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("PUT Error: " + www.error);
            formChecker.ShowWarning("There was a problem while connecting to server.");
        }
        else
        {
            string responseText = www.downloadHandler.text;
            Debug.Log("Server response: " + responseText);

            RegistrationResponse regResponse = JsonUtility.FromJson<RegistrationResponse>(responseText);

            if (regResponse != null && regResponse.data != null)
            {
                
                Debug.Log("Update user successful: " + regResponse.message);

                UserDataSession.fullName = regResponse.data.name;
               // UserDataSession.email = regResponse.data.email;
                UserDataSession.schoolName = regResponse.data.school;

                formChecker.ShowSubmit("Update successful.");
            }
            else
            {
                Debug.LogError("Gagal parse JSON: " + responseText);
                formChecker.ShowWarning("There was a problem while retrieving user data.");
            }
        }
    }
}
