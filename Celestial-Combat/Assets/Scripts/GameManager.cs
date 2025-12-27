using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;


public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    PlayerController player;
    OponentController oponent;
    Transform playerTransform;
    Transform oponentTransform;
    static CinemachineImpulseSource impulseSource;
    
    //limit game to 120fps
    void Awake()
    {
        QualitySettings.vSyncCount = 1;   
        Application.targetFrameRate = 120; // Cap to 120 FPS
    }

    void Start()
    {
        player = FindFirstObjectByType<PlayerController>();
        oponent = FindFirstObjectByType<OponentController>();
        playerTransform = player.GetComponent<Transform>();
        oponentTransform = oponent.GetComponent<Transform>();
        impulseSource = GetComponent<CinemachineImpulseSource>();

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

    public void CameraShake(float strength, bool block){
        //Debug.Log("camera shake");
        float yStrength = 0.25f;
        if (block){
            yStrength = 0f;
        }   
        // Vector3 impulse = new Vector3(          // TODO: naredi da bo strength pa yStrength randomizrial (-/+ smer)
        // Random.Range(-0.2f, 0.2f),              // tiny horizontal variation
        // Random.Range(-yStrength, yStrength),
        // 0f);
        Vector3 impulse = new Vector3(strength, yStrength, 0f);
        impulseSource.GenerateImpulse(impulse);
    }
}
