using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class TestingPostgrab : MonoBehaviour
{
    public void Start()
    {
        StartCoroutine(SendRequest());
    }

    public IEnumerator SendRequest()
    {
        string user_id = UserDataSession.id;
        // Contoh mengirim PUT, bisa juga POST / GET tergantung kebutuhan
        string url = "https://example.com/api/v1/users/"+user_id+"?name=testapi23"; // misal
        UnityWebRequest www = new UnityWebRequest(url, "PUT");

        // (Jika perlu kirimkan body JSON di uploadHandler, dsb.)

        www.downloadHandler = new DownloadHandlerBuffer();
        yield return www.SendWebRequest();

        // 1. Cek status code
        long statusCode = www.responseCode;
        Debug.Log("HTTP status code: " + statusCode);

        // 2. Cek body response
        string responseBody = www.downloadHandler.text;
        Debug.Log("Raw response body: " + responseBody);

        // 3. Pengecekan error
        if (www.result == UnityWebRequest.Result.ConnectionError ||
            www.result == UnityWebRequest.Result.ProtocolError)
        {
            // Misalnya untuk status 400 Bad Request, 422 Unprocessable Entity, dsb.
            Debug.LogError("Request Error: " + www.error);
            // Mungkin server mengirim JSON/HTML di body
            // Anda bisa parse body response untuk pesan error spesifik
        }
        else
        {
            // Jika status code 200 OK atau 201 Created, dsb.
            Debug.Log("Request Sukses!");
        }
    }
}
