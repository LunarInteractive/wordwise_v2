using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

// Script ini bertanggung jawab untuk login otomatis
// dengan cara GET data user berdasarkan user_id yang tersimpan.

/// <summary> Fajar
/// Tambahan, beberapa modifikasi diperlukan untuk memastikan:
/// - Autologin juga mengambil data class_id, class_code (token),dan class_name dari database
/// - Autologin dapat dipanggil dari scene profile dan main_menu setelah proses apapun
/// (untuk pemanggilan dari scene 'main_menu', cek SceneManagerMain.cs)
/// API: https://documenter.getpostman.com/view/20942971/2sAYJ9BK8j#0d03c40f-454e-405d-b87b-c72e01a5be01
/// </summary>
public class AutoLogin : MonoBehaviour
{
    private void Start()
    {
        Debug.Log("Signed in?: " + UserDataSession.OnSession);
        if (UserDataSession.OnSession)
        {


            // Periksa jika PlayerPrefs belum memiliki class_code dan class_id 
            if ((!PlayerPrefs.HasKey("class_code")) && (!PlayerPrefs.HasKey("class_id")))
            {
                string savedUserId = PlayerPrefs.GetString("user_id");
                StartCoroutine(CheckClass(savedUserId));
            }
            else
            {
                Debug.Log("user telah login, tidak perlu login kembali");

                // Jika autologin dipanggil di scene 'main_menu'
                // Setup scene untuk menampilkan nama di tombol profile
                // Harus dipanggil setelah class_code dicek
            }
        }
        else
        {
            // Periksa apakah user_id pernah disimpan di PlayerPrefs
            if (PlayerPrefs.HasKey("user_id"))
            {
                // Ambil user_id yang disimpan
                string savedUserId = PlayerPrefs.GetString("user_id");
                Debug.Log("UserId tersimpan, langsung login otomatis. UserId = " + savedUserId);

                // Tampilkan panel 'proses login'
                //processPanel.SetContent("Trying to Login ...");
                //processPanel.gameObject.SetActive(true);

                // Mulai coroutine GET data user
                StartCoroutine(LogIn(savedUserId));
            }
            else
            {
                // Jika tidak ada user_id, user harus login manual
                Debug.Log("Belum ada user_id, user perlu login manual.");
            }
        }

    }

    // Coroutine untuk GET data user dari endpoint https://wordwise.id/api/v1/users/{UserID}
    private IEnumerator LogIn(string userID)
    {
        string url = "https://wordwise.id/api/v1/users/" + userID;
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error GET: " + www.error);
            }
            else
            {
                string responseText = www.downloadHandler.text;
                Debug.Log("Server response: " + responseText);

                RegistrationResponse regResponse = JsonUtility.FromJson<RegistrationResponse>(responseText);

                if (regResponse != null)
                {
                    /// Jika diperlukan menampilkan panel setelah login berhasil

                    //if (warningPanel != null)
                    //{
                    //  warningPanel.SetContent("Login successful. Welcome " + regResponse.data.name);
                    //  warningPanel.gameObject.SetActive(true);
                    //}

                    UserDataSession.id = regResponse.data.id;
                    UserDataSession.fullName = regResponse.data.name;
                    UserDataSession.email = regResponse.data.email;
                    UserDataSession.role = regResponse.data.role;
                    UserDataSession.schoolName = regResponse.data.school;

                    // Jika autologin dilakukan di scene 'profile' tampilkan data user

                    /// Sebelumnya, menampilkan class_code/token di Scene 'profile' 
                    /// hanya mengambil dari PlayerPrefs. 
                    /// Sekarang, data diambil lagi 
                    /// dari database dan menampilkan class_name.
                    /// Sistem lama masih disimpan kalau sistem sekarang bermasalah. 
                    
                    StartCoroutine(CheckClass(userID));

                    //string savedClassCode = PlayerPrefs.GetString("class_code");
                    //string savedClassID = PlayerPrefs.GetString("class_id");
                    //UserDataSession.classID = savedClassID;

                    //if (savedClassCode != null && savedClassCode != "")
                    //{
                    //    UserDataSession.classCode = savedClassCode;
                    //    profileFunction.AddClass(UserDataSession.classCode);
                    //    Debug.Log("masuk ke kelas dengan kode: " + savedClassCode);
                    //}
                    //else
                    //{
                    //    Debug.Log("belum ada kode kelas yang disimpan");
                    //}
                }
                else
                {
                    Debug.LogError("Gagal parse JSON: " + responseText);
                    //warningPanel.SetContent("Something went wrong. Invalid JSON format.");
                }
            }
        }
    }

    /// <summary> Fajar
    /// Mengakses database joinedclass dengan input user_id (userID), mengambil data class_id, token, class_name.
    /// Karena di joined class, user_id bisa terhubung dengan banyak id di table tsb., maka diambil
    /// entry paling akhir (id paling besar) yang mengandung user_id tsb.
    /// Data id-id dari tabel disimpan di class array UserJoinedClassesResponse[]. Selengkapnya, cek API.
    /// Hasil output berupa data className langsung ditampilkan di panel dan profile.
    /// </summary>

    private IEnumerator CheckClass(string userID)
    {
        string url = "https://wordwise.id/api/v1/classes/joinedclass/" + userID;
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error GET: " + www.error);
            }
            else
            {
                string responseText = www.downloadHandler.text;
                Debug.Log("Server response: " + responseText);

                ClassesResponse classCheckResponse = JsonUtility.FromJson<ClassesResponse>(responseText);

                if (classCheckResponse != null)
                {

                    UserJoinedClassesResponse[] classCheck = classCheckResponse.user_joined_classes;
                    if (classCheck.Length > 0) 
                    {
                        int lastClassId = classCheck.Length - 1;

                        UserDataSession.classID = classCheck[lastClassId].class_id;
                        UserDataSession.classCode = classCheck[lastClassId].classes.token;
                        UserDataSession.className = classCheck[lastClassId].classes.class_name;

                        PlayerPrefs.SetString("class_code", UserDataSession.classCode);
                        PlayerPrefs.SetString("class_id", UserDataSession.classID);

                        PlayerPrefs.Save();

                        Debug.Log("masuk ke kelas dengan kode: " + UserDataSession.classCode);

                    }
                    else
                    {
                        Debug.Log("\nUser belum terdaftar dalam kelas atau masalah lain dalam pengambilan data kelas");
                    }


                }
                else
                {
                    Debug.LogError("Gagal parse JSON: " + responseText);
                    Debug.Log("\nUser belum terdaftar dalam kelas atau masalah lain dalam pengambilan data kelas");

                }

                // Jika autologin dipanggil di scene 'main_menu'
                // Setup scene untuk menampilkan nama di tombol profile
                // Harus dipanggil setelah class_code dicek

            }
        }
    }
}
