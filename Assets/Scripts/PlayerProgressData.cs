using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProgressData : MonoBehaviour
{
    [System.Serializable]
    public class LevelData
    {
        public string LevelName;
        public int Score;

        public LevelData(string levelName, int score)
        {
            LevelName = levelName;
            Score = score;
        }
    }

    [System.Serializable]
    public class ChapterData
    {
        public string ChapterName;
        public List<LevelData> Levels;

        public ChapterData(string chapterName)
        {
            ChapterName = chapterName;
            Levels = new List<LevelData>();
        }
    }

    [System.Serializable]
    public class PlayerProgress
    {
        public List<ChapterData> Chapters;

        public PlayerProgress()
        {
            Chapters = new List<ChapterData>();
        }
    }

}
