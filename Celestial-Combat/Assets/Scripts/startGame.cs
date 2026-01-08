using UnityEngine;

public class startGame : MonoBehaviour
{
    public GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartGame(){
        gameManager.StartGame();
        gameObject.SetActive(false);
    }
}
