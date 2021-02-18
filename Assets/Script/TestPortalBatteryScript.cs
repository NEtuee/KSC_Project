using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TestPortalBatteryScript : MonoBehaviour
{
    public int battery = 0;
    public Text text;


    public void Update()
    {
        text.text = battery.ToString();
    }
}
