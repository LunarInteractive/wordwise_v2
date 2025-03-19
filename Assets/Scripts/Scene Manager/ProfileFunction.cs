using UnityEngine;

/// <summary> Fajar
/// Fungsi-fungsi operasional di scene 'profile'.
/// Juga menjadi perantara antara script terkait UI 
/// dengan koneksi database
/// </summary>
public class ProfileFunction : MonoBehaviour
{
    [SerializeField] UpdateClass updateClass;
    [SerializeField] GameObject transition;
    [SerializeField] SceneManagerProfile sceneManagerProfile;
    public void SignOut()
    {
        UserDataSession.CloseSession();
    }

    public void UpdateClass()
    {
        updateClass.StartConnecting();
    }

    public void TransitionStart()
    {
        transition.SetActive(true);
    }

    public void AddClass(string className)
    {
        sceneManagerProfile.AddClass(className);
    }
}
