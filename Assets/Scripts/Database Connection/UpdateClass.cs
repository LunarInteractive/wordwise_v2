using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;


/// <summary>
/// Pengaturan saat menambahkan kelas. Koneksi ke database untuk mengubah kelas diatur dari sini.
/// API saat ini hanya memungkinkan untuk menambahkan data user dan kelas yang diikuti ke table joined_class
/// API belum bisa memeriksa jika user sudah terdaftar di kelas tersebut sebelumnya.
/// Script akan memanggil warning panel saat request daftar kelas diproses dan saat selesai (berhasil/gagal).
/// Setelah mendaftarkan user ke class, sistem akan mengakses database lagi pakai perintah yang berbeda
/// untuk mengambil data class_name, class_id, dan token/class_code.
/// API: https://documenter.getpostman.com/view/20942971/2sAYJ9BK8j#0d03c40f-454e-405d-b87b-c72e01a5be01
/// </summary>

public class UpdateClass : MonoBehaviour
{
    [SerializeField] private TMP_InputField inputClass;
    [SerializeField] private WarningPanel processPanel;
    [SerializeField] private WarningPanel warningPanel;
    [SerializeField] private ProfileFunction profileFunction;


    public void StartConnecting()
    {
        // Cek apakah user memasukkan text atau tidak
        if (string.IsNullOrEmpty(inputClass.text))
        {
            warningPanel.SetContent("Please input a valid class code.");
            warningPanel.gameObject.SetActive(true);
        } else
        {
            processPanel.SetContent("Joining Class " + inputClass.text + "...");
            processPanel.gameObject.SetActive(true);
            StartCoroutine(GetDataClass(inputClass.text));
        }        
    }
    
    public IEnumerator GetDataClass(string classCode)
    {

        WWWForm form = new WWWForm();
        string userID = UserDataSession.id;
        Debug.Log(userID + ", " + classCode);
        form.AddField("user_id", userID);
        form.AddField("token", classCode);

        using (UnityWebRequest www = UnityWebRequest.Post("https://wordwise.id/api/v1/joinedclass?user_id=2&token=Cz6vgJVf82", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)// check for errors
            {
                string responseText = www.downloadHandler.text;
                Debug.Log(responseText);

                RegistrationResponse regResponse = JsonUtility.FromJson<RegistrationResponse>(responseText);

                warningPanel.SetContent("There was a problem while connecting to server.");
                warningPanel.gameObject.SetActive(true);

            }
            else
            {
                string responseText = www.downloadHandler.text;

                Debug.Log("Server response: " + responseText);

                ClassCodeResponse regResponse = JsonUtility.FromJson<ClassCodeResponse>(responseText);


                if (regResponse != null)
                {
                    Debug.Log("update class successful: " + regResponse.message);

                    /// Sebelumnya, menampilkan class_code/token di Scene 'profile' 
                    /// hanya mengambil dari teks yang baru diinput oleh user pada 
                    /// form panel addClass (classCode). Sekarang, data diambil lagi 
                    /// dari database dan menampilkan class_name.
                    /// Sistem lama masih disimpan kalau sistem sekarang bermasalah.
                    
                    StartCoroutine(CheckClass(regResponse.data.user_id));

                    //warningPanel.SetContent("New class registration successful.");
                    //warningPanel.gameObject.SetActive(true);


                    //UserDataSession.classID = regResponse.data.class_id;

                    //UserDataSession.classCode = classCode;
                    //profileFunction.AddClass(UserDataSession.classCode);

                    //string classcode = UserDataSession.classCode;
                    //string classid = UserDataSession.classID;

                    //PlayerPrefs.SetString("class_id", classid);
                    //PlayerPrefs.SetString("class_code", classcode);
                    //PlayerPrefs.Save();


                }
                else
                {
                    Debug.LogError("Gagal parse JSON: " + responseText);
                    Debug.Log("\nUser belum terdaftar dalam kelas atau masalah lain dalam pengambilan data kelas");
                    warningPanel.SetContent("There was a problem while registering user to class.");
                    warningPanel.gameObject.SetActive(true);
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
                warningPanel.SetContent("There was a problem while connecting to server.");
                warningPanel.gameObject.SetActive(true);
            }
            else
            {
                string responseText = www.downloadHandler.text;
                Debug.Log("Server response: " + responseText);

                ClassesResponse classCheckResponse = JsonUtility.FromJson<ClassesResponse>(responseText);

                if (classCheckResponse != null)
                {

                    UserJoinedClassesResponse[] classCheck = classCheckResponse.user_joined_classes;

                    /// Memastikan kalau user_id memang sudah ada di joined_class
                    /// dan isi array classCheck lebih dari 0
                    if (classCheck.Length > 0) 
                    {
                        int lastClassId = classCheck.Length - 1;

                        UserDataSession.classID = classCheck[lastClassId].class_id;
                        UserDataSession.classCode = classCheck[lastClassId].classes.token;
                        UserDataSession.className = classCheck[lastClassId].classes.class_name;

                        profileFunction.AddClass(UserDataSession.className);

                        /// className tidak disimpan di playerpref karena sebaiknya dipanggil lagi dari
                        /// database saja berdasarkan userID lewat fitur AutoLogin. 
                        /// ClassCode dan classID menurutku ga perlu disimpan di PlayerPref
                        /// juga, tapi mungkin dibutuhkan untuk keperluan lain.

                        PlayerPrefs.SetString("class_code", UserDataSession.classCode);
                        PlayerPrefs.SetString("class_id", UserDataSession.classID);

                        PlayerPrefs.Save();

                        warningPanel.SetContent("You have registered to Class " + UserDataSession.className);
                        warningPanel.gameObject.SetActive(true);

                        Debug.Log("masuk ke kelas dengan kode: " + UserDataSession.classCode);
                    }
                    else
                    {
                        warningPanel.SetContent("Registration to Class " + UserDataSession.className + " failed. Please, try again.");
                        warningPanel.gameObject.SetActive(true);
                        Debug.Log("\nUser belum terdaftar dalam kelas atau masalah lain dalam pengambilan data kelas");
                    }
                    

                }
                else
                {
                    Debug.LogError("Gagal parse JSON: " + responseText);
                    Debug.Log("\nUser belum terdaftar dalam kelas atau masalah lain dalam pengambilan data kelas");
                    warningPanel.SetContent("There was a problem when retrieving class data.");
                    warningPanel.gameObject.SetActive(true);
                }
            }
        }
    }
}
