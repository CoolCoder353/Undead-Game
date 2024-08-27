using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider bar;
    public static HealthBar Instance;


    void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetHealth(float health, float maxHealth)
    {
        bar.value = Mathf.Lerp(bar.value, health / maxHealth, 2f);
    }
}