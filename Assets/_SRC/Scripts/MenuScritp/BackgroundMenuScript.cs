using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMenuScript : MonoBehaviour
{
    [SerializeField] private Vector2 speedParallax;
    private Vector2 offset;
    private Material material;

    private void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;
    }

    private void FixedUpdate()
    {
        offset = speedParallax * Time.deltaTime;
        material.mainTextureOffset += offset;
    }
}
