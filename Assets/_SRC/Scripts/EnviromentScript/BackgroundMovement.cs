using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMovement : MonoBehaviour
{
    [SerializeField] private Camera cameraPlayer;
    [SerializeField] private float speedParallax;
    private float lenght, startPos;

    void Start()
    {
        startPos = transform.position.x;
        lenght = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    void FixedUpdate()
    {
        float temp = (cameraPlayer.transform.position.x * (1 - speedParallax));
        float dist = (cameraPlayer.transform.position.x * speedParallax);

        transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);

        if(temp > startPos + lenght / 2)
        {
            startPos += lenght;
        }
        else if(temp < startPos - lenght / 2)
        {
            startPos -= lenght;
        }
    }
}
