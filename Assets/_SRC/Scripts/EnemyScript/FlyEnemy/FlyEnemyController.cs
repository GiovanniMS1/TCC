using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyEnemyController : MonoBehaviour
{
    private Transform playerTransform;
    public float distanceBetweenPlayer;
    private Vector3 initialPoint;
    private Animator anim;

    private void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        anim = GetComponent<Animator>();
        initialPoint = transform.position;
    }

    private void Update()
    {
        CalculateDistance();
        AnimationState();
    }

    private void CalculateDistance()
    {
        distanceBetweenPlayer = Vector2.Distance(transform.position, playerTransform.position);
    }

    public Vector3 GetInitialPoint()
    {
        return initialPoint;
    }

    public void FlipSprite(Vector3 target)
    {
        if(transform.position.x < target.x)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    private void AnimationState()
    {
        anim.SetFloat("Distance", distanceBetweenPlayer);
    }
}
