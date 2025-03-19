using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

/// <summary> Fajar
/// Pengaturan untuk mengubah password. User (se)harus(nya) input password lama.
/// Namun, karena batasan API, saat ini user bisa langsung update password tanpa 
/// autentikasi password lagi. Sistem pengiriman pesan ke database pakai PUT
/// disamakan dengan script UserUpdate.cs. API bisa cek di: https://documenter.getpostman.com/view/20942971/2sAYJ9BK8j#0d03c40f-454e-405d-b87b-c72e01a5be01
/// </summary>

public class ChangePass : MonoBehaviour
{
    [SerializeField] private FormChecker formChecker;
    
    //public void StartConnecting(string oldPass, string newPass, string repeatPass)
    //{
    //    StartCoroutine(GetChangePass(oldPass, newPass, repeatPass));
    //}

    public void StartConnecting(string newPass, string repeatPass)
    {
        StartCoroutine(GetChangePass(newPass, repeatPass));
    }

    //private IEnumerator GetChangePass(string oldPass, string newPass, string repeatPass)
    private IEnumerator GetChangePass(string newPass, string repeatPass)
    {
        UpdateUserPayload payload = new UpdateUserPayload();
        payload.password = newPass;
        payload.password_confirmation = repeatPass;
        

        string jsonData = JsonUtility.ToJson(payload);

        string user_id = UserDataSession.id;
        string url = "https://wordwise.id/api/v1/userspass/" + user_id + "?password=Zaqwesdxc123&password_confirmation=Zaqwesdxc123";

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
                Debug.Log("Update Password successful: " + regResponse.message);

                formChecker.ShowSubmit("Password updated successfully.");
            }
            else
            {
                Debug.LogError("Gagal parse JSON: " + responseText);
                formChecker.ShowWarning("There was a problem while retrieving data.");
            }
        }
    }

    /// LEGACY SNIPPET dari sistem lama sebelum pakai API untuk referensi penggunaan old password
    /// hapus jika perlu

    //            /* string money = (www.downloadHandler.text); //the echoed data


    //            if (money == "10")
    //            {
    //                formChecker.ShowWarning("ID not found.");
    //            }
    //            else if (money == "1")
    //            {
    //                formChecker.ShowWarning("Password incorrect. Please re-check your password.");
    //                formChecker.MarkField(formChecker.oldPassword);
    //            }
    //            else if (money == "Query Error")
    //            {
    //                formChecker.ShowWarning(money + ".");

    //            }
    //            else
    //            {
    //                Debug.Log("money = " + money);
    //                formChecker.ShowSubmit("Password change successful.");

    //            } */

}
