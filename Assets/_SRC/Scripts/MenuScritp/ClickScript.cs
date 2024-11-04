using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickScript : MonoBehaviour
{
    public void PlayClickSound(string SFXName)
    {
        SoundManager.Instance.PlaySound2D(SFXName);
    }
}
