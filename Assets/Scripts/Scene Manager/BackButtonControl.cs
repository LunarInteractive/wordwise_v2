using TransitionsPlus;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary> Fajar
/// Script untuk respon tombol back pada device dan 
/// menyimpan playerprefs saat keluar app
/// </summary>
public class BackButtonControl : MonoBehaviour
{
    

    [SerializeField] WarningPanel warningPanel;
    [SerializeField] TransitionAnimator transition;

    private void Start()
    {
        ////Jika di scene pertama, load data user jika ada
        //if ( SceneManager.GetActiveScene().name == "splash")
        //{
        //    if ((PlayerPrefs.HasKey("user_id")) && (!string.IsNullOrEmpty(PlayerPrefs.GetString("user_id"))))
        //    {
        //        UserDataSession.id = PlayerPrefs.GetString("user_id");
        //        UserDataSession.classCode = PlayerPrefs.GetString("class_code");
        //        UserDataSession.classID = PlayerPrefs.GetString("class_id");
        //        Debug.Log("load");
        //    }
            
        //}
        
    }


    private void Update()
    {

        //old input system//if (Input.GetKeyDown(KeyCode.Escape))
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            // Respon tombol back ada 2:
            // - kembali ke layar sebelumnya (jika sudah diset manual)
            // - menampilkan tombol konfirmasi keluar app (jika ada warningPanel di assign)

            if (warningPanel)
            {

                warningPanel.gameObject.SetActive(true);
            }
            else if (transition)
            {
                TransitionToScene(transition);
            }
        }
    }

    public void TransitionToScene(TransitionAnimator transition)
    {
        transition.gameObject.SetActive(true);
    }

    public void QuitApps()
    {
        Application.Quit();
    }

    //Memastikan data tersimpan jika user sudah sign in sehingga tidak perlu sign in lagi saat buka app setelahnya
    //Juga memastikan tidak ada tersimpan jika app ditutup dalam keadaan sign out
    private void OnApplicationQuit()
    {
        if (UserDataSession.OnSession)
        {
            PlayerPrefs.SetString("user_id", UserDataSession.id);
            PlayerPrefs.SetString("class_code", UserDataSession.classCode);
            PlayerPrefs.SetString("class_id", UserDataSession.classID);


        }
        else
        {
            //PlayerPrefs.DeleteAll();
            PlayerPrefs.DeleteKey("user_id");
            PlayerPrefs.DeleteKey("class_code");
            PlayerPrefs.DeleteKey("class_id");

        }
        PlayerPrefs.Save();
        Debug.Log("saved");
    }

}
