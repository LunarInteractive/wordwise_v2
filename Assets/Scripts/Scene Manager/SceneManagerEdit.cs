using TMPro;
using UnityEngine;

/// <summary> Fajar
/// Mengatur tampilan scene 'Edit'. Mengisi tiap inputfield dengan data yang akan diubah.
/// Sebelumnya email bisa diganti dari scene ini, tapi berhubung dari backend membatasi
/// hal tsb. (batasan database), email di comment dulu. Suatu waktu bisa di uncomment tapi
/// Mengubah email sepertinya tidak akan dari scene ini.
/// </summary>
public class SceneManagerEdit : MonoBehaviour
{
    [SerializeField] TMP_InputField fullName;
    //[SerializeField] TMP_InputField email;
    [SerializeField] TMP_InputField schoolName;

    void Start()
    {
        if (UserDataSession.OnSession)
        {
            fullName.text = UserDataSession.fullName;
            //email.text = UserDataSession.email;
            schoolName.text = UserDataSession.schoolName;

        }

    }
}
