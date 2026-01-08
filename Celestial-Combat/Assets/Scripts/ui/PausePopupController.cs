using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class PausePopupController : MonoBehaviour
{
 
    public PopupController pauseMenuUI;    


 
    public void Pause()
    {
        
        Time.timeScale = 0f;
        pauseMenuUI.ShowPopup();

     

      
       
    }

   
    public void Resume()
    {
       

        Time.timeScale = 1f; 
        pauseMenuUI.HidePopup();
    }

    
 

   
    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

   
    public void MainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); 
    }
}
