using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabGroup : MonoBehaviour
{
    public List<TabButton> tabButtons;
    public List<GameObject> objectToSwap;

    public Sprite tabIdle;
    public Sprite tabHover;
    public Sprite tabActive;

    public Color colorIdle = Color.white;
    public Color colorHover = Color.gray;
    public Color colorActive = Color.blue;

    public TabButton selectedTab;

    public void Subscribe(TabButton button)
    {
        if (tabButtons == null)
        {
            tabButtons = new List<TabButton>();
        }

        tabButtons.Add(button);
    }

    public void OnTabEnter(TabButton button)
    {
        ResetTab();
        if (selectedTab == null || button != selectedTab)
        {
            button.background.sprite = tabHover;
            button.background.color = colorHover;
        }
    }

    public void OnTabExit(TabButton button)
    {
        ResetTab();
    }

    public void OnTabSelected(TabButton button)
    {
        if (selectedTab != null)
        {
            selectedTab.Deselect();
        }

        selectedTab = button;

        selectedTab.Select();

        ResetTab();
        button.background.sprite = tabActive;
        button.background.color = colorActive;
        int index = button.transform.GetSiblingIndex();
        for (int i = 0; i < objectToSwap.Count; i++)
        {
            if (i == index)
            {
                objectToSwap[i].SetActive(true);
            }
            else
            {
                objectToSwap[i].SetActive(false);
            }
        }
    }

    public void ResetTab()
    {
        foreach (TabButton button in tabButtons)
        {
            if (selectedTab != null && button == selectedTab) { continue; }
            button.background.sprite = tabIdle;
            button.background.color = colorIdle;
        }
    }
}
