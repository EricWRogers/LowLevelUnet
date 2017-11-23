using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour {
    public Text timerText;
    private float startTime;
	// Use this for initialization
	void Start () {
        startTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
        float t = Time.time - startTime;
        

        string minutes = ((int)t / 60).ToString();
        float s = (t % 60);
        string seconds = s.ToString("f0");
        //timerText.text = minutes + ":" + seconds;
        if (s <= 9)
        {
            timerText.text = minutes + ":0" + seconds;
        }
        if (s > 10)
        {
            timerText.text = minutes + ":" + seconds;
        }
        
		
	}
}
