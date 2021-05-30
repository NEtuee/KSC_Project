using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonGroup : MonoBehaviour
{
    private Canvas _canvas;

    public List<TextBaseButtonUi> buttonList = new List<TextBaseButtonUi>();

    void Awake()
    {
        _canvas = GetComponent<Canvas>();
    }

    public void Active(bool active)
    {
        _canvas.enabled = active;

        if (buttonList.Count == 0)
            return;

        if(active == true)
        {
            foreach (var button in buttonList)
            {
                button.Active(active);
            }

            buttonList[0].Select(active);
        }
        else
        {
            foreach (var button in buttonList)
            {
                button.Active(active);
                button.Select(active);
            }
        }
    }
}
