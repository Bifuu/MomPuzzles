using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProgressBar : MonoBehaviour {

    public Transform Foreground;
    public Transform Background;
    public Image ForeLeft;
    public Image ForeCenter;
    public Image ForeRight;

    public float FillPercent { get; set; }

    private float totalWidth;
    private float leftWidth;
    private float centerWidth;
    private float rightWidth;


    // Use this for initialization
    void Start()
    {
        leftWidth = ForeLeft.rectTransform.sizeDelta.x;
        centerWidth = GetComponent<RectTransform>().sizeDelta.x;
        rightWidth = ForeRight.rectTransform.sizeDelta.x;
        totalWidth = leftWidth + centerWidth + rightWidth;
        Debug.Log(leftWidth + ", " + centerWidth + "," + rightWidth + ": " + totalWidth);
    }
	// Update is called once per frame
	void Update () {
        float fillTotal = FillPercent * totalWidth;

        if (fillTotal <= leftWidth)
        {
            ForeLeft.fillAmount = (fillTotal / leftWidth);
            ForeCenter.fillAmount = 0;
            ForeRight.fillAmount = 0;
        }
        else if (fillTotal <= leftWidth + centerWidth)
        {
            ForeLeft.fillAmount = 1;
            ForeCenter.fillAmount = (fillTotal - leftWidth) / centerWidth;
            ForeRight.fillAmount = 0;
        }
        else
        {
            ForeLeft.fillAmount = 1;
            ForeCenter.fillAmount = 1;
            ForeRight.fillAmount = (fillTotal - leftWidth - centerWidth) / rightWidth;
        }
	}
}
