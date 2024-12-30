using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    public TMP_Text timerText;
    public float timeElapsed; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timeElapsed = 0.0f;
    }


    // Update is called once per frame
    void Update()
    {
		// Increment elapsed time
		timeElapsed += Time.deltaTime;

		// Format the time as MM:SS and display it
		int minutes = Mathf.FloorToInt(timeElapsed / 60);
		int seconds = Mathf.FloorToInt(timeElapsed % 60);
		timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
