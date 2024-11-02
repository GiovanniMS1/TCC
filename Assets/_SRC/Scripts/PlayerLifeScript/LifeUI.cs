using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LifeUI : MonoBehaviour
{
    public List<Image> heartList;
    public GameObject heartPrefab;
    public PlayerLife playerLife;
    public int actualIndex;
    public Sprite fullHeart;
    public Sprite emptyHeart;

    private void Awake()
    {
        playerLife.changeLife.AddListener(ChangeHeart);
    }

    private void ChangeHeart(int actualLife)
    {
        if(!heartList.Any())
        {
            CreateHeart(actualLife);
        }
        else
        {
            ChangeLife(actualLife);
        }
    }

    private void CreateHeart(int quantityMaxlLife)
    {
        for(int i = 0; i < quantityMaxlLife; i++)
        {
            GameObject heart = Instantiate(heartPrefab, transform);

            heartList.Add(heart.GetComponent<Image>());
        }

        actualIndex = quantityMaxlLife - 1;
    }

    private void ChangeLife(int actualLife)
    {
        if(actualLife <= actualIndex)
        {
            RemoveHeart(actualLife);
        }
        else
        {
            AddHeart(actualLife);
        }
    }

    private void RemoveHeart(int actualLife)
    {
        for(int i = actualIndex; i >= actualLife; i--)
        {
            actualIndex = i;
            heartList[actualIndex].sprite = emptyHeart;
        }
    }

    private void AddHeart(int actualLife)
    {
        for(int i = actualIndex; i < actualLife; i++)
        {
            actualIndex = i;
            heartList[actualIndex].sprite = fullHeart;
        }
    }
}