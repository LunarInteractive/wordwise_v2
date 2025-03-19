using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;

public class DownloadImageExample : MonoBehaviour
{
    [Header("Link Gambar dari Server")]
    public string imageUrl = "https://lunarinteractive.net/Sekolah%20Hamidah%20Sampurna.png";

    [Header("Tempat Menampilkan Gambar (UI Image)")]
    public Image targetImage;

    // Contoh memulai download ketika scene aktif
    private void Start()
    {
        StartCoroutine(DownloadAndSetImage());
    }

    private IEnumerator DownloadAndSetImage()
    {
        // Buat request untuk mendownload texture dari URL
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(imageUrl);

        // Mulai download
        yield return request.SendWebRequest();

        // Cek apakah terjadi error
        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Gagal mendownload gambar: {request.error}");
        }
        else
        {
            // Jika berhasil, dapatkan Texture2D
            Texture2D downloadedTexture = DownloadHandlerTexture.GetContent(request);



            // Buat sprite dari Texture2D
            Sprite newSprite = Sprite.Create(
                downloadedTexture,
                new Rect(0, 0, downloadedTexture.width, downloadedTexture.height),
                Vector2.zero
            );

            // Tampilkan sprite pada UI Image
            targetImage.sprite = newSprite;
        }
    }
}
