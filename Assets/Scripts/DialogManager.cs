using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using PixelCrushers.DialogueSystem;

public class DialogManager : MonoBehaviour
{
    // Ganti dengan endpoint API yang sesuai untuk levels

    void Start()
    {
        Debug.Log(UserDataSession.classID);
        // Muat data dari file JSON (jika tersedia)
        GlobalVariable.LoadFromJsonFile();
        // Mulai permintaan HTTP untuk memperbarui data dari API
        StartCoroutine(FetchLevelsData());
    }

    public void startFetchIE()
    {
        StartCoroutine(FetchLevelsData());
    }

    IEnumerator FetchLevelsData()
    {

        string apiUrl = "https://wordwise.id/api/v1/classes/" + UserDataSession.classID;

        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to fetch levels data: " + request.error);
            }
            else
            {
                string jsonResponse = request.downloadHandler.text;
                Debug.Log(UserDataSession.classID);
                Debug.Log(jsonResponse);

                // Parse data dari API
                ClassAndLevelsResponse apiData = JsonUtility.FromJson<ClassAndLevelsResponse>(jsonResponse);

                if (apiData != null && apiData.levels != null)
                {
                    // Dapatkan JSON dari data yang sudah tersimpan di local
                    string localDataJson = GlobalVariable.GetCurrentDataAsJson();

                    // Bikin JSON dari "apiData.levels" saja
                    string apiDataJson = JsonUtility.ToJson(new LevelResponse { data = apiData.levels }, true);

                    if (localDataJson == apiDataJson)
                    {
                        // Data sama, tidak perlu update
                        Debug.Log("Local JSON data is identical to API data. No update needed.");
                    }
                    else
                    {
                        // Data berbeda, update file JSON
                        // Hanya kirim "levels" ke GlobalVariable
                        GlobalVariable.SetAllLevelsData(apiData.levels);
                        GlobalVariable.SaveToJsonFile();
                        Debug.Log("Local JSON data was different. File has been updated.");
                    }
                }
                else
                {
                    Debug.LogError("Data from API is not in the expected format (no 'levels').");
                }
            }
        }
    }


    // Method untuk mengambil dialog berdasarkan class_id dan level_name
    public string GetDialogueByClassAndLevel(long classId, string levelName)
    {
        LevelData levelData = GlobalVariable.GetLevelByClassAndName(classId, levelName);

        if (levelData != null)
        {
            Debug.Log($"Retrieved Dialogue for Class ID {classId}, Level '{levelName}': {levelData.dialogue_data}");
            return levelData.dialogue_data;
        }
        else
        {
            Debug.LogError($"Dialogue not found for Class ID: {classId}, Level: {levelName}");
            return $"Dialogue not found for Class ID: {classId}, Level: {levelName}";
        }
    }

    // Contoh method untuk memulai level
    public void StartLevel(long classId, string levelName)
    {
        string dialogue = GetDialogueByClassAndLevel(classId, levelName);
        if (!string.IsNullOrEmpty(dialogue))
        {
            Debug.Log($"Starting Level: Class ID {classId}, Level '{levelName}'");
            Debug.Log($"Dialogue Data: {dialogue}");

            // Parsing XML jika diperlukan
            ParseDialogueData(dialogue);

            // Implementasikan logika dialog di sini
            // Misalnya, memulai dialog dengan dialog_data yang telah diparsing
        }
        else
        {
            Debug.LogError($"Cannot start level. Dialogue not found for Class ID {classId}, Level '{levelName}'.");
        }
    }

    // Contoh method untuk memparsing dialogue_data dari XML
    public void ParseDialogueData(string xmlData)
    {
        try
        {
            XElement levelElement = XElement.Parse(xmlData);

            // Contoh: Mengambil MainDialog
            XElement mainDialog = levelElement.Element("MainDialog");
            if (mainDialog != null)
            {
                string character = mainDialog.Element("Character")?.Value;
                string question = mainDialog.Element("Question")?.Value;

                Debug.Log($"Character: {character}");
                Debug.Log($"Question: {question}");

                // Mengambil opsi
                var options = mainDialog.Elements("OptionA")
                                        .Concat(mainDialog.Elements("OptionB"))
                                        .Concat(mainDialog.Elements("OptionC"))
                                        .Concat(mainDialog.Elements("OptionD")); // Tambahkan opsi lain jika ada

                foreach (var option in options)
                {
                    string action = option.Attribute("Action")?.Value;
                    string optionText = option.Value;

                    Debug.Log($"Option: {optionText}, Action: {action}");
                }

                // Implementasikan logika dialog Anda berdasarkan data yang diparsing
            }
            else
            {
                Debug.LogError("MainDialog not found in dialogue_data.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to parse dialogue_data XML: {ex.Message}");
        }
    }

    public void FetchDialogueAndCreateXML(long classId, string levelname)
    {
        string dialogueXml = GetDialogueByClassAndLevel(classId, levelname);

        if (string.IsNullOrEmpty(dialogueXml))
        {
            Debug.LogError("Failed to get dialogue data from DB.");
            return;
        }

        string folderPath = Path.Combine(Application.persistentDataPath, "DialogueFiles");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string filePath = Path.Combine(folderPath, "CurrentDialogue.xml");

        try
        {
            File.WriteAllText(filePath, dialogueXml);
            Debug.Log($"File XML created/updated: {filePath}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Failed to write XML: {ex.Message}");
        }
    }

}

[System.Serializable]
public class ClassAndLevelsResponse
{
    public ClassDetail @class;   // Penampung data "class"
    public List<LevelData> levels;  // Penampung data "levels"
}

[System.Serializable]
public class ClassDetail
{
    public long id;
    public long user_id;
    public string class_name;
    public string token;
    public string created_at;
    public string updated_at;
}

// Struktur untuk memetakan respons JSON
[System.Serializable]
public class DialogueResponse
{
    public List<DialogueData> data; // Data list dari dialog
}

// Struktur untuk data dialog individu
[System.Serializable]
public class DialogueData
{
    public int id;
    public string dialogue_data;
    public string created_at;
    public string updated_at;
}

[System.Serializable]
public class LevelResponse
{
    public List<LevelData> data;
}

[System.Serializable]
public class LevelData
{
    public long id;
    public long class_id;
    public string level_name;
    public string chapter_name;
    public string dialogue_data;
    public string created_at;
    public string updated_at;
}

// Kelas GlobalVariable untuk menyimpan data
public static class GlobalVariable
{
    private static Dictionary<long, Dictionary<string, LevelData>> levelsData = new Dictionary<long, Dictionary<string, LevelData>>();

    private static string filePath => Path.Combine(Application.persistentDataPath, "levels.json");

    public static void SetAllLevelsData(List<LevelData> levelDataList)
    {
        levelsData.Clear();

        foreach (var level in levelDataList)
        {
            if (!levelsData.ContainsKey(level.class_id))
            {
                levelsData[level.class_id] = new Dictionary<string, LevelData>();
            }

            if (!levelsData[level.class_id].ContainsKey(level.level_name))
            {
                levelsData[level.class_id][level.level_name] = level;
            }
            else
            {
                Debug.LogWarning($"Duplicate level_name '{level.level_name}' found for class_id {level.class_id}. Overwriting existing data.");
                levelsData[level.class_id][level.level_name] = level;
            }
        }

    }

    public static List<LevelData> GetAllLevelsList()
    {
        List<LevelData> allLevels = new List<LevelData>();
        foreach (var classEntry in levelsData)
        {
            foreach (var levelEntry in classEntry.Value) // levelEntry: Key=levelName, Value=LevelData
            {
                allLevels.Add(levelEntry.Value);
            }
        }
        return allLevels;
    }


    public static LevelData GetLevelByClassAndName(long classId, string levelName)
    {
        if (levelsData.TryGetValue(classId, out Dictionary<string, LevelData> classLevels))
        {
            if (classLevels.TryGetValue(levelName, out LevelData levelData))
            {
                return levelData;
            }
        }
        return null;
    }

    public static void SaveToJsonFile()
    {
        List<LevelData> allData = new List<LevelData>();
        foreach (var classEntry in levelsData)
        {
            foreach (var levelEntry in classEntry.Value)
            {
                allData.Add(levelEntry.Value);
            }
        }

        string jsonData = JsonUtility.ToJson(new LevelResponse { data = allData }, true);
        File.WriteAllText(filePath, jsonData);
        Debug.Log($"Data saved to file: {filePath}");
    }

    public static void LoadFromJsonFile()
    {
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            LevelResponse response = JsonUtility.FromJson<LevelResponse>(jsonData);
            if (response != null && response.data != null)
            {
                SetAllLevelsData(response.data);
                Debug.Log("Data loaded from file.");
            }
            else
            {
                Debug.LogError("Failed to parse JSON from file.");
            }
        }
        else
        {
            Debug.LogWarning("No saved file found to load data.");
        }
    }

    public static string GetCurrentDataAsJson()
    {
        List<LevelData> allData = new List<LevelData>();
        foreach (var classEntry in levelsData)
        {
            foreach (var levelEntry in classEntry.Value)
            {
                allData.Add(levelEntry.Value);
            }
        }

        return JsonUtility.ToJson(new LevelResponse { data = allData }, true);
    }
}
