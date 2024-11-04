using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SavePrefs : MonoBehaviour
{
    float volume = 0;
    
    //string resolucao = "1080";
    int score = 1;
  
    private void SavePref()
    {
        PlayerPrefs.SetFloat("volume", volume);
        //PlayerPrefs.SetString("volume", resolucao);
        PlayerPrefs.SetInt("volume", score);
    }

    private void LoadPref()
    {
        volume = PlayerPrefs.GetFloat("volume", 0);
        //resolucao = PlayerPrefs.GetString("volume", "1080");
        score = PlayerPrefs.GetInt("volume", 1);
    }
}
