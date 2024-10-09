using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMovement : MonoBehaviour
{
    [SerializeField] private Vector2 velocityMovement;
    private Vector2 offset;
    private Material material;
    private Rigidbody2D playerMovement;

    private void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        offset = (playerMovement.velocity.x * 0.1f) * velocityMovement * Time.deltaTime;
        material.mainTextureOffset += offset;
    }
}
