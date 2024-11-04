using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class NextLevelScript : MonoBehaviour
{
    public PlayableDirector playableDirector;
    private MenuScript menuScript;

    private void Start()
    {
        menuScript = FindObjectOfType<MenuScript>();
    }
    public void NextLevel()
    {
        playableDirector.Play();
    }

    public void TransitionNextLevel()
    {
        SceneTransition.Instance.DissolveNextLevel();
    }
}
