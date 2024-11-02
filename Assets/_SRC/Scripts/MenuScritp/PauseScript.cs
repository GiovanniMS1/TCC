using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
    public GameObject pauseMenu;
    public static bool paused;

    void Start()
    {
        SetPauseMenu(false);
    }

    public void SetPauseMenu(bool isPaused)
    {
        paused = isPaused;
        Time.timeScale = paused ? 0 : 1;
        pauseMenu.SetActive(paused);
    }

    public void Reset()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackMenu(string name)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(name);
    }
    
}
