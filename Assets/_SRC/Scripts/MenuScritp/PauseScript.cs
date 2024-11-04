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
    }

    public void BackMenu()
    {
        Time.timeScale = 1;
        SceneTransition.Instance.DissolveExit(0);
    }
    
}
