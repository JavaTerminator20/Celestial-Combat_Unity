using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    PlayerController player;
    OponentController oponent;
    Transform playerTransform;
    Transform oponentTransform;

    void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        oponent = FindFirstObjectByType<OponentController>();
        playerTransform = player.GetComponent<Transform>();
        oponentTransform = oponent.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {   
        //koda ki uprvalja orientacijo obeh igralcev - ju obraca drug proti drugemu
        if (player.grounded){
            if (playerTransform.position.x < oponentTransform.position.x){
                player.orientation = 1;
                oponent.orientation = -1;
                playerTransform.rotation = Quaternion.Euler(0, 90, 0);
                oponentTransform.rotation = Quaternion.Euler(0, -90, 0);
            } else{
                player.orientation = -1;
                oponent.orientation = 1;
                playerTransform.rotation = Quaternion.Euler(0, -90, 0);
                oponentTransform.rotation = Quaternion.Euler(0, 90, 0);
            }
        }
        
    }
}
