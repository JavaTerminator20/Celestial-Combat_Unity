using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthBarSlider;   



    public void SetHealth(float health)
    {
        healthBarSlider.value = health;
    }
}
