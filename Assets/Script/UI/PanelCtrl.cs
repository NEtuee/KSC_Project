using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelCtrl : MonoBehaviour
{
    [SerializeField] private bool startDisable;

    [SerializeField] private List<Image> panelImageList = new List<Image>();
    [SerializeField] private List<Text> textList = new List<Text>();

    private void Start()
    {
        if(startDisable == true)
        {
            foreach(var image in panelImageList)
            {
                Color color = image.color;
                color.a = 0f;
                image.color = color;
            }

            foreach(var text in textList)
            {
                Color color = text.color;
                color.a = 0f;
                text.color = color;
            }
        }
    }

    public IEnumerator FadeIn()
    {
        float alpha = 0.0f;

        while(alpha < 1f)
        {
            alpha += 1.5f * Time.unscaledDeltaTime;

            foreach (var image in panelImageList)
            {
                Color color = image.color;
                color.a = alpha;
                image.color = color;
            }

            foreach (var text in textList)
            {
                Color color = text.color;
                color.a = alpha;
                text.color = color;
            }

            yield return null;
        }
    }
}
