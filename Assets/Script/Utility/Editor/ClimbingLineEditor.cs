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
            "None","CreateLine","Add Point", "View PlaneInfo","Node Setting"
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
                    Selection.activeObject = line.gameObject;
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
                        if(GUILayout.Button("Direction Filp"))
                        {
                            if (_currentLine.directionType == DirectionType.LeftMin)
                            {
                                _currentLine.directionType = DirectionType.LeftMax;
                            }
                            else 
                            {
                                _currentLine.directionType = DirectionType.LeftMin;
                            }
                        }
                        if(_currentLine.directionType == DirectionType.LeftMin)
                        {
                            GUILayout.Label("Left : Min  Right : Max");
                        }
                        else
                        {
                            GUILayout.Label("Left : Max  Right : Min");
                        }

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
                                _currentLine.RemovePoint(point);
                                break;
                            }

                            GUILayout.EndHorizontal();
                        }

                        GUILayout.EndScrollView();
                    }
                    GUILayout.EndVertical();
                }
                break;
            case 3:
                {
                    GUILayout.BeginVertical("box", GUILayout.Width(300.0f));
                    GUILayout.Label("Plane Info List");

                    if (_currentLine != null)
                    {
                        _pointListScroll = GUILayout.BeginScrollView(_pointListScroll);

                        foreach (var planeInfo in _currentLine.planeInfo)
                        {
                            GUILayout.BeginHorizontal("box");
                            if (GUILayout.Button(planeInfo.gameObject.name, GUILayout.Width(100.0f)))
                            {
                                Selection.activeObject = planeInfo.gameObject;
                            }
                            GUILayout.EndHorizontal();
                        }

                        GUILayout.EndScrollView();
                    }
                    GUILayout.EndVertical();
                }
                break;
            case 4:
                {
                   

                    GUILayout.BeginVertical("box", GUILayout.Width(300.0f));
                    if (GUILayout.Button("Min", GUILayout.Width(100.0f)))
                    {
                        Selection.activeObject = _nodeMinObject;
                    }

                    if (GUILayout.Button("Max", GUILayout.Width(100.0f)))
                    {
                        Selection.activeObject = _nodeMaxObject;
                    }

                    _climbingLineManager.DrawNode = GUILayout.Toggle(_climbingLineManager.DrawNode, "Draw Node");

                    maxClimbingLineNum = EditorGUILayout.IntField(maxClimbingLineNum);

                    if(GUILayout.Button("Build"))
                    {
                        _climbingLineManager.BulidNode(maxClimbingLineNum);
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

        CheckClimbingManager();

        if(_climbingLineManager._rootNode == null)
        {
            GameObject rootNode = new GameObject("RootNode");
            rootNode.transform.SetParent(_climbingLineManager.transform);
            _climbingLineManager._rootNode = rootNode.AddComponent<CL_Node>();
        }

        if (_nodeMinObject == null)
        {
            GameObject minObj = new GameObject("MinObject");
            _nodeMinObject = minObj.transform;
            _nodeMinObject.position = _climbingLineManager._rootNode.min;
        }

        if (_nodeMaxObject == null)
        {
            GameObject maxObj = new GameObject("MaxObject");
            _nodeMaxObject = maxObj.transform;
            _nodeMaxObject.position = _climbingLineManager._rootNode.max;
        }
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
        SceneVisibilityManager.instance.EnableAllPicking();
        _lineList.Clear();

        _climbingLineManager = null;

        if (_nodeMinObject != null)
        {
            DestroyImmediate(_nodeMinObject.gameObject);
        }

        if(_nodeMaxObject != null)
        {
            DestroyImmediate(_nodeMaxObject.gameObject);
        }
    }

    private void Update()
    {
        //Debug.Log(_climbingLineManager._rootNode.min);

        if (_nodeMinObject != null && _nodeMaxObject != null)
        {
            Vector3 minPos = _nodeMinObject.position;
            Vector3 maxPos = _nodeMaxObject.position;

            Vector3 finalMinPos;
            finalMinPos.x = minPos.x > maxPos.x ? maxPos.x : minPos.x;
            finalMinPos.y = minPos.y > maxPos.y ? maxPos.y : minPos.y;
            finalMinPos.z = minPos.z > maxPos.z ? maxPos.z : minPos.z;

            Vector3 finalMaxPos;
            finalMaxPos.x = minPos.x > maxPos.x ? minPos.x : maxPos.x;
            finalMaxPos.y = minPos.y > maxPos.y ? minPos.y : maxPos.y;
            finalMaxPos.z = minPos.z > maxPos.z ? minPos.z : maxPos.z;

            _nodeMinObject.position = finalMinPos;
            _nodeMaxObject.position = finalMaxPos;

            _climbingLineManager._rootNode.min = _nodeMinObject.position;
            _climbingLineManager._rootNode.max = _nodeMaxObject.position;
        }


    }

    private void OnDrawGizmosSelected() 
    {
        if (_nodeMinObject != null)
        {
            Handles.Label(_nodeMinObject.position, "MIN");
        }

        if (_nodeMaxObject != null)
        {
            Handles.Label(_nodeMaxObject.position, "MAX");
        }
    }

    private void OnSceneGUI(SceneView view)
    {
        if (_nodeMinObject != null)
        {
            Handles.Label(_nodeMinObject.position, "MIN");
        }

        if (_nodeMaxObject != null)
        {
            Handles.Label(_nodeMaxObject.position, "MAX");
        }

        if (_currentMode == 0 || _currentMode == 3)
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
                Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(new Vector3(currentEvent.mousePosition.x, Screen.height - currentEvent.mousePosition.y -36, 0));
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                {
                    CheckClimbingManager();

                    GameObject newClimbingLineObject = new GameObject("ClimbingLine" + _climbingLineCount);
                    _currentLine = newClimbingLineObject.AddComponent<ClimbingLine>();
                    newClimbingLineObject.tag = "ClimbingLine";

                    newClimbingLineObject.transform.position = hit.point;
                    newClimbingLineObject.transform.SetParent(_climbingLineManager.transform);

                    _climbingLineManager.AddClimbingLines(_currentLine);

                    GameObject newPoint = new GameObject("point");
                    newPoint.transform.position = hit.point;
                    newPoint.transform.SetParent(_currentLine.transform);

                    _currentLine.AddPoint(newPoint.transform);
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
                    _currentLine.AddPoint(newPoint.transform);
                }
            }
        }
    }

    private void CheckClimbingManager()
    {
        if (_climbingLineManager == null)
        {
            if (GameObject.Find("ClimbingLineManager") == null)
            {
                GameObject climbingLineManger = new GameObject("ClimbingLineManager");
                _climbingLineManager = climbingLineManger.AddComponent<ClimbingLineManager>();
            }
            else
            {
                _climbingLineManager = GameObject.Find("ClimbingLineManager").GetComponent<ClimbingLineManager>();
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

    private ClimbingLineManager _climbingLineManager = null;

    private Transform _nodeMinObject = null;
    private Transform _nodeMaxObject = null;

    private int maxClimbingLineNum = 2;
}
