using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginChecker : MonoBehaviour
{
    [SerializeField] GameObject loginPanel;

    void Start()
    {
        if (UserDataSession.OnSession)
        {
            Debug.Log("user telah login, tidak masuk ke panel login");
        }
        else
        {
            loginPanel.SetActive(true);
        }
    }
}
