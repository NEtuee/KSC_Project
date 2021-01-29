using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MultiSelectPopupWindow : PopupWindowContent
{
    bool toggle1 = true;
    bool toggle2 = true;
    bool toggle3 = true;

    Vector2 scrollPosition;

    Dictionary<string, bool> items;
    List<string> itemKeys;

    public override Vector2 GetWindowSize()
    {
        return new Vector2(200, 150);
    }

    public void Initialize(Dictionary<string, bool> dic)
    {
        items = dic;
        itemKeys = items.Keys.ToList();
    }

    public override void OnGUI(Rect rect)
    {
        GUILayout.BeginVertical();
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        foreach(var key in itemKeys)
        {
            items[key] = EditorGUILayout.Toggle(key,items[key]);
        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    // public override void OnOpen()
    // {
    //     Debug.Log("Popup opened: " + this);
    // }

    // public override void OnClose()
    // {
    //     Debug.Log("Popup closed: " + this);
    // }
}