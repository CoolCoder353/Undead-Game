using UnityEngine;
using UnityEngine.UI;


public class TimeCounter : MonoBehaviour
{
    public static TimeCounter Instance;

    public Text timeText;

    public float time = 0f;

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

    void Update()
    {
        time += Time.deltaTime;
        timeText.text = time.ToString("F2");
    }


}