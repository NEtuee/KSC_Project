using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EnumerateText : MonoBehaviour
{
    public float characterIntervalTime = 0.1f;

    private TextMeshProUGUI text;

    private bool running = false;
    private string targetString;
    private string currentString;

    private int currentTextIndex = 0;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        StartCoroutine(TextLoop());
    }

    IEnumerator TextLoop()
    {
        while(true)
        {
            if(running == true)
            {
                while(currentTextIndex + 1 < targetString.Length)
                {
                    currentTextIndex++;
                    currentString = targetString.Substring(0, currentTextIndex);
                    text.text = currentString;

                    yield return new WaitForSeconds(characterIntervalTime);
                }
                running = false;
            }

            yield return null;
        }
    }

    public void SetTargetString(string target)
    {
        running = true;
        targetString = target;
        currentString = "";
        currentTextIndex = 0;
        text.text = currentString;
    }
}
