using UnityEngine;
using UnityEngine.UI;

public class MenuScript : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;
    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        AudioManager.Instance.InitializeSliders(musicSlider, sfxSlider);
    }

    public void Play(int levelIndex)
    {
        SceneTransition.Instance.DissolveExit(levelIndex);
    }

    public void Quit()
    {
        SceneTransition.Instance.DissolveExitGame();
    }

    public void UpdateMusicVolume(float volume)
    {
        AudioManager.Instance.UpdateMusicVolume(volume);
    }

    public void UpdateSoundVolume(float volume)
    {
        AudioManager.Instance.UpdateSFXVolume(volume);
    }

    public void SaveVolume()
    {
        AudioManager.Instance.SaveVolumeSettings();
    }
}