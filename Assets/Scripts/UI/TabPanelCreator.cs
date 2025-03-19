using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TabPanelCreator : MonoBehaviour
{
    [Header("Tab Settings")]
    public GameObject tabChapterPrefab;       // Prefab panel chapter
    public TabContainerManager tabManager;    // Digunakan setelah panel dibuat

    public GameObject transitionObject;       // (Opsional) Jika Anda pakai object tertentu untuk transisi
    public TextMeshProUGUI chapterTitle;

    [Header("Reference Dialog Manager")]
    public DialogManager dialogManager;       // ← agar bisa get data levels


    // Panel yang di-generate
    private List<GameObject> panels = new List<GameObject>();

    private void Start()
    {
        InitializePanels();
    }

    public void InitializePanels()
    {
        // 1) Bersihkan panel-panel lama
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        transform.DetachChildren();
        panels.Clear();

        // 2) Ambil semua data level
        List<LevelData> allLevels = GlobalVariable.GetAllLevelsList();
        if (allLevels == null || allLevels.Count == 0)
        {
            Debug.LogWarning("No levels data found.");
            return;
        }

        // 3) Group by chapter_name
        var groupedByChapter = allLevels
                               .OrderBy(l => l.chapter_name)
                               .GroupBy(l => l.chapter_name);

        // 4) Buat panel + isi tombol level untuk setiap chapter
        foreach (var chapterGroup in groupedByChapter)
        {
            string chapterName = chapterGroup.Key;
            // group ini berisi semua level yg punya chapter_name = chapterName

            // a) Buat panel chapter
            GameObject chapterContent = Instantiate(tabChapterPrefab, transform);
            chapterContent.gameObject.name = $"Panel Chapter {chapterName}";

            TabContentCreator contentCreator = chapterContent.GetComponent<TabContentCreator>();
            if (contentCreator != null)
            {
                contentCreator.dialogManager = this.dialogManager;

                // --- Set variabel chapterName di sini ---
                contentCreator.chapterName = chapterName;

                // Lalu panggil fungsi untuk membuat tombol level
                List<LevelData> levelsInChapter = chapterGroup.ToList();
                contentCreator.CreateLevelButtons(levelsInChapter, transitionObject);
            }
                
            // c) Buat tombol level
            //    Di sini, Anda bisa pakai method "InitializePanel" bawaan,
            //    tapi kita perlu modifikasi agar bisa kirim daftar level.
            if (contentCreator != null)
            {
                // Convert 'chapterGroup' (IEnumerable<LevelData>) ke List
                List<LevelData> levelsInChapter = chapterGroup.ToList();

                contentCreator.CreateLevelButtons(levelsInChapter, transitionObject);
            }

            panels.Add(chapterContent);
        }

        // 5) Inisialisasi tab manager
        tabManager.InitializeTabs();
    }

    public void SetChapterTitle(string chapterName)
    {
        if (chapterTitle != null)
        {
            Debug.Log(chapterName);
            chapterTitle.text = chapterName;
        }
    }
}
