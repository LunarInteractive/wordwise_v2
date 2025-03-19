using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary> Fajar
/// Pengaturan saat sign up. Koneksi ke database untuk sign up diatur dari sini.
/// API: https://documenter.getpostman.com/view/20942971/2sAYJ9BK8j#0d03c40f-454e-405d-b87b-c72e01a5be01
/// Input baru full name, email password, password conf, dan schoolname. Bisa dikembangkan.
/// Email ganda tidak dapat diregistrasi (sudah ada batasan dari backend), namun belum ada respon 
/// spesifik. Respon umum diberikan jika terdapat kegagalan dalam registrasi (line 79).
/// </summary>
public class UserRegistration : MonoBehaviour
{

    [SerializeField] private FormChecker formChecker;
    //public void StartConnecting(string full_name, string email, string password, string schoolName, string className, string photoPath)
    public void StartConnecting(string full_name, string email, string password, string password_conf, string schoolName)
    {

        //StartCoroutine(GetDataSignUp(full_name, email, password, schoolName, className, photoPath));
        StartCoroutine(GetDataSignUp(full_name, email, password, password_conf, schoolName));
    }
    //public IEnumerator GetDataSignUp(string full_name, string email, string password, string schoolName, string className, string photoPath)
    public IEnumerator GetDataSignUp(string full_name, string email, string password, string password_conf, string schoolName)
    {

        WWWForm form = new WWWForm();
        form.AddField("name", full_name);
        form.AddField("email", email);
        form.AddField("password", password);
        form.AddField("password_confirmation", password_conf);
        form.AddField("school", schoolName);
        //form.AddField("signUpClass", className);
        //form.AddField("photoUrl", photoPath);


        using (UnityWebRequest www = UnityWebRequest.Post("https://wordwise.id/api/v1/users?name=testapi&email=testapi@api.api&password=zaqwesdxc123&password_confirmation=zaqwesdxc123&school=Sekolah API", form))
        {

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError ||
                www.result == UnityWebRequest.Result.ProtocolError)// check for errors
            {
                Debug.Log(www.error);
                formChecker.ShowWarning("There was a problem while connecting to server.");

            }
            else
            {
                string responseText = www.downloadHandler.text;

                Debug.Log("Server response: " + responseText);

                RegistrationResponse regResponse = JsonUtility.FromJson<RegistrationResponse>(responseText);

                if (regResponse != null)
                {
                    // Cek isi message
                    if (regResponse.message == "User created successfully")
                    {
                        Debug.Log("Sign up successful: " + regResponse.message);

                        UserDataSession.id = regResponse.data.id;
                        UserDataSession.role = regResponse.data.role;
                        UserDataSession.fullName = regResponse.data.name;
                        UserDataSession.email = regResponse.data.email;
                        UserDataSession.schoolName = regResponse.data.school;

                        string userId = UserDataSession.id;
                        PlayerPrefs.SetString("user_id", userId);
                        PlayerPrefs.Save();

                        formChecker.ShowSubmit("Sign up successful.");
                    }
                    else
                    {
                        Debug.Log("Server message: " + regResponse.message);
                        formChecker.ShowWarning("Sign up failed. Please re-check your form.");
                    }
                }
                else
                {
                    Debug.LogError("Gagal parse JSON: " + responseText);
                    formChecker.ShowWarning("There was a problem while registering user data.");
                }


                /// LEGACY SNIPPET untuk referensi pengecekan respon email duplikat saat submit

                /* string money = (www.downloadHandler.text); //the echoed data

                if (money == "1")
                {
                    Debug.Log("money = " + money);
                    formChecker.ShowWarning("Email already exists. Please use another email adress.");
                    formChecker.MarkField(formChecker.email);
                }
                if (money == "0")
                {
                    Debug.Log("money = " + money);
                    formChecker.ShowSubmit("Sign up successful.");
                }
                else
                {
                    Debug.Log("money = " + money);
                    formChecker.ShowWarning(money);
                } */
            }
        }

    }
}
