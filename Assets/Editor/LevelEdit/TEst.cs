using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using UnityEditor.SceneManagement;

public class TEst : EditorWindow
{
    [MenuItem("Example/SplitView")]
    public static void Init()
    {
        EditorWindow t = GetWindow<TEst>();
    }

    [SerializeField, SerializeReference]
    EditorGUISplitView splitView = new EditorGUISplitView(EditorGUISplitView.Direction.Horizontal);

    StageManagerEx _stageManager;

#region  ItemMenu
    string[] _itemMenu;
    Vector2 _itemMenuScroll = Vector2.zero;
    int _itemMenuSelect = 0;
#endregion


#region BooleanTriggerMenu
    string[] _booleanTriggerMenu;
    Vector2 _booleanTriggerMenuScroll = Vector2.zero;
    Vector2 _booleanTriggerScroll = Vector2.zero;
    Vector2 _booleanTriggerDescScroll = Vector2.zero;
    int _booleanTriggerMenuSelect = 0;
    int _booleanTriggerSelect = 0;

    string _booleanTriggerCreateName = "";
    string _booleanTriggerFindName = "";
    string _booleanTriggerDesc = "";

    BooleanTrigger[] _booleanTrigger = new BooleanTrigger[2];

#endregion


#region LevelObjectMenu

    string _LevelObjCreateName = "";
    Vector2 _levelObjScroll = new Vector2();
    Vector2 _levelObjReferenceScroll = new Vector2();

    int _levelObjSelect = -1;

#endregion


#region PathEditorMenu

    PathManagerEx _pathManager;

    Vector2 _pathMenuScroll = new Vector2();
    string _pathCreateName = "";
    float _heightDist = 0f;
    int _pointSelect = -1;

#endregion

    Dictionary<int,Action> _menuFunc = new Dictionary<int, Action>();
    List<GraphObjectBase> _levelObjects = new List<GraphObjectBase>();

    void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        _itemMenu = new string[]
        {
            "Boolean Triggers","Level Object Manage","Path Editor","","","","","","",""
        };
        _booleanTriggerMenu = new string[]{"GlobalTrigger","StageTrigger"};

        _stageManager = GameObject.FindObjectOfType<StageManagerEx>();
        _booleanTrigger[0] = (BooleanTrigger)AssetDatabase.LoadAssetAtPath("Assets/Settings/GlobalBooleanTrigger.asset",typeof(BooleanTrigger));
        _booleanTrigger[1] = _stageManager?.stageTriggerAsset;

        _pathManager = _stageManager?.GetPathManager();

        GetLevelObjects();

        _menuFunc.Add(0,BooleanTriggerMenu);
        _menuFunc.Add(1,LevelObjectMenu);
        _menuFunc.Add(2,PathEditorMenu);
    }

    void OnGUI()
    {
        if(_itemMenu == null || _menuFunc?.Count == 0)
            Initialize();
    

        GUILayout.BeginHorizontal();
        DrawMenu(150f);

        _menuFunc[_itemMenuSelect]();

        // splitView.BeginSplitView();

        // GUILayout.Label("Check");

        // splitView.Split();

        // GUILayout.Label("Check");
 
        // splitView.EndSplitView();

        // Repaint();
        //EditorUtility.FindAsset(,);

        GUILayout.EndHorizontal(); 
    }

    void BooleanTriggerMenu()
    {
        GUILayout.BeginVertical("box",GUILayout.Width(150f));
        //_booleanTriggerMenuScroll = GUILayout.BeginScrollView(_booleanTriggerMenuScroll);
        _booleanTriggerMenuSelect = GUILayout.SelectionGrid(_booleanTriggerMenuSelect,_booleanTriggerMenu,1);
        
        var currentTrigger = _booleanTrigger[_booleanTriggerMenuSelect];
        var isTriggerAssetExist = currentTrigger != null;


        GUILayout.Space(10f);


        GUI.enabled = isTriggerAssetExist;

        GUI.SetNextControlName("tf_btcn");
        _booleanTriggerCreateName = GUILayout.TextField(_booleanTriggerCreateName);

        if(GUILayout.Button("CreateTrigger") || 
            (Event.current.type == EventType.KeyUp && 
            Event.current.keyCode == KeyCode.Return && 
            GUI.GetNameOfFocusedControl() == "tf_btcn"))
        {
            if(_booleanTriggerCreateName == "")
            {
                Debug.Log("name is empty");
            }
            else
            {
                currentTrigger.AddTrigger(_booleanTriggerCreateName);

                EditorUtility.SetDirty(currentTrigger);

                _booleanTriggerCreateName = "";
                GUI.FocusControl("");

                Repaint();
            }
        }

        GUILayout.BeginHorizontal();

        if(GUILayout.Button("▲"))
        {
            if(_booleanTriggerSelect > 0)
            {
                var save = currentTrigger.booleans[_booleanTriggerSelect-1];
                currentTrigger.booleans[_booleanTriggerSelect-1] = currentTrigger.booleans[_booleanTriggerSelect];
                currentTrigger.booleans[_booleanTriggerSelect] = save;

                _booleanTriggerSelect -= 1;
                Repaint();
            }
        }

        if(GUILayout.Button("▼"))
        {
            if(_booleanTriggerSelect < currentTrigger.booleans.Count - 1)
            {
                var save = currentTrigger.booleans[_booleanTriggerSelect+1];
                currentTrigger.booleans[_booleanTriggerSelect+1] = currentTrigger.booleans[_booleanTriggerSelect];
                currentTrigger.booleans[_booleanTriggerSelect] = save;

                _booleanTriggerSelect += 1;
                Repaint();
            }
        }

        GUILayout.EndHorizontal();

        GUILayout.Space(10f);

        _booleanTriggerDescScroll = GUILayout.BeginScrollView(_booleanTriggerDescScroll);

        var existsCheck = currentTrigger != null && _booleanTriggerSelect >= 0 && _booleanTriggerSelect < currentTrigger.booleans.Count;

        GUI.enabled = existsCheck;

        if(!existsCheck)
            _booleanTriggerDesc = "";

        _booleanTriggerDesc = EditorGUILayout.TextArea(_booleanTriggerDesc,GUILayout.ExpandHeight(true));

        GUILayout.EndScrollView();
        
        if(GUILayout.Button("Change Desc"))
        {
            currentTrigger.booleans[_booleanTriggerSelect].description = _booleanTriggerDesc;
            GUI.FocusControl("");
            EditorUtility.SetDirty(currentTrigger);
        }

        GUI.enabled = true;


        GUILayout.EndVertical();

        if(currentTrigger == null)
        {
            GUILayout.Label("Current Boolean Trigger Is Not Exists");

            if(_booleanTriggerMenuSelect == 1)
            {
                if(GUILayout.Button("Create Boolean Trigger Set") && _stageManager != null)
                {
                    _stageManager.stageTriggerAsset = ScriptableObject.CreateInstance<BooleanTrigger>();
                    _booleanTrigger[1] = _stageManager.stageTriggerAsset;
                }
            }
            return;
        }

        GUILayout.BeginHorizontal("box",GUILayout.ExpandHeight(true));

        int fitCount = (int)(position.height / 25f);

        for(int i = 0; i <= currentTrigger.booleans.Count / fitCount; ++i)
        {
            var drawedCount = (i * fitCount);
            var limit = currentTrigger.booleans.Count - drawedCount;
            limit = limit > fitCount ? fitCount : limit;

            GUILayout.BeginVertical();

            for(int j = 0; j < limit; ++j)
            {
                var tuple = currentTrigger.booleans[j + drawedCount];
                var triggered = tuple.trigger;


                GUILayout.BeginHorizontal();

                //GUI.enabled = !(_booleanTriggerSelect == (j + drawedCount));
                var currentIs = _booleanTriggerSelect == (j + drawedCount);
                var color = GUI.backgroundColor;
                if(currentIs)
                {
                    GUI.backgroundColor = Color.gray;
                }

                if(GUILayout.Button(new GUIContent("···",tuple.description),GUILayout.Width(30f)))
                {
                    _booleanTriggerSelect = !currentIs ? (j + drawedCount) : -1;
                    if(_booleanTriggerSelect != -1)
                        _booleanTriggerDesc = currentTrigger.booleans[_booleanTriggerSelect].description;
                }

                //GUI.enabled = true;
                GUI.backgroundColor = color;

                tuple.trigger = GUILayout.Toggle(tuple.trigger, " " + (j + drawedCount) + ". " + tuple.name);
                GUILayout.Space(5f);

                if(tuple.trigger != triggered)
                {
                    EditorUtility.SetDirty(currentTrigger);
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            //for(int j = i * 20; j < i * 20)
        }

        GUILayout.EndHorizontal();
    }

    void LevelObjectMenu()
    {
        GUILayout.BeginVertical("box",GUILayout.Width(150f),GUILayout.ExpandHeight(true));
        
        GUILayout.Label("Options");

        if(GUILayout.Button("Create Level Object"))
        {
            var obj = CreateLevelObject();
            _levelObjects.Add(obj);
            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);
        }
        
        if(GUILayout.Button("Create PathFollow Object"))
        {
            var obj = CreatePathFollowLevelObject();
            _levelObjects.Add(obj);
            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);
        }

        if(GUILayout.Button("Create Trigger"))
        {
            var obj = CreateTrigger();
            _levelObjects.Add(obj);
            Selection.activeObject = obj;
            EditorGUIUtility.PingObject(obj);
        }

        GUILayout.EndVertical();


        GUILayout.BeginVertical("box",GUILayout.Width(150f),GUILayout.ExpandHeight(true));
        
        GUILayout.Label("Object List");

        if(GUILayout.Button("Refresh"))
        {
            GetLevelObjects();
            Repaint();
        }

        GUILayout.Space(10f);

        _levelObjScroll = GUILayout.BeginScrollView(_levelObjScroll);

        for(int i = 0; i < _levelObjects.Count;)
        {
            if(_levelObjects[i] == null)
            {
                _levelObjects.RemoveAt(i);
                continue;
            }

            GUILayout.BeginHorizontal();

            if(GUILayout.Button("<",GUILayout.Width(20f)))
            {
                Selection.activeObject = _levelObjects[i];
                EditorGUIUtility.PingObject(_levelObjects[i]);
            }

            var currentIs = _levelObjSelect == i;
            GUI.enabled = !currentIs;
            var color = GUI.backgroundColor;
            if(currentIs)
            {
                GUI.backgroundColor = Color.gray;
            }


            if(GUILayout.Button(_levelObjects[i].name,GUILayout.ExpandWidth(true)))
            {
                _levelObjSelect = i;
            }

            GUI.enabled = true;
            GUI.backgroundColor = color;

            GUILayout.EndHorizontal();

            ++i;
        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();



        GUILayout.BeginVertical("box");

        GUILayout.Label("Object Reference");

        _levelObjReferenceScroll = GUILayout.BeginScrollView(_levelObjReferenceScroll);

        GraphObjectBase current = _levelObjSelect < 0 ? null : (_levelObjects.Count <= _levelObjSelect ? null : _levelObjects[_levelObjSelect]);

        if(current?.graphOrigin != null)
        {
            foreach(var item in current.graphOrigin.exposedParameters)
            {
                if(!item.canDelete)
                    continue;

                GUILayout.BeginHorizontal();

                GUILayout.Label(item.name,GUILayout.Width(150f));
                if(item.value == null)
                    GUILayout.Label("Value is Null",GUILayout.ExpandWidth(true));
                else if(item.GetValueType() == typeof(UnityEngine.Events.UnityEvent))
                {
                    var value = (UnityEngine.Events.UnityEvent)item.value;
                    var count = value.GetPersistentEventCount();

                    GUILayout.BeginVertical("box");
                    GUILayout.Label("Unity Event List");

                    for(int i = 0; i < count; ++i)
                    {
                        var eventItem = value.GetPersistentTarget(i);

                        GUI.enabled = false;
                        var obj = EditorGUILayout.ObjectField(eventItem,eventItem.GetType(),true,GUILayout.ExpandWidth(true));
                        GUI.enabled = true;

                        GUILayout.Label(" -" + value.GetPersistentMethodName(i),GUILayout.ExpandWidth(true));

                    }

                    GUILayout.EndVertical();
                }
                else if(item.GetValueType().IsSubclassOf(typeof(UnityEngine.Object)))
                {
                    var obj = EditorGUILayout.ObjectField((UnityEngine.Object)item.value,item.GetValueType(),true,GUILayout.ExpandWidth(true));
                    if(item.value != (object)obj)
                    {
                        item.value = obj;
                        EditorUtility.SetDirty(current.graphOrigin);
                    }
                    
                    //item.value = EditorGUILayout.PropertyField(item.);
                }

                GUILayout.EndHorizontal();
            }
            
        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    void PathEditorMenu()
    {
        GUILayout.BeginVertical("box",GUILayout.Width(150f));
        if(_pathManager == null)
        {
            _pathManager = _stageManager.GetPathManager();
            return;
        }

        var path = _pathManager.movePaths;

        _pathCreateName = GUILayout.TextField(_pathCreateName);
        if(GUILayout.Button("Create Path") && _pathCreateName != "")
        {
            if(path.Find((x)=>{return x.name == _pathCreateName;}) != null)
            {
                Debug.Log("a path with the same name already exists");
                return;
            }

            _pathManager.CreatePath(_pathCreateName);
            _pathManager.currentPath = _pathCreateName;

            _pathCreateName = "";

            GUI.FocusControl("");
        }

        GUILayout.Space(10f);

        _pathMenuScroll = GUILayout.BeginScrollView(_pathMenuScroll);

        for(int i = 0; i < path.Count; ++i)
        {
            GUI.enabled = _pathManager.currentPath != path[i].name;
            if(GUILayout.Button(path[i].name))
            {
                _pathManager.currentPath = path[i].name;
            }
            GUI.enabled = false;
        }

        GUILayout.EndScrollView();
        GUILayout.EndVertical();

        GUILayout.BeginVertical("box",GUILayout.Width(150f),GUILayout.ExpandHeight(true));
        GUI.enabled = _pathManager.currentPath != "";

        GUILayout.Label(_pathManager.currentPath);

        EditorGUIUtility.labelWidth = 60f;
        _heightDist = EditorGUILayout.FloatField("Height",_heightDist);

        if(GUILayout.Button("Delete Path"))
        {
            _pathManager.DisposePath(_pathManager.currentPath);
            _pathManager.currentPath = "";
        }

        GUILayout.Space(10f);

        if(GUILayout.Button("AddPoint"))
		{
			_stageManager.AddPoint(_pathManager.currentPath,CreatePoint());
		}

        GUI.enabled = _pointSelect != -1;

        if(GUILayout.Button("Delete Point"))
        {
            _stageManager.DeletePoint(_pathManager.currentPath,_pointSelect);
            _pointSelect = -1;
        }

        GUI.enabled = true;

        if(GUILayout.Button("Save"))
        {
            EditorUtility.SetDirty(_pathManager);
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        GUI.enabled = false;
        GUILayout.EndVertical();

        var list = _stageManager.GetPointList(_pathManager.currentPath);
        if(list == null)
            return;


        GUILayout.BeginHorizontal("box",GUILayout.ExpandHeight(true));

        int fitCount = (int)(position.height / 25f);

        for(int i = 0; i <= list.Count / fitCount; ++i)
        {
            var drawedCount = (i * fitCount);
            var limit = list.Count - drawedCount;
            limit = limit > fitCount ? fitCount : limit;

            GUILayout.BeginVertical();

            for(int j = 0; j < limit; ++j)
            {
                var item = list[j + drawedCount];

                GUILayout.BeginHorizontal();

                var currentIs = Selection.activeGameObject == item.gameObject;
                var color = GUI.backgroundColor;
                GUI.enabled = !currentIs;

                if(currentIs)
                {
                    GUI.backgroundColor = Color.gray;
                }

                if(GUILayout.Button(new GUIContent("···"),GUILayout.Width(30f)))
                {
                    _pointSelect = j + drawedCount;
                    Selection.activeObject = item.gameObject;
                }

                GUI.enabled = true;
                GUI.backgroundColor = color;

                GUILayout.Label(" " + (j + drawedCount));
                GUILayout.Space(5f);

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
            //for(int j = i * 20; j < i * 20)
        }

        GUILayout.EndHorizontal();

    }

    void OnEnable()
    {
        SceneView.duringSceneGui += this.OnSceneGUI;
    }

    void OnDisable()
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
    }

    void OnSceneGUI(SceneView view)
	{
        if(_itemMenuSelect == 2)
        {
            AddPointToCursorPoint();
            DrawPointButtons();
        }
    }

    public void DrawPointButtons()
    {
        if(_stageManager == null || _stageManager.pathManager == null)
            return;

        var list = _stageManager.GetPointList(_pathManager.currentPath);

        if(list == null)
            return;
        
        foreach(var item in list)
        {
            Handles.CapFunction del = (controlID, position, rotation, size, eventType)=>{
                Handles.RectangleHandleCap(controlID, position, rotation, size, eventType);
            };

            if(Handles.Button(item.GetPoint(),Camera.current.transform.rotation,.1f,.2f,del))
            {
                EditorGUIUtility.PingObject(item);
                Selection.activeObject = item;

                Repaint();
            }
        }

        
    }
	public void AddPointToCursorPoint()
	{
        if(_stageManager == null || _stageManager.pathManager == null)
            return;

		Event e = Event.current;

		if(e.type == EventType.KeyDown && 
			e.keyCode == KeyCode.P &&
			_stageManager.GetPathManager().currentPath != "")
		{
			//Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay(e.mousePosition);
			Ray ray = SceneView.lastActiveSceneView.camera.ScreenPointToRay( new Vector3( e.mousePosition.x, Screen.height - e.mousePosition.y - 36, 0 ) );
			if(Physics.Raycast(ray,out RaycastHit hit,Mathf.Infinity))
			{
				var point = CreatePoint();
				point.transform.position = hit.point + hit.normal * _heightDist;
				point.transform.rotation = Quaternion.FromToRotation(point.transform.up,hit.normal);

				_stageManager.AddPoint(_stageManager.GetPathManager().currentPath,point);
                Repaint();
			}
			else
			{
				Debug.Log("raycast failed");
			}
		}
        else if(e.type == EventType.KeyDown && 
			e.keyCode == KeyCode.O &&
			_stageManager.GetPathManager().currentPath != "")
        {
            var point = CreatePoint();

			_stageManager.AddPoint(_stageManager.GetPathManager().currentPath,point);
            Repaint();
        } 
	}

    private MovePointEx CreatePoint()
	{
		GameObject obj = new GameObject(_stageManager.GetPathManager().currentPath + " : Point");
		var point = obj.AddComponent<MovePointEx>();

        obj.transform.position = SceneView.lastActiveSceneView.pivot;
		obj.transform.SetParent(_stageManager.transform);

		return point;
	}

    GraphObjectBase CreatePathFollowLevelObject()
    {
        GameObject obj = new GameObject("New PathFollow Object");
        obj.transform.position = SceneView.lastActiveSceneView.pivot;

        var graph = obj.AddComponent<PathFollowGraphObjectBase>();
        graph.graphOrigin = ScriptableObject.CreateInstance<LevelObjectGraph>();
        graph.graphOrigin.AddExposedParameter("PathFollow Object",typeof(GraphProcessor.PathFollowGraphObjectParamter),graph,false,true);

        return graph;
    }

    GraphObjectBase CreateLevelObject()
    {
        GameObject obj = new GameObject("New Level Object");
        obj.transform.position = SceneView.lastActiveSceneView.pivot;

        var graph = obj.AddComponent<GraphObjectBase>();
        graph.graphOrigin = ScriptableObject.CreateInstance<LevelObjectGraph>();

        return graph;
    }

    GraphObjectBase CreateTrigger()
    {
        GameObject obj = new GameObject("New Trigger Object");
        obj.transform.position = SceneView.lastActiveSceneView.pivot;

        var graph = obj.AddComponent<GraphObjectBase>();
        graph.graphOrigin = ScriptableObject.CreateInstance<LevelObjectGraph>();

        var coll = graph.gameObject.AddComponent<BoxCollider>();
        coll.isTrigger = true;

        return graph;
    }

    void GetLevelObjects()
    {
        _levelObjects?.Clear();
        _levelObjects = new List<GraphObjectBase>(FindObjectsOfType<GraphObjectBase>());
    }

    void DrawMenu(float width)
    {
        GUILayout.BeginVertical("box",GUILayout.Width(width));
        _itemMenuScroll = GUILayout.BeginScrollView(_itemMenuScroll);
        _itemMenuSelect = GUILayout.SelectionGrid(_itemMenuSelect,_itemMenu,1);

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

}