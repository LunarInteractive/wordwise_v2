using TMPro;
using UnityEngine;

/// <summary> Fajar
/// Hanya untuk menyembunyikan atau menampilkan password pada form.
/// Dilakukan dengan mengubah content type pada input_field password dan repeatpassword
/// </summary>

public class ShowHidePassword : MonoBehaviour
{

    
    public void SetVisibility(TMP_InputField inputField)
    {
        if (inputField.contentType == TMP_InputField.ContentType.Password)
        {
            inputField.contentType = TMP_InputField.ContentType.Standard;
        }
        else if (inputField.contentType == TMP_InputField.ContentType.Standard)
        {
            inputField.contentType = TMP_InputField.ContentType.Password;
            
        }
        inputField.ForceLabelUpdate();
    }
}
