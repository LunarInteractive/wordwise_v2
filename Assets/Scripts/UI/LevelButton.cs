using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelButton : MonoBehaviour
{
    [Header("Lock")]
    public GameObject lockIcon; // Game object ikon kunci
    public GameObject text; // Teks tombol
    bool lockState = true;  // Status kunci

    public void lockUpdate(bool state)
    {
        // Jika lockUpdate(true) icon dimunculkan, button non-aktif
        lockIcon.SetActive(state);
        gameObject.GetComponent<Button>().interactable = !state;
    }


}
