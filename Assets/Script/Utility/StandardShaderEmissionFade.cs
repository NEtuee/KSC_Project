using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandardShaderEmissionFade : MonoBehaviour
{
    private Renderer renderer;
    private Material mat;
    [SerializeField] private float startIntencity = 2.5f;
    [SerializeField] private float targetIntencity = -1.0f;
    [SerializeField] private float speed = 1f;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
        mat = renderer.material;
    }

    public void StartFade()
    {
        StartCoroutine(Fade());
    }

    IEnumerator Fade()
    {
        float intencity = startIntencity;
        Color emissionColor = mat.GetColor("_EmissionColor");

        //Debug.Log(emissionColor);

        while(intencity > targetIntencity)
        {
            mat.SetColor("_EmissionColor", emissionColor * intencity);
            intencity = Mathf.MoveTowards(intencity, targetIntencity, speed * Time.deltaTime);
            yield return null;
        }
    }
}
