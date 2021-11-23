using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEdit_ComponentActivator : MonoBehaviour
{
    public MonoBehaviour[] targetComponents;

    public void SetActive(bool value)
    {
        foreach(var item in targetComponents)
        {
            item.enabled = value;
        }
    }
}
