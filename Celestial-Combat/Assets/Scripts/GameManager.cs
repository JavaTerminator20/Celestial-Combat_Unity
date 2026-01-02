using UnityEngine;
using System.Collections.Generic;
using Unity.Cinemachine;
using System.Transactions;
using UnityEditor.Experimental.GraphView;


public class GameManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    PlayerController player;
    OponentController oponent;
    Transform playerTransform;
    Transform oponentTransform;
    public CinemachineImpulseSource impulseSource;

    
    
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

    float maxDist = 8;
    float borderL = -13f;
    float borderR = 13f;

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

        // dolocanje maximalne razdalje med igralcema
        float excess = Mathf.Abs(playerTransform.position.x - oponentTransform.position.x) - maxDist;
        if (Mathf.Abs(playerTransform.position.x - oponentTransform.position.x) > maxDist){
            playerTransform.position += new Vector3(excess*player.orientation, 0, 0);      //samo playerja popravi (nasprotnik je le Bot)
            //oponentTransform.position += new Vector3(excess*oponent.orientation, 0, 0);
        }

        // meje levo-desno
        var playerPos = playerTransform.position;
        playerPos.x = Mathf.Min(Mathf.Max(playerPos.x, borderL), borderR);
        playerTransform.position = playerPos;

        var oponentPos = oponentTransform.position;
        oponentPos.x = Mathf.Min(Mathf.Max(oponentPos.x, borderL), borderR);
        oponentTransform.position = oponentPos;

        float dist = Mathf.Abs(playerPos.x - oponentPos.x);
        Debug.Log("Punch hitbox distance: " + dist);


    }

    public Transform getPlayerTransform(){return playerTransform;}
    public Transform getOponentTransform(){return oponentTransform;}

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

    //funkcija ki jo poklice PlayerController.cs (v metodu Ult al neki)
    public IEnumerator<WaitForSecondsRealtime> sustainedShake(int iterations){
        for (int i = 0; i < iterations; i++){
            // Vector3 impulse = new Vector3(          // TODO: naredi da bo strength pa yStrength randomizrial (-/+ smer)
            // Random.Range(-0.2f, 0.2f),              // tiny horizontal variation
            // Random.Range(-yStrength, yStrength),
            // 0f);
            float lowerB = 0.05f;
            float upperB = 0.1f;
            float strengthX = Random.Range(lowerB, upperB);
            int sign1 = Random.Range(-1, 1) > 0 ? 1 : -1;
            int sign2 = Random.Range(-1, 1) > 0 ? 1 : -1;
            float strengthY = Random.Range(lowerB, upperB);
            Vector3 impulse = new Vector3(strengthX*sign1, strengthY*sign2, 0f);
            impulseSource.GenerateImpulse(impulse);
            yield return new WaitForSecondsRealtime(0.4f);
        }
    }

    //funkcija ki jo poklice bigStomp.cs (ko sonce udari z nogo ob tla)
    public void StompCameraShake(){
        StartCoroutine(ActualStomp());   
    }

    private IEnumerator<WaitForSecondsRealtime> ActualStomp(){
        Debug.Log("stomp camera shake");
        Vector3 impulse = new Vector3(0.15f, -0.7f, 0f);
        impulseSource.GenerateImpulse(impulse);
        oponent.SunUltFloating();
        yield return new WaitForSecondsRealtime(0.4f);
        impulseSource.GenerateImpulse(impulse);
    }
}
