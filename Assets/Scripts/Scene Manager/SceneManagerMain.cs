using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Fajar
/// Mengatur nama di tombol profil dan pilihan pada panel Select Lesson.
/// Karena untuk mengambil data perlu login dulu, scene 'main_menu' perlu autologin.
/// SetUp() akan dipanggil oleh Autologin setelah data-data yang diperlukan
/// tersedia. 
/// </summary>
public class SceneManagerMain : MonoBehaviour
{
    [SerializeField] TMP_Text userName;
    //[SerializeField] string userPhoto; //Untuk memasang foto di tombol profile

    [SerializeField] Button classButton;
    [SerializeField] Button storyButton;

    public void Awake()
    {

        if (UserDataSession.OnSession)
        {
            userName.text = UserDataSession.fullName.Split(' ')[0];

            if (!string.IsNullOrEmpty(UserDataSession.fullName))
            {
                storyButton.interactable = true;
            }
            else
            {
                classButton.interactable = false;
            }


            if (!string.IsNullOrEmpty(UserDataSession.classCode))
            {
                classButton.interactable = true;
            }
            else
            {
                classButton.interactable = false;
            }
        }
        else
        {
            userName.text = "Guest";
            classButton.interactable = false;
        }
    }



}
