using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Fajar
/// Untuk mempermudah pemanggilan panel konfirmasi agar lebih modular
/// dan otomatis.
/// </summary>

public class WarningPanel : MonoBehaviour
{
    [SerializeField] public Animator blurAnimator;
    [SerializeField] public TMP_Text contentText;

    private void OnEnable()
    {
        
        blurAnimator.SetTrigger("blur");
        blurAnimator.SetBool("Blur", true);

    }

    //private void OnDisable()
    //{

    //    blurAnimator.SetTrigger("back");
    //    blurAnimator.SetBool("Blur", false);

    //}

    public void ManualBluroff()
    {

        blurAnimator.SetTrigger("back");
        blurAnimator.SetBool("Blur", false);

    }

    public void SetContent(string content)
    {
        contentText.text = content;
    }

}
