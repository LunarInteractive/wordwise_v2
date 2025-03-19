using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using NativeGalleryNamespace;


/// <summary> Fajar
/// Fungsi untuk mengambil file gambar dari file explorer, menyalinnya ke folder Assets/Resource
/// tergantung orientasi dan di-crop di tengah. Ukuran masih fixed (300x400). Belum kepikiran
/// yang lebih responsive. Belum dipakai karena fitur photo profile belum implement.
/// </summary>

public class FileUploader : MonoBehaviour
{




    [SerializeField] Image imageContainer;
    [SerializeField] string pathopath;
    //public bool userImage = false;
    public string imagePath;
    public void FileExplorerTest()
    {
        #if UNITY_EDITOR
            imagePath = EditorUtility.OpenFilePanelWithFilters("Select Image File", "", new string[] { "Image files", "png,jpg,bmp"});

        #elif UNITY_ANDROID

            NativeGallery.GetImageFromGallery((path) => { imagePath = path; }, "Select Image File");
            
        #endif





        //FileUtil.CopyFileOrDirectory(imagePath, "Assets/Resources/imagePhoto.png");
        if (!string.IsNullOrEmpty(imagePath))
        {   
            System.IO.File.Copy(imagePath, "Assets/Resources/imagePhoto.png", true);
        }
        

        Texture2D textured = null;
        byte[] fileData;

        if (File.Exists(pathopath))
        {
   
            fileData = File.ReadAllBytes(pathopath);
            textured = new Texture2D(2, 2);
            textured.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        
        }

        Sprite sprited = Sprite.Create(textured, new Rect(0.0f, 0.0f, textured.width, textured.height), new Vector2(0.5f, 0.5f), 100.0f);
        
        imageContainer.sprite = sprited;
        imageContainer.SetNativeSize();
        imageContainer.preserveAspect = true;
        

        if (imageContainer.rectTransform.rect.width > imageContainer.rectTransform.rect.height)
        {
            imageContainer.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 400.0f);

        }
        else
        {
            imageContainer.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300.0f);
        }

        imageContainer.rectTransform.pivot = new Vector2(0.5f, 0.0f);
        imageContainer.rectTransform.anchoredPosition = new Vector2(0.0f, 0.0f);
        imageContainer.color = Color.white;

    }

    private async void RequestPermissionAsynchronously(NativeGallery.PermissionType permissionType, NativeGallery.MediaType mediaTypes)
    {
        NativeGallery.Permission permission = await NativeGallery.RequestPermissionAsync(permissionType, mediaTypes);
        Debug.Log("Permission result: " + permission);
    }

}
