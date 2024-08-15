using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextFader : MonoBehaviour
{
    private Color startColor;
    private Color endColor;
    private bool startFading = false;
    private float timeCounter;

    public float fadingSpeed = 0.05f;
    public bool fadeIn = true;

    void Start()
    {
        if (fadeIn)
        {
            endColor = gameObject.GetComponent<Text>().color;
            startColor = endColor;
            startColor.a = 0f;
        }
        else // fadeOut
        {
            startColor = gameObject.GetComponent<Text>().color;
            endColor = startColor;
            endColor.a = 0f;
        }

        fade();
    }

    void fade()
    {
        timeCounter = 0f;
        startFading = true;
    }

    void Update()
    {
        if (startFading)
        {
            timeCounter += Time.deltaTime;
            gameObject.GetComponent<Text>().color = Color.Lerp(startColor, endColor, fadingSpeed * timeCounter);
        }
    }
}
