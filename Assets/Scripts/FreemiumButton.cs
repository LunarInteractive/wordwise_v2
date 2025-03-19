using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreemiumButton : MonoBehaviour
{
    public int classId = 7;
    public DialogManager dialogManager;

    public void OnButtonBackPressed()
    {
        string savedclassID = PlayerPrefs.GetString("class_id");
        UserDataSession.classID = savedclassID;
    }

    public void OnButtonClassPressed() 
    {
        UserDataSession.classID = classId.ToString();

        dialogManager.startFetchIE();
    }
}
