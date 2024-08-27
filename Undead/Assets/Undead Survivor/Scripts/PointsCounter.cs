using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class PointsCounter : MonoBehaviour
{
    public static PointsCounter Instance;

    public Text pointsText;

    public float time = 1f;


    public float points = 0;

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

    public void AddPoints(float pointsToAdd)
    {
        points = pointsText.text == "" ? 0 : float.Parse(pointsText.text);
        pointsText.text = Mathf.Lerp(points, points + pointsToAdd, time).ToString();

    }

    public void RemovePoints(float pointsToRemove)
    {
        points = pointsText.text == "" ? 0 : float.Parse(pointsText.text);
        pointsText.text = Mathf.Lerp(points, points - pointsToRemove, time).ToString();
    }
}