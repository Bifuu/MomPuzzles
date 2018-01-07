using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageNumber : MonoBehaviour {

    public float minRotation = -40.0f;
    public float maxRotation = 40.0f;
    public int minSize = 40;
    public int maxSize = 80;
    public float fadeTime = 1.0f;
    public float travelDistance = 20.0f;
    float speed = 10.0f;

    Text text;
    RectTransform rectT;
    float startTime;
    Vector2 StartPos;
    Vector2 EndPos;
    bool Animating = false;

	// Use this for initialization
	void Start () {
        text = GetComponent<Text>();
        rectT = GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Animating)
        {
            float currTime = (Time.time - startTime) / (fadeTime);
            rectT.anchoredPosition = Vector2.Lerp(StartPos, EndPos, currTime);
            text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Lerp(1, 0, currTime));

            if (currTime > 1)
                Animating = false;
        }
        
	}

    public void Show(string displayedString, bool useLargestFontSize)
    {
        Show(displayedString, useLargestFontSize, Color.red, true);
    }

    public void Show(string displayedString, bool useLargestFontSize, Color textColor, bool randomPosition)
    {
        //reset
        rectT.rotation = Quaternion.identity;
        rectT.anchoredPosition = Vector2.zero;

        text.text = displayedString;
        text.fontSize = useLargestFontSize ? maxSize : Random.Range(minSize, maxSize + 1);
        text.color = textColor;

        if (randomPosition)
        {
            float rndRot = Random.Range(minRotation, maxRotation);
            float width = rectT.sizeDelta.x / 2.0f * -1;
            float height = rectT.sizeDelta.y / 2.0f * -1;
            //Debug.Log(rndRot + ": " + rectT.anchoredPosition + ": " + width + " " + height);
            Quaternion qRot = new Quaternion();
            qRot.eulerAngles = new Vector3(0, 0, rndRot);
            rectT.rotation = qRot;
            rectT.anchoredPosition = new Vector2(Random.Range(width * -1, width), Random.Range(height * -1, height));
        }

        startTime = Time.time;
        StartPos = rectT.anchoredPosition;
        EndPos = rectT.anchoredPosition + new Vector2(0, travelDistance);
        //Debug.Log("Start: " + StartPos + " End: " + EndPos);
        Animating = true;
    }
}
