using UnityEngine;

public class InstructionsPopup : MonoBehaviour
{
    public GameObject instructionsPanel;


    public void ShowPanel()
    {
        instructionsPanel.SetActive(true);
    }

   
    public void HidePanel()
    {
        instructionsPanel.GetComponent<Animator>().SetTrigger("shrink");
        Invoke("disablePanel", 0.3f);
    }

    void disablePanel(){
        instructionsPanel.SetActive(false);
    }
}
