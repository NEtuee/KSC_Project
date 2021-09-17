using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ClimbingLineEditor : EditorWindow
{
    [MenuItem("CustomWindow/ClimbingLineEditor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(ClimbingLineEditor));
    }

    private void Awake()
    {
        _modeItem = new string[]
        {
            "None","CreateLine","Add Point"
        };
    }

    private void OnGUI()
    {
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical("box", GUILayout.Width(150.0f));

        _currentMode = GUILayout.SelectionGrid(_currentMode, _modeItem, 1);

        GUILayout.Label("Current Line");
        _currentLine = (ClimbingLine)EditorGUILayout.ObjectField(_currentLine, typeof(ClimbingLine), true);

        GUILayout.Label("Line List");
        _lineListScroll = GUILayout.BeginScrollView(_lineListScroll);

        foreach(var line in _lineList)
        {
            try
            {
                if (GUILayout.Button(line.gameObject.name))
                {
                    _currentLine = line;
                    _currentMode = 2;
                }
            }
            catch(MissingReferenceException)
            {

            }
        }

        GUILayout.EndScrollView();

        GUILayout.EndVertical();

        switch(_currentMode)
        {
            case 2:
                {
                    GUILayout.BeginVertical("box",GUILayout.Width(300.0f));
                    GUILayout.Label("Point List");

                    if (_currentLine != null)
                    {
                        _pointListScroll = GUILayout.BeginScrollView(_pointListScroll);

                        foreach (var point in _currentLine.points)
                        {
                            GUILayout.BeginHorizontal("box");
                            if(GUILayout.Button(point.gameObject.name, GUILayout.Width(100.0f)))
                            {
                                Selection.activeObject = point.gameObject;
                                _currentMode = 0;
                            }

                            if(GUILayout.Button("Delete", GUILayout.Width(50.0f)))
                            {
                                _currentLine.points.Remove(point);
                                break;
                            }

                            GUILayout.EndHorizontal();
                        }

                        GUILayout.EndScrollView();
                    }
                    GUILayout.EndVertical();
                }
                break;
        }

        GUILayout.EndHorizontal();
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += this.OnSceneGUI;

        _climbingLineCount = 0;
        GameObject[] lines = GameObject.FindGameObjectsWithTag("ClimbingLine");
        foreach (var line in lines)
        {
            if (line.TryGetComponent<ClimbingLine>(out ClimbingLine climbingLine))
            {
                _lineList.Add(climbingLine);
                _climbingLineCount++;
            }
        }
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
        SceneVisibilityManager.instance.EnableAllPicking();
        _lineList.Clear();
    }

    private void OnSceneGUI(SceneView view)
    {
        if (_currentMode == 0)
        {
            SceneVisibilityManager.instance.EnableAllPicking();
            return;
        }

        Event currentEvent = Event.current;
        if (currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.N)
        {
            _currentMode = 0;
            return;
        }

        SceneVisibilityManager.instance.DisableAllPicking();
        AddPoint();
    }

    public void AddPoint()
    {        
        Event currentEvent = Event.current;

        if(currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
        {
            if(_currentMode == 1)
            {
                Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(new Vector3(currentEvent.mousePosition.x, Screen.height - currentEvent.mousePosition.y, 0));
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                {
                    GameObject newClimbingLineObject = new GameObject("ClimbingLine" + _climbingLineCount);
                    _currentLine = newClimbingLineObject.AddComponent<ClimbingLine>();
                    newClimbingLineObject.tag = "ClimbingLine";

                    newClimbingLineObject.transform.position = hit.point;

                    GameObject newPoint = new GameObject("point");
                    newPoint.transform.position = hit.point;
                    newPoint.transform.SetParent(_currentLine.transform);

                    _currentLine.points.Add(newPoint.transform);
                    _climbingLineCount++;
                    _currentMode = 2;

                    _lineList.Add(_currentLine);
                }
            }
            else if(_currentMode == 2)
            {
                if (_currentLine == null)
                    return;

                Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(new Vector3(currentEvent.mousePosition.x, Screen.height - currentEvent.mousePosition.y - 36, 0));
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                {
                    GameObject newPoint = new GameObject("point"+ _currentLine.points.Count);
                    newPoint.transform.position = hit.point;
                    newPoint.transform.SetParent(_currentLine.transform);
                    _currentLine.points.Add(newPoint.transform);
                }
            }
        }
    }

    private string[] _modeItem;
    private int _currentMode = 0;
    private ClimbingLine _currentLine;

    private List<ClimbingLine> _lineList = new List<ClimbingLine>();
    private int _climbingLineCount = 0;
    private Vector2 _lineListScroll = Vector2.zero;
    private Vector2 _pointListScroll = Vector2.zero;
}
