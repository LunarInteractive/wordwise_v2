using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary> Fajar
/// Mengatur tampilan layout scene Profile yang bergantung pada UserDataSession.OnSession (kondisi sign in)
/// </summary>
public class SceneManagerProfile : MonoBehaviour
{

    [SerializeField] TMP_Text email;
    [SerializeField] TMP_Text fullName;
    [SerializeField] TMP_Text schoolName;
    //[SerializeField] TMP_Text className;
    [SerializeField] TMP_Text classCode;
    [SerializeField] GameObject addClass;
    [SerializeField] GameObject ChangeClass;
    //[SerializeField] photoUrl = moneys[5];


    [SerializeField] Button signIn;
    [SerializeField] Button signUp;
    [SerializeField] Button signOut;
    [SerializeField] public Button editProfile;
    [SerializeField] public Button changePass;

    void Start()
    {
        if (UserDataSession.OnSession)
        {
            fullName.text = UserDataSession.fullName;
            email.text = UserDataSession.email;
            schoolName.text = UserDataSession.schoolName;
            //className.text = UserDataSession.className;

            if (classCode)
            {
                if (UserDataSession.className != null)
                {
                    classCode.text = UserDataSession.className;
                    addClass.SetActive(false);
                    ChangeClass.SetActive(true);
                }
                else
                {
                    classCode.text = "-";
                    addClass.SetActive(true);
                    ChangeClass.SetActive(false);
                }

            }

            signIn.gameObject.SetActive(false);
            signUp.gameObject.SetActive(false);
            signOut.gameObject.SetActive(true);
            editProfile.gameObject.SetActive(true);
            changePass.gameObject.SetActive(true);

        }
        else
        {
            fullName.text = "Guest";
            email.text = "-";
            schoolName.text = "-";
            //className.text = "-";
            classCode.text = "-";

            addClass.SetActive(false);
            ChangeClass.SetActive(false);

            signIn.gameObject.SetActive(true);
            signUp.gameObject.SetActive(true);
            signOut.gameObject.SetActive(false);
            editProfile.gameObject.SetActive(false);
            changePass.gameObject.SetActive(false);
        }

    }

    public void UpdateProfile()
    {
        fullName.text = UserDataSession.fullName;
        email.text = UserDataSession.email;
        schoolName.text = UserDataSession.schoolName;
        //className.text = UserDataSession.className;

        if (classCode)
        {
            if (UserDataSession.className != null)
            {
                classCode.text = UserDataSession.className;
                addClass.SetActive(false);
                ChangeClass.SetActive(true);
            }
            else
            {
                classCode.text = "-";
                addClass.SetActive(true);
                ChangeClass.SetActive(false);
            }

        }

        signIn.gameObject.SetActive(false);
        signUp.gameObject.SetActive(false);
        signOut.gameObject.SetActive(true);
        editProfile.gameObject.SetActive(true);
        changePass.gameObject.SetActive(true);
    }

    public void AddClass(string className)
    {
        addClass.SetActive(false);
        ChangeClass.SetActive(true);
        classCode.SetText(className);
    }

}
