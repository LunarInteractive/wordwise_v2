using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
/// <summary> Fajar
/// Class AudioController sudah ada sebelum saya sentuh. Saya modif sedikit dan
/// kedepannya bisa dipakai buat mengatur audio sesuai channel yang tersedia:
/// Master, SFX, Dialog
/// Tapi KAYAKNYA SETTING YANG SAYA BUAT BELUM DI MERGE 
/// mungkin bareng sama branch yang ada setting pilih mic (fajar_baru)
/// Script ini menghubungkan slider di panel setting (main_menu)
/// dengan Player_pref sehingga pengaturan volume akan disimpan jika app dimatikan
/// Yang sudah saya tambahkan sebelumnya adalah pengaturan untuk channel dialog
/// sehingga volume dialog bisa diatur.
/// </summary>
public class AudioController : MonoBehaviour
{
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadVolume();
        }
        else
        {
            SetMusicVolume();
        }

        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            LoadSFXVolume();
        }
        else
        {
            SetSFXVolume();
        }

        if (musicSlider != null) musicSlider.onValueChanged.AddListener(delegate { SetMusicVolume(); });
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(delegate { SetSFXVolume(); });
    }

    public void SetMusicVolume()
    {
        if (musicSlider != null)
        {
            float volume = musicSlider.value;
            myMixer.SetFloat("music", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("musicVolume", volume);
        }
    }
    public void SetSFXVolume()
    {
        if (sfxSlider != null)
        {
            float volume = sfxSlider.value;
            myMixer.SetFloat("sfx", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("sfxVolume", volume);
        }
    }

    private void LoadVolume()
    {
        if (musicSlider != null) 
        {
            musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
            SetMusicVolume();
        }
    }
    private void LoadSFXVolume()
    {
        if (sfxSlider != null)
        {
            sfxSlider.value = PlayerPrefs.GetFloat("sfxVolume");
            SetSFXVolume();
        }
    }

}
