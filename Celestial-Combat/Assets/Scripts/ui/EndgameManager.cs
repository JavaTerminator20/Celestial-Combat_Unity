using UnityEngine;

public class EndgameManager : MonoBehaviour
{
    public GameObject popupUI;   
    public Animator popupAnimator;  

    void Awake() {
        if (popupAnimator == null)
            popupAnimator = popupUI.GetComponent<Animator>();
    }

    public void ShowLose()
    {
        popupUI.SetActive(true);
        popupAnimator.Update(0f); 
        popupAnimator.Play("YouLose"); 
    }

    public void ShowWin()
    {
        popupUI.SetActive(true);
        popupAnimator.Update(0f); 
        popupAnimator.Play("YouWin"); 
    }

    public void HidePopup()
    {
        popupUI.SetActive(false);
    }
}