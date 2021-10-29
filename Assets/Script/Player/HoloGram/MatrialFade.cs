using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatrialFade : MonoBehaviour
{
    [SerializeField] private List<Renderer> renderers = new List<Renderer>();
    private List<Material> mats = new List<Material>();

    private void Awake()
    {
        for(int i = 0; i <renderers.Count; i++)
        {
            mats.Add(renderers[i].material);
        }

    }

    private IEnumerator Fade(float target)
    {
        float current = mats[0].GetFloat("FadeAmount");
        while(current != target)
        {
            current = Mathf.MoveTowards(current, target, 2f * Time.deltaTime);
            for(int i = 0; i < mats.Count; i++)
            {
                mats[i].SetFloat("FadeAmount", current);
            }
            yield return null;
        }
    }

    public void FadeIn()
    {
        StartCoroutine(Fade(0f));
    }

    public void FadeOut()
    {
        StartCoroutine(Fade(1f));
    }
}
