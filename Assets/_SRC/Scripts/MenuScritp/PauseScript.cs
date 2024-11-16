using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseScript : MonoBehaviour
{
    public GameObject pauseMenu;
    public static bool paused;
    public Slider musicSlider;
    public Slider sfxSlider;
    private PlayerBehaviour playerBehaviour;

    private void Start()
    {
        playerBehaviour = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
        SetPauseMenu(false);
        AudioManager.Instance.InitializeSliders(musicSlider, sfxSlider);
    }

    private void ChangeCursor()
    {
        if(paused)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    public void BlockPlayerInput()
    {
        playerBehaviour.DisablePlayerControl();
    }

    public void AllowPlayerInput()
    {
        playerBehaviour.EnablePlayerControl();
    }

    public void SetPauseMenu(bool isPaused)
    {
        paused = isPaused;
        Time.timeScale = paused ? 0f : 1f;
        pauseMenu.SetActive(paused);
        ChangeCursor();
    }

    public void Reset()
    {
        Time.timeScale = 1f;
        SceneTransition.Instance.DissolveExit(SceneManager.GetActiveScene().buildIndex);
        BlockPlayerInput();
    }

    public void BackMenu()
    {
        Time.timeScale = 1f;
        SceneTransition.Instance.DissolveExit(0);
        BlockPlayerInput();
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