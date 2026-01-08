using UnityEngine;

public class InstructionsPopup : MonoBehaviour
{
   public PopupController instructionsPopup; 

  
    public void ShowPanel()
    {
        if (instructionsPopup != null)
            instructionsPopup.ShowPopup();
        else
            Debug.LogWarning("InstructionsPopup reference not set!");
    }

  
    public void HidePanel()
    {
        if (instructionsPopup != null)
            instructionsPopup.HidePopup();
    }
}
