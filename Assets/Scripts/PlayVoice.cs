using Crosstales.RTVoice;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Fajar
/// Class terpisah agar pengaturan voice ga numpuk semua di conversation speech
/// Note: voice dipanggil dari sejak splash scene lewat object VoiceInitalizer dan RTVoice
/// agar voice yang dimodif bisa diputar sejak dialog dipanggil di scene gameplay
/// </summary>

public class PlayVoice : MonoBehaviour
{
   

    [Header("Text to Speech")]
    //public Speaker speaker;
    public Button textToSpeechButton;
    public Speaker speaker;
    public string voiceName;
    public string speechTextTest;

    public TMP_Text showVoices;
    public enum VoiceMode { random, native, simple };
    public VoiceMode initialMode = VoiceMode.simple;

    // parameter pitch dan speed rate tiap mode bisa diatur di editor
    [Header("Random Mode Setting")]
    [Range(0.5f, 1.5f)]
    public float pitchMax = 0.75f;
    [Range(0.5f, 1.5f)]
    public float pitchMin = 1.5f;

    [Range(0.0f, 2.0f)]
    public float rateMax = 0.75f;
    [Range(0.0f, 2.0f)]
    public float rateMin = 1.5f;

    [Header("Simple Mode Setting")]
    [Range(0.0f, 2.0f)]
    public float pitchMaxSim = 1.2f;
    [Range(0.5f, 1.5f)]
    public float pitchMinSim = 0.9f;

    [Range(0.0f, 2.0f)]
    public float rateSim = 0.9f;


    //Fungsi utama memanggil TTS dalam 3 mode
    //native, voice menggunakan pengaturan sistem
    //random, voice menggunakan model voice (sesuai yang tersedia di sistem), pitch, dan speed rate acak
    //simple, yang dipakai. Voice default, pitch sedikit acak, speed fixed diperlambat
    //mode default simple
    public void PlaySpeech(string speech, VoiceMode mode = VoiceMode.simple)
    {
        if (speaker.areVoicesReady)
        {
            List<Crosstales.RTVoice.Model.Voice> voicesList = speaker.VoicesForLanguage(SystemLanguage.English);

            float rate;
            float pitch;
            int voice;

            if (mode == VoiceMode.random)
            {

                rate = CustomRandomizer(rateMin, rateMax);
                pitch = CustomRandomizer(pitchMax, pitchMin);

                voice = Random.Range(0, voicesList.Count);

                speaker.Speak(speech, null, voicesList[voice], true, rate, pitch);


                print(
                    "spoken text: " + speech + "\n" +
                    "voice: " + voicesList[voice] + "\n" +
                    "volume: " + rate + "\n" +
                    "pitch: " + pitch
                );


            }
            else if (mode == VoiceMode.simple)
            {
                rate = rateSim;
                pitch = CustomRandomizer(pitchMaxSim, pitchMinSim);
                
                //voice = Random.Range(0, (int) Mathf.Clamp(voicesList.Count, 1,2));



                //speaker.Speak(speech, null, voicesList[voice], true, rate, pitch);
                speaker.Speak(speech, null, speaker.VoiceForName(speaker.DefaultVoiceName), true, rate, pitch);


                print(
                    "spoken text: " + speech + "\n" +
                    "voice: " + speaker.DefaultVoiceName + "\n" +
                    "volume: " + rate + "\n" +
                    "pitch: " + pitch
                );


            }
            else if (mode == VoiceMode.native)
            {
                Speaker.Instance.SpeakNative(speech);
            }

        }
        else
        {
            Speaker.Instance.SpeakNative(speech);
            Debug.Log("Voice not ready");
        }
    }


    //Legacy, buat manggil dialog setelah voice selesai di-load
    //private void WaitForVoices()
    //{
    //    LoadMainDialog();
    //}


    //Hanya buat tes OnVoiceLoad
//    private void CheckAndShowVoices()
//    {
//        string voicesList = "";

//        //if (speaker.VoicesForLanguage(SystemLanguage.English).Count > 0)
//        //{
//        //    foreach (var vo in speaker.VoicesForLanguage(SystemLanguage.English))
//        //    {
//        //        voicesList += vo.ToString() + "\n";
//        //    }
//        //}

//        //showVoices.text = voicesList;

//#if ANDROID || UNITY_ANDROID || PLATFORM_ANDROID
//        showVoices.text = "pakai: " + speaker.AndroidEngine.ToString();
//        showVoices.text += "\n ada: " + speaker.Engines;
//        foreach (var vo in speaker.Engines)
//        {
//            voicesList += "\n" + vo;
//        }
//#else
//        showVoices.text = "wendaw"; 
//#endif

//        PlaySpeech(speechTextTest, initialMode);


//    }

    // Legacy buat tes mode-mode voice
    public void Button1Speech()
    {
        PlaySpeech(speechTextTest, VoiceMode.native);
    }


    public void Button3Speech()
    {
        PlaySpeech(speechTextTest, VoiceMode.simple);
    }
    
    // Pengaturan TTS untuk di speech checker
    public void SetTTSButton(string speech)
    {
        // Kosongkan semua listener. Kalau tombol TTS menghasilkan suara dobel, di sini masalahnya
        textToSpeechButton.onClick.RemoveAllListeners();
        textToSpeechButton.transform.parent.gameObject.SetActive(true);
        textToSpeechButton.onClick.AddListener(delegate { PlaySpeech(speech, initialMode); });
    }

    public void HideTTSButton()
    {
        // Kosongkan semua listener. Kalau tombol TTS menghasilkan suara dobel, di sini masalahnya
        textToSpeechButton.onClick.RemoveAllListeners();
        textToSpeechButton.transform.parent.gameObject.SetActive(false);
    }

    // Fungsi random tambahan
    public float CustomRandomizer(float min, float max)
    {
        float output = 0f;

        if (max < min)
        {
            output = Random.Range(max, min);
        }
        else if (min < max)
        {
            output = Random.Range(min, max);
        }
        else if (max == min)
        {
            output = max;
        }

        return output;
    }
}
