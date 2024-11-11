using UnityEngine;

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
