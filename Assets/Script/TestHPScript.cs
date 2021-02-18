using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestHPScript : MonoBehaviour
{
    public float displayHp;
    public Text text;

    void Update()
    {
        text.text = displayHp.ToString();
    }
}
