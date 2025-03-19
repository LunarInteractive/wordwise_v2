using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class TabContainerManager : MonoBehaviour
{
    [Header("UI References")]
    public GameObject tabButtonPrefab; // Prefab untuk tombol tab
    public Transform tabButtonParent;  // Parent tempat tombol tab akan dibuat
    public Transform chapterContainerParent; // Parent dari semua Chapter Container
    public TMP_Text chapterLabel;      // TMP_Text untuk menampilkan nama Chapter aktif
    public Button nextButton; // Button buat aktifkan tab selanjutnya atau bisa  untuk geser scroll ke kanan
    public Button prevButton; // Button buat aktifkan tab sebelumnya atau bisa  untuk geser scroll ke kiri
    public Scrollbar scrollTab; // Scrollbar horizontal yang ngatur scroll tabButtonParent
    public float scrollSmoothness = 0.01f; // Jumlah geseran scroll jika digeser pakai button

    [Header("Tab Settings")]
    public Color activeTabColor = Color.white;     // Warna tombol aktif
    public Color inactiveTabColor = Color.gray;   // Warna tombol tidak aktif
    public Color activeTextColor = Color.black;   // Warna teks aktif
    public Color inactiveTextColor = Color.white; // Warna teks tidak aktif
    public float activeTextSize = 18f;            // Ukuran teks aktif
    public float inactiveTextSize = 14f;          // Ukuran teks tidak aktif

    private List<GameObject> chapterContainers = new List<GameObject>(); // List dari semua Chapter Container
    private List<Button> tabButtons = new List<Button>();               // List dari semua tombol tab
    private List<TMP_Text> tabTexts = new List<TMP_Text>();             // List dari semua teks tombol tab
    private int activeTab = 0; // Indeks tab aktif

    void Start()
    {
       //InitializeTabs();
        
    }

    
    public void InitializeTabs()
    {

        // Hapus tombol tab yang ada (jika ada)
        foreach (Transform child in tabButtonParent)
        {
            Destroy(child.gameObject);
        }

        chapterContainers.Clear();
        tabButtons.Clear();
        tabTexts.Clear();

        // Cari semua Chapter Container
        foreach (Transform child in chapterContainerParent)
        {
            if (child.gameObject.activeSelf)
            {
                chapterContainers.Add(child.gameObject);
            }
        }

        // Buat tombol tab untuk setiap Chapter Container
        for (int i = 0; i < chapterContainers.Count; i++)
        {
            int index = i; // Perlu untuk closure dalam listener

            // Instantiate tombol tab
            GameObject tabButton = Instantiate(tabButtonPrefab, tabButtonParent);
            tabButton.gameObject.name = "Tombol Chapter " + (i + 1);

            // Ubah teks tombol menggunakan TMP_Text
            TMP_Text buttonText = tabButton.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = "Chapter " + (i + 1);
                tabTexts.Add(buttonText);
            }

            // Tambahkan event listener ke tombol
            Button button = tabButton.GetComponent<Button>();
            button.onClick.AddListener(() => OnTabButtonClicked(index));

            // Simpan tombol ke dalam list
            tabButtons.Add(button);
        }

        // Set tab pertama sebagai aktif secara default
        if (chapterContainers.Count > 0)
        {
            ShowChapterContainer(0);
        }
    }

    void OnTabButtonClicked(int index)
    {
        ShowChapterContainer(index);
    }

    public void ShowChapterContainer(int index)
    {
        // Tampilkan hanya Chapter Container yang dipilih
        for (int i = 0; i < chapterContainers.Count; i++)
        {
            chapterContainers[i].SetActive(i == index);

            // Ubah warna tombol tab berdasarkan status aktif
            if (i < tabButtons.Count)
            {
                ColorBlock colors = tabButtons[i].colors;
                colors.normalColor = (i == index) ? activeTabColor : inactiveTabColor;
                tabButtons[i].colors = colors;
            }

            // Ubah warna dan ukuran teks
            if (i < tabTexts.Count)
            {
                TMP_Text text = tabTexts[i];
                text.color = (i == index) ? activeTextColor : inactiveTextColor;
                text.fontSize = (i == index) ? activeTextSize : inactiveTextSize;
            }
        }

        // Ambil nama chapter dari script TabContentCreator
        if (chapterLabel != null && index < chapterContainers.Count)
        {
            TabContentCreator contentCreator = chapterContainers[index].GetComponent<TabContentCreator>();
            if (contentCreator != null)
            {
                chapterLabel.text = contentCreator.chapterName; // Tampilkan nama chapter
            }
            else
            {
                // fallback jika tidak ada script-nya
                chapterLabel.text = "Chapter " + (index + 1);
            }
        }

        // Nonaktifkan tombol panah jika tidak perlu
        if (activeTab == 0)
        {
            nextButton.interactable = true;
            prevButton.interactable = false;
        }
        else if (activeTab == tabButtons.Count-1)
        {
            prevButton.interactable = true;
            nextButton.interactable = false;
        } else
        {
            prevButton.interactable = true;
            nextButton.interactable = true;
        }


    }

    public void NextTab()
    {
        // Aktifkan tab di kanan dari yang aktif, selama masih ada
        if (activeTab < tabButtons.Count - 1)
        {
            activeTab++;
            ShowChapterContainer(activeTab);
        }
    }

    public void PrevTab()
    {
        // Aktifkan tab di kiri dari yang aktif, selama masih ada
        if (activeTab > 0)
        {
            activeTab--;
            ShowChapterContainer(activeTab);
        }
    }

    public void ScrollLeft()
    {
        // Geser Scroll ke kiri
        UpdateScroll(-scrollSmoothness);
    }

    public void ScrollRight()
    {
        // Geser Scroll ke kanan
        UpdateScroll(scrollSmoothness);
    }

    public void UpdateScroll(float amount)
    {
        Canvas.ForceUpdateCanvases();

        scrollTab.value = Math.Clamp(scrollTab.value + amount, 0.0f, 1.0f);

    }




}


