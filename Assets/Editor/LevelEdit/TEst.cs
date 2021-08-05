using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

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

    Dictionary<int,Action> _menuFunc = new Dictionary<int, Action>();

    void Awake()
    {
        Initialize();
    }

    void Initialize()
    {
        _itemMenu = new string[]
        {
            "BooleanTriggers","","","","","","","","",""
        };
        _booleanTriggerMenu = new string[]{"GlobalTrigger","StageTrigger"};

        _stageManager = GameObject.FindObjectOfType<StageManagerEx>();
        _booleanTrigger[0] = (BooleanTrigger)AssetDatabase.LoadAssetAtPath("Assets/Settings/GlobalBooleanTrigger.asset",typeof(BooleanTrigger));
        _booleanTrigger[1] = _stageManager?.stageTriggerAsset;

        _menuFunc.Add(0,BooleanTriggerMenu);
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

    void DrawMenu(float width)
    {
        GUILayout.BeginVertical("box",GUILayout.Width(width));
        _itemMenuScroll = GUILayout.BeginScrollView(_itemMenuScroll);
        _itemMenuSelect = GUILayout.SelectionGrid(_itemMenuSelect,_itemMenu,1);

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

}