using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class EnumerateText : MonoBehaviour
{
    public float characterIntervalTime = 0.1f;

    private TextMeshProUGUI text;

    private bool running = false;
    [SerializeField]private string targetString;
    [SerializeField]private string currentString;

    private int currentTextIndex = 0;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        StartCoroutine(TextLoop());
    }

    IEnumerator TextLoop()
    {
        while(true)
        {
            if(running == true)
            {
                while(currentTextIndex < targetString.Length)
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

    public void SetEmpty()
    {
        text.text = "";
    }

    public void TextFade(float duration)
    {
        text.DOFade(0f, duration);
    }

    public void Init()
    {
        SetEmpty();
        text.DOFade(1f, 0f);
    }
}
