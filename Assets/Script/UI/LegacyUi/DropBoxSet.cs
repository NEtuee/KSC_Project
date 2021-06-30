using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DropBoxSet : MonoBehaviour
{
    public ImageBaseButton button;
    public TMP_Dropdown dropBox;

    public void Awake()
    {
        button.Init();
        button.Interactable = false;
    }
    public void Active(bool active)
    {
        dropBox.interactable = active;
    }
}
