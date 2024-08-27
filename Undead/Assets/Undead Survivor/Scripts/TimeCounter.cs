using UnityEngine;
using UnityEngine.UI;


public class TimeCounter : MonoBehaviour
{
    public static TimeCounter Instance;

    public Text timeText;
    public bool isCounting = true;

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
        if (!isCounting) { return; }
        time += Time.deltaTime;
        timeText.text = time.ToString("F2");
    }


}