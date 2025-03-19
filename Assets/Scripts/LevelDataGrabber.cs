using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.IO;
using System.Xml;

[System.Serializable]
public class LevelDataTest
{
    public int id;
    public int class_id;
    public string dialogue_data;
    public string level_name;
    public string photo_url;
}

[System.Serializable]
public class ApiResponse
{
    public LevelDataTest data;
}

public class LevelDataGrabber : MonoBehaviour
{
    private string apiUrl = "https://lunarinteractive.net/api/v1/levels/7";

    void Start()
    {
        // Mulai proses ambil data
        StartCoroutine(FetchLevelData());
    }

    IEnumerator FetchLevelData()
    {
        // 1. Request data JSON dari server
        using (UnityWebRequest request = UnityWebRequest.Get(apiUrl))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError
                || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error fetching data: " + request.error);
                yield break;
            }

            // 2. Parse JSON -> ApiResponse
            string json = request.downloadHandler.text;
            ApiResponse response = JsonUtility.FromJson<ApiResponse>(json);
            if (response == null || response.data == null)
            {
                Debug.LogError("Gagal parse JSON atau data kosong.");
                yield break;
            }

            // 3. Ambil link photo_url
            string photoUrl = response.data.photo_url;
            Debug.Log("Photo URL: " + photoUrl);

            // 4. Download gambar dari link photoUrl
            yield return StartCoroutine(DownloadImage(photoUrl));
        }
    }

    IEnumerator DownloadImage(string url)
    {
        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError
                || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Error downloading image: " + request.error);
                yield break;
            }

            // 5. Convert hasil download menjadi Texture2D
            Texture2D downloadedTexture = DownloadHandlerTexture.GetContent(request);

            // 6. Encode texture ke format PNG (bisa juga JPG)
            byte[] pngData = downloadedTexture.EncodeToPNG();

            // 7. Simpan ke folder di device (misalnya Application.persistentDataPath)
            //    Agar bisa dipakai di Android/iOS/PC tanpa masalah permission.
            //    Di sini, kita beri nama file "downloaded_image.png"
            string filePath = Path.Combine(Application.persistentDataPath, "downloaded_image.png");
            File.WriteAllBytes(filePath, pngData);
            Debug.Log("Gambar berhasil disimpan di: " + filePath);

            // 8. Simpan path file ke dalam XML
            SavePathToXml(filePath);
        }
    }

    private void SavePathToXml(string localPath)
    {
        // Contoh menyimpan path file ke XML sederhana
        // Buat file XML -> <LevelData><ImagePath>...</ImagePath></LevelData>

        // 1. Buat instance XmlDocument
        XmlDocument xmlDoc = new XmlDocument();

        // 2. Buat elemen root "LevelData"
        XmlElement root = xmlDoc.CreateElement("LevelData");
        xmlDoc.AppendChild(root);

        // 3. Buat elemen "ImagePath" dan isi dengan localPath
        XmlElement imagePathElem = xmlDoc.CreateElement("ImagePath");
        imagePathElem.InnerText = localPath;
        root.AppendChild(imagePathElem);

        // 4. Simpan file di persistentDataPath dengan nama "level_data.xml"
        string xmlPath = Path.Combine(Application.persistentDataPath, "level_data.xml");
        xmlDoc.Save(xmlPath);

        Debug.Log("XML dengan path gambar berhasil disimpan di: " + xmlPath);
    }
}
