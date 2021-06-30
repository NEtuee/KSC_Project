using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FilledTest : MonoBehaviour
{
    private Image image;



    void Start()

    {

        image = GetComponent<Image>();

    }



    bool isFill = false;

    float timer = 0f;



    void Update()

    {

        if (timer >= 1f)

        {

            timer = 0f;

            isFill = !isFill;

        }

        timer += Time.deltaTime;

        image.fillAmount = isFill ? timer : 1f - timer;

    }



}
