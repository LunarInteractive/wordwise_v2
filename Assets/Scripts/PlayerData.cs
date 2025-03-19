using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using TMPro;

public class Data
{
    public string Chapter;
    public string Level;
    public string Score;

    public Data(string Chapter, string Level, string Score)
    {
        this.Chapter = Chapter;
        this.Level = Level;
        this.Score = Score;
    }
}

public class PlayerData : MonoBehaviour
{
    public string chapterInput;
    public string levelInput;
    public TMP_InputField scoreInput;

    public Data ReturnClass()
    {
        // Correcting the use of ToString method
        return new Data(chapterInput, levelInput, scoreInput.text);
    }

    public void SetUi(Data data)
    {
        // Assigning values from the Data object to the UI
        chapterInput = data.Chapter;
        levelInput = data.Level;
        scoreInput.text = data.Score;
    }
}
