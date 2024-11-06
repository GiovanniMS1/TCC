using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public Button[] buttons;
    public GameObject levelButtons;
    private void Start()
    {
        UpdateButtonIntecartivity();
    }

    private void UpdateButtonIntecartivity()
    {
        ButtonsToArray();

        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 0);

        for(int i = 0; i < buttons.Length; i++)
        {
            buttons[i].interactable = false;
        }
        for(int i = 0; i < unlockedLevel; i++)
        {
            buttons[i].interactable = true;
        }
    }

    void ButtonsToArray()
    {
        int childCount = levelButtons.transform.childCount;
        buttons = new Button[childCount];
        for(int i = 0; i < childCount; i++)
        {
            buttons[i] = levelButtons.transform.GetChild(i).gameObject.GetComponent<Button>();
        }
    }

    public void LockAllLevels()
    {
        PlayerPrefs.SetInt("UnlockedLevel", 0);
        PlayerPrefs.Save();
    }
}

    
