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
        instructionsPanel.SetActive(false);
    }
}
