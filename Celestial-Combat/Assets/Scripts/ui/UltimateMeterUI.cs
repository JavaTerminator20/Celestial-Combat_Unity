using UnityEngine;
using UnityEngine.UI;

public class UltimateMeterUI : MonoBehaviour
{
    public Image fillImage;          // The UI fill image
    public CharacterBase character;  // The character this bar represents

    private float maxUltimate = 10f; // Full meter at 10

    void Update()
    {
        if (character == null || fillImage == null)
            return;

        float fill = Mathf.Clamp01(character.ultimateMeter / maxUltimate);
        fillImage.fillAmount = fill;
    }
}