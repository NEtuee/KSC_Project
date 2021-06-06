using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class OutTroCtrl : MonoBehaviour
{
    public Image thankYouImage;
    private Material thMat;

    public TextBaseButtonUi button;

    private float _startValue = 0.1f;
    private float _endValue = 0.0f;

    void Start()
    {
        thMat = thankYouImage.material;
        thMat.SetFloat("_DispProbability", _startValue);

        button.Active(false);

        StartCoroutine(Process());
    }

    void Update()
    {
        
    }

    IEnumerator Process()
    {
        yield return new WaitForSeconds(2f);
        //thMat.SetFloat("_DispProbability", 0.0f);
        float value = _startValue;
        DOTween.To(() => value, x => value = x, _endValue, 2f)
            .OnUpdate(() => thMat.SetFloat("_DispProbability", value));


        yield return new WaitForSeconds(1f);
        button.Active(true);
    }

    public void GoToTitle()
    {
        SceneManager.LoadScene(1);
    }
}
