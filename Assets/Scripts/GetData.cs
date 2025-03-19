using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.Networking;
using TMPro;

public class GetData : MonoBehaviour
{
    public Button loginbutton;
    public TMP_InputField UsernameInput;
    public TMP_InputField PasswordInput;
    // Use this for initialization
    //void Start()
    //{
    //    //starts function when button is clicked
    //    loginbutton.onClick.AddListener(() => {

    //        StartCoroutine(getData(UsernameInput.text, PasswordInput.text));
    //    });
    //}



    // Update is called once per frame
    public void StartConnecting()
    {
        StartCoroutine(GetDataSignIn(UsernameInput.text, PasswordInput.text));
    }
    public IEnumerator GetDataSignIn(string username, string password)
    {

        WWWForm form = new WWWForm();
        form.AddField("loginUser", username);
        form.AddField("loginPass", password);


        using (UnityWebRequest www = UnityWebRequest.Post("https://kasumba.id/getdata.php", form))//web requests and sends data as _POST to php script
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)// check for errors
            {
                Debug.Log(www.error);

            }
            else
            {
                string money = (www.downloadHandler.text); //the echoed data

                Debug.Log("money = " + money);//displays the amount of money





            }
        }

    }
}
