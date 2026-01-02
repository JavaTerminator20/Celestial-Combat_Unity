using System;
using UnityEngine;

public class followPlayers : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public PlayerController player;
    public OponentController oponent;

    // Update is called once per frame
    float midPoint;
    void Update()
    {
        midPoint = (player.transform.position.x + oponent.transform.position.x)/2;
        var newPos = transform.position;
        newPos.x = midPoint;
        transform.position = newPos;
    }
}
