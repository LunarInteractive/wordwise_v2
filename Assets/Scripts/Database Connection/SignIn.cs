using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

/// <summary> Fajar
/// Pengaturan saat sign in. Koneksi ke database untuk sign in diatur dari sini.
/// untuk API, bisa cek di sini: https://documenter.getpostman.com/view/20942971/2sAYJ9BK8j#0d03c40f-454e-405d-b87b-c72e01a5be01
/// Perlu diingat bahwa API tidak menyediakan respon jika email atau password 
/// bermasalah (masing-masing). Tapi kita bisa ngasih tahu user kalau input
/// email dan passwordnya perlu dicek lagi kalau salah.
/// </summary>
public class SignIn : MonoBehaviour
{
    [SerializeField] private FormChecker formChecker;
    public void StartConnecting(string email, string password)
    {

        StartCoroutine(GetDataSignIn(email, password));
    }
    public IEnumerator GetDataSignIn(string email, string password)
    {

        WWWForm form = new WWWForm();
        form.AddField("email", email);
        form.AddField("password", password);

        string header_type = "X-XSRF-TOKEN";
        string header_content = "{{xsrf-token}}";


        using (UnityWebRequest www = UnityWebRequest.Post("https://wordwise.id/api/v1/login", form))
        {
            www.SetRequestHeader(header_type, header_content);

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)// check for errors
            {
                Debug.Log(www.error);

                formChecker.ShowWarning("There was a problem while connecting to server.");

            }
            else
            {
                string responseText = (www.downloadHandler.text); //the echoed data
                Debug.Log("Server response: " + responseText);

                SigninResponse signinResponse = JsonUtility.FromJson<SigninResponse>(responseText);

                if (signinResponse != null)
                {
                    // Cek isi message
                    if (signinResponse.message == "Login successful")
                    {
                        Debug.Log("Sign up successful: " + signinResponse.message);

                        UserDataSession.id = signinResponse.user.id;
                        UserDataSession.role = signinResponse.user.role;
                        UserDataSession.fullName = signinResponse.user.name;
                        UserDataSession.email = signinResponse.user.email;
                        UserDataSession.schoolName = signinResponse.user.school;
                        

                        string userId = UserDataSession.id;
                        PlayerPrefs.SetString("user_id", userId);
                        PlayerPrefs.Save();

                        formChecker.ShowSubmit("Sign in successful.");
                    }
                    else
                    {
                        Debug.Log("Server message: " + signinResponse.message);
                        formChecker.ShowWarning("Sign in failed. Please re-check your email and password.");
                    }
                }
                else
                {
                    Debug.LogError("Gagal parse JSON: " + responseText);
                    formChecker.ShowWarning("There was a problem while retrieving user data.");
                }


                /// LEGACY SNIPPET untuk referensi pengecekan email sebelum submit password
                //{
                //    formChecker.ShowWarning("Email not found. Please insert registered email.");
                //    formChecker.MarkField(formChecker.email);
                //} else if (money == "1")
                //{
                //    formChecker.ShowWarning("Password incorrect. Please re-check your password.");
                //    formChecker.MarkField(formChecker.password);
                //}
                //else if (money == "Query Error")
                //{
                //    formChecker.ShowWarning(money + ".");
                    
                //}
                //else
                //{
                //    string[] moneys = money.Split("\n");
                    
                //    UserDataSession.id = moneys[0];
                //    UserDataSession.role = moneys[1];
                //    UserDataSession.fullName = moneys[2];
                //    UserDataSession.email = moneys[3];
                //    UserDataSession.schoolName = moneys[4];
                //    //UserDataSession.className = moneys[4];
                //    //UserDataSession.photoUrl = moneys[5];

                //    if (moneys.Length > 5) 
                //    {
                //        UserDataSession.classCode = moneys[5];
                //    }

                //    formChecker.ShowSubmit("Sign in successful.");
                //}
            }
        }
    }
}
