using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public void Play()
    {
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Debug.Log("Saiu do jogo!");
        Application.Quit();
    }
}
