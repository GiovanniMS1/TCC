using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseScript : MonoBehaviour
{
    public GameObject pauseMenu;
    public static bool paused;
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public Slider sfxSlider;
    private PlayerBehaviour playerBehaviour;

    void Start()
    {
        playerBehaviour = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
        SetPauseMenu(false);
        LoadVolume();
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
        Time.timeScale = paused ? 0 : 1;
        pauseMenu.SetActive(paused);
        ChangeCursor();
    }

    public void Reset()
    {
        Time.timeScale = 1;
        SceneTransition.Instance.DissolveExit(SceneManager.GetActiveScene().buildIndex);
        BlockPlayerInput();
    }

    public void BackMenu()
    {
        Time.timeScale = 1;
        SceneTransition.Instance.DissolveExit(0);
        BlockPlayerInput();
    }

    public void UpdateMusicVolume(float volume)
    {
        audioMixer.SetFloat("MusicVolume", volume);
    }

    public void UpdateSoundVolume(float volume)
    {
        audioMixer.SetFloat("SFXVolume", volume);
    }

    public void SaveVolume()
    {
        audioMixer.GetFloat("MusicVolume", out float musicVolume);
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);

        audioMixer.GetFloat("SFXVolume", out float sfxVolume);
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
    }

    public void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume");
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume");
    }
    
}
