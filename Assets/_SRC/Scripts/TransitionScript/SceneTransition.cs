using System.Collections;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;
    private PlayerBehaviour playerBehaviour;
    [Header("Dissolve")]
    public CanvasGroup dissolveCanvasGroup;
    public float timeToDissolveEntry;
    public float timeToDissolveExit;
    public string musicLevel;
    public float waitTimeToPlayMusic;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(DissolveEntry());
        GetPlayerComponent();
    }

    private void GetPlayerComponent()
    {
        if(playerBehaviour == null)
        {
            return;
        }
        else
        {
            playerBehaviour = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerBehaviour>();
        }
    }
    private IEnumerator DissolveEntry()
    {   
        LeanTween.alphaCanvas(dissolveCanvasGroup, 0f, timeToDissolveEntry).setOnComplete(()=>
        {
            dissolveCanvasGroup.blocksRaycasts = false;
            dissolveCanvasGroup.interactable = false;
        });

        yield return new WaitForSeconds(waitTimeToPlayMusic);

        MusicManager.Instance.PlayMusic(musicLevel);
    }

    public void DissolveExit(int indexScene)
    {
        dissolveCanvasGroup.blocksRaycasts = true;
        dissolveCanvasGroup.interactable = true;

        MusicManager.Instance.StopMusic();

        LeanTween.alphaCanvas(dissolveCanvasGroup, 1f, timeToDissolveExit).setOnComplete(()=>
        {
            SceneManager.LoadScene(indexScene);
        });
    }

    public void DissolveExitGame()
    {
        dissolveCanvasGroup.blocksRaycasts = true;
        dissolveCanvasGroup.interactable = true;

        MusicManager.Instance.StopMusic();

        LeanTween.alphaCanvas(dissolveCanvasGroup, 1f, timeToDissolveExit).setOnComplete(()=>
        {
            Application.Quit();
        });
    }

    public void DissolveNextLevel()
    {
        dissolveCanvasGroup.blocksRaycasts = true;
        dissolveCanvasGroup.interactable = true;

        MusicManager.Instance.StopMusic();

        LeanTween.alphaCanvas(dissolveCanvasGroup, 1f, timeToDissolveExit).setOnComplete(()=>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        });
    }
    
}
