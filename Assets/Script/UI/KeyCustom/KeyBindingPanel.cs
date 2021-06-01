using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBindingPanel : MonoBehaviour
{
    public List<ImageBaseButton> imageBaseButtons = new List<ImageBaseButton>();

    void Awake()
    {
        foreach(var button in imageBaseButtons)
        {
            button.Init();
            button.Interactable = false;
        }
    }

    void Update()
    {
        
    }
}
