using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneStatusUI : MonoBehaviour
{
    private Canvas _canvas;

    [SerializeField]private GameObject[] hpIcons = new GameObject[10];

    private void Awake()
    {
        _canvas = GetComponent<Canvas>();
        //_canvas.enabled = false;
    }

    public void SetHpCount(int count)
    {
        count = Mathf.Clamp(count , 0, 10);
        for(int i = 0; i < 10; i++)
        {
            if(i > count - 1)
                hpIcons[i].SetActive(false);
            else
                hpIcons[i].SetActive(true);
        }
    }

    public void Enable(bool enable)
    {
        _canvas.enabled = enable;
    }
}
