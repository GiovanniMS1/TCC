using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameOverScript : MonoBehaviour
{
    public GameObject menuGameOver;
    private PlayerLife playerLife;

    private void Start()
    {
        playerLife = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLife>();
        playerLife.playerDeath += ActiveMenu;
    }

    private void ResetPhysic()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Enemy"), false);
    }
    private void ActiveMenu(object sender, EventArgs e)
    {
        menuGameOver.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Restart()
    {
        ResetPhysic();
        playerLife.playerDeath -= ActiveMenu;
        SceneTransition.Instance.DissolveExit(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackMenu()
    {
        ResetPhysic();
        playerLife.playerDeath -= ActiveMenu;
        SceneTransition.Instance.DissolveExit(0);
    }
}