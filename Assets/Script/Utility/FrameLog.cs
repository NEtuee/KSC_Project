using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrameLog : MonoBehaviour
{
    private float _fps;
    private float _prevMS;

    private float _showFps;
    private float _showMS;

    void Start()
    {
        StartCoroutine(UpdateFps());
    }

    void Update()
    {
        _prevMS = Time.deltaTime;
        _fps = 1.0f / Time.deltaTime;
    }

    IEnumerator UpdateFps()
    {
        while(true)
        {
            _showFps = _fps;
            yield return new WaitForSeconds(1f);
        }
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 25;
        style.normal.textColor = Color.white;

        GUI.Label(new Rect(1700f, 900f, 100, 20), "FPS : " + (int)_showFps, style);
        GUI.Label(new Rect(1700f, 930f, 100, 20), "MS : " + _prevMS*1000.0f, style);
    }
}
