using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem;

public class StringRulerEditor : EditorWindow
{
    private readonly string _options_Unit = "SR_UNIT";
    private readonly string _options_LabelSize = "SR_LABELSIZE";
    private readonly string _options_LineSize = "SR_LINESIZE";

    private readonly string[] _unitTitles = 
    {
        "Centimeter",
        "Meter",
        "Inch"
    };

    private readonly float _meterToInch = 39.3701f;

    private int _unit = 0;
    private float _lineSize = 0.1f;

    private Vector3 _startPosition;
    private Vector3 _endPosition;

    private bool _isPicking = false;
    private int _currentPoint = 0;
    private StringRulerPointList _pointAsset;
    private Handles.CapFunction _buttonHandle;
    private GUIStyle _labelStyle;


    public float GetTotalLen() {return _pointAsset.points.Count == 0 ? 0f : _pointAsset.points[_pointAsset.points.Count - 1].total;}
    public string GetUnitString() {return _unit == 0 ? " cm" : (_unit == 1 ? " m" : " inch");}
    public float GetLengthToUnit(float factor) {return _unit == 0 ? factor * 100f : (_unit == 1 ? factor : factor * _meterToInch);}


    [MenuItem("CustomWindow/String Ruler")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(StringRulerEditor), false, "String Ruler");
    }

    private void OnEnable()
    {
        _unit = EditorPrefs.GetInt(_options_Unit,0);
        _lineSize = EditorPrefs.GetFloat(_options_LineSize,0.01f);

        _pointAsset = (StringRulerPointList)AssetDatabase.LoadAssetAtPath("Assets/Settings/StringRulerPointList.asset",typeof(StringRulerPointList));

        _buttonHandle = (controlID, position, rotation, size, eventType)=>{
                Handles.CircleHandleCap(controlID, position, rotation, size, eventType);
            };
    }

    private void OnDisable()
    {
        EditorPrefs.SetInt(_options_Unit,_unit);
        EditorPrefs.SetFloat(_options_LineSize,_lineSize);
    }

    void OnFocus() 
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
        SceneView.duringSceneGui += this.OnSceneGUI;
    }
 
    void OnDestroy() 
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
    }

    private void OnGUI()
    {
        EditorGUIUtility.labelWidth = 60f;
        _unit = EditorGUILayout.Popup("Unit ",_unit, _unitTitles);
        _lineSize = EditorGUILayout.Slider("Line Size ",_lineSize,0f,1f);
        GUILayout.Label("Total Size : " + GetLengthToUnit(GetTotalLen()) + GetUnitString());

        if(GUILayout.Button("Clear"))
        {
            _pointAsset.Clear();
        }
    }

    void OnSceneGUI(SceneView sceneView) 
    {
        var currentEvent = Event.current;
        if(currentEvent.type == EventType.KeyDown && currentEvent.keyCode == KeyCode.LeftAlt && !_isPicking)
        {
            StartPick();
        }
        else if(currentEvent.type == EventType.KeyUp && currentEvent.keyCode == KeyCode.LeftAlt)
        {
            _isPicking = false;
        }

        if(_isPicking)
        {
            if(ScreenRaycast(out var hit,Event.current.mousePosition))
            {
                int next = _currentPoint + 1 >= _pointAsset.points.Count ? _pointAsset.points.Count - 1 : _currentPoint + 1;
                
                _pointAsset.points[_currentPoint].position = hit.point;
                _pointAsset.CalcDistanceAndAngle(_currentPoint);
                if(next != _currentPoint)
                    _pointAsset.CalcDistanceAndAngle(next);

                _pointAsset.CalcTotal(_currentPoint);

                Repaint();
            }
        }

        DrawWireCubes();
    }

    public void DrawWireCubes()
    {
        var camTransform = SceneView.lastActiveSceneView.camera.transform;
        var rot = camTransform.rotation;
        for(int i = 0; i < _pointAsset.points.Count; ++i)
        {
            Handles.matrix = Matrix4x4.identity;
            var dist = Vector3.Distance(camTransform.position,_pointAsset.points[i].position);
            dist = Mathf.Clamp(dist,0f,10f);
            if(Handles.Button(_pointAsset.points[i].position,rot,dist * 0.03f,dist * 0.03f,_buttonHandle))
            {
                if(_isPicking)
                {
                    _isPicking = false;
                }
                else
                {
                    _currentPoint = i;
                    _isPicking = true;
                }
            }

            if(i >= 1)
            {   
                Handles.matrix = Matrix4x4.TRS(_pointAsset.points[i].position, 
                    Quaternion.FromToRotation(Vector3.up, _pointAsset.points[i - 1].position - _pointAsset.points[i].position), 
                    new Vector3(_lineSize, Vector3.Distance(_pointAsset.points[i].position, _pointAsset.points[i - 1].position), _lineSize));
                Handles.DrawWireCube(Vector3.up * 0.5f, Vector3.one);

                Handles.matrix = Matrix4x4.identity;
                _labelStyle = new GUIStyle(GUI.skin.label);
                //_labelStyle.normal.textColor = Color.green;

                string label = "Dist : " + GetLengthToUnit(_pointAsset.points[i].distance) + GetUnitString() + "\n" + 
                                "Total : " + GetLengthToUnit(_pointAsset.points[i].total) + GetUnitString();
                Handles.Label(_pointAsset.points[i].position + camTransform.up * _lineSize + (camTransform.up * 0.4f),label,_labelStyle);
            }
            
        }
        
    }

    public void StartPick()
    {
        _isPicking = ScreenRaycast(out var hit,Event.current.mousePosition);
        if(!_isPicking)
        {
            Debug.LogError("Raycast failed");
            return;
        }

        if(_pointAsset.points.Count == 0)
            _pointAsset.Add(hit.point);

        _pointAsset.Add(hit.point);
        EditorUtility.SetDirty(_pointAsset);

        //Undo.RecordObject(_pointAsset,"String Ruler Picking");

        _currentPoint = _pointAsset.points.Count - 1;
    }

    public bool ScreenRaycast(out RaycastHit hit, Vector2 mousePosition)
    {
        Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(new Vector3(mousePosition.x, Screen.height - mousePosition.y - 36f, 0));
        return Physics.Raycast(ray, out hit, Mathf.Infinity);
    }
}
