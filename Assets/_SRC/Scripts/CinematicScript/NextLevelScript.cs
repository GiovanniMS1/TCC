using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class NextLevelScript : MonoBehaviour
{
    public void TransitionNextLevel()
    {
        SceneTransition.Instance.DissolveNextLevel();
    }

    public void BackToMenu()
    {
        SceneTransition.Instance.DissolveExit(0);
    }

}
