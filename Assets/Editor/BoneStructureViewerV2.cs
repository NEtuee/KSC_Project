using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditorInternal;
using System.Collections.Generic;


public class BoneStructureViewerV2 : EditorWindow
{
    [System.Serializable]
    public class ChildObjectItem
    {
        public Transform transform;
        public int parent;
        public Vector2 viewPosition;
        public int deep;
    }

    [SerializeField]List<ChildObjectItem> _currentItemCache = new List<ChildObjectItem>();

    ChildObjectItem _root;

    private Matrix4x4 _modelMatrix;
    private Rect _viewRect;

    private Vector2 _centerPosition;
    private Vector2 _prevMousePosition;

    private Vector3 _modelPos;
    private Vector3 _modelRotation;
    private float _modelScale = 1f;

    private float _scaleFactor = 1f;
    private float _buttonScale = 10f;
    private float _sceneButtonScale = 0.01f;


    [MenuItem("CustomWindow/ChildStructureViewerV2")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(BoneStructureViewerV2),false,"ChildStructureViewerV2");
        
    }

    void OnGUI()
    {
        DropAreaGUI();

        if(_currentItemCache.Count > 0)
        {
            _root = _currentItemCache[0];
        }

        GUILayout.BeginHorizontal("box");

        GUILayout.BeginVertical();
        GUILayout.Label("ScalingFactor");
        _scaleFactor = EditorGUILayout.FloatField(_scaleFactor);
        GUILayout.EndVertical();
        
        GUILayout.BeginVertical();
        GUILayout.Label("SceneButtonSize");
        _sceneButtonScale = EditorGUILayout.FloatField(_sceneButtonScale);
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.Label("ButtonSize");
        _buttonScale = EditorGUILayout.FloatField(_buttonScale);
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        KeyInputCheck();
        DrawStructure("",new Vector2(5,130),new Vector2(-10f,-10f),new Vector3());
    }

    private void KeyInputCheck()
    {
        Event e = Event.current;

        //rotate
        if(e.type == EventType.MouseDown && e.button == 1)
        {
            _prevMousePosition = e.mousePosition;
        }
        else if(e.type == EventType.MouseDrag && e.button == 1)
        {
            Vector3 factor = e.mousePosition - _prevMousePosition;

            AddRotation(new Vector2(factor.y,factor.x));

            _prevMousePosition = e.mousePosition;
            e.Use();

            Repaint();
        }
        //translate
        else if(e.type == EventType.MouseDown && e.button == 2)
        {
            _prevMousePosition = e.mousePosition;
        }
        else if(e.type == EventType.MouseDrag && e.button == 2)
        {
            Vector3 factor = e.mousePosition - _prevMousePosition;

            AddPosition(factor);

            _prevMousePosition = e.mousePosition;
            e.Use();

            Repaint();
        }
        //scale
        else if(e.type == EventType.ScrollWheel)
        {
            AddScale(e.delta.y * _scaleFactor, true);
            e.Use();
            
            Repaint();
        }
    }

    private void AddScale(float factor, bool translate)
    {
        _modelScale += factor;
    }

    private void AddPosition(Vector3 factor)
    {
        _modelPos += factor;
    }

    private void AddRotation(Vector3 factor)
    {
        _modelRotation += factor;
        _modelRotation.x = Mathf.Abs(_modelRotation.x) >= 360f ? _modelRotation.x - 360f : _modelRotation.x;
        _modelRotation.y = Mathf.Abs(_modelRotation.y) >= 360f ? _modelRotation.y - 360f : _modelRotation.y;
    }

    private void DrawStructure(string targetName, Vector2 viewPos, Vector2 align,Vector3 euler)
    {
        _viewRect = new Rect(viewPos.x,viewPos.y,position.width + align.x, position.height - viewPos.y + align.y);
        _centerPosition = new Vector2(_viewRect.width * .5f, _viewRect.height * .5f);

        GUI.Box(_viewRect,targetName);
        GUI.BeginClip(_viewRect,Vector2.zero,Vector2.zero,false);

        if(_root != null)
        {
            _modelMatrix = Matrix4x4.Translate(_modelPos) * 
                        Matrix4x4.Rotate(Quaternion.Euler(_modelRotation)) *
                        Matrix4x4.Scale(new Vector3(_modelScale,_modelScale,_modelScale));

            var center = _root.transform.position;
            
            foreach(var item in _currentItemCache)
            {
                Vector4 pos = (item.transform.position - center);
                pos.x *= -1f;
                pos.w = 1f;
                pos = _modelMatrix * pos;

                item.viewPosition = new Vector2(pos.x,pos.y) + _centerPosition + viewPos;

                if(DrawButton(item.viewPosition,new Vector2(_buttonScale,_buttonScale),item.transform.name))
                {
                    PingTarget(item.transform.gameObject);
                }

                if(Selection.activeGameObject == item.transform.gameObject)
                {
                    DrawCrossLine(item.viewPosition,10f,Color.green);
                }

                if(item.parent != -1)
                {
                    Handles.color = Color.white;
                    Handles.DrawLine(item.viewPosition,GetItem(item.parent).viewPosition);
                }
            }
            
        }

        

        GUI.EndClip();
    }
    
    private void DrawCrossLine(Vector2 pos,float length, Color color)
    {
        var vS = new Vector2(pos.x - length,pos.y);
        var vE = new Vector2(pos.x + length,pos.y);
        var hS = new Vector2(pos.x,pos.y + length);
        var hE = new Vector2(pos.x,pos.y - length);

        Handles.color = color;
        Handles.DrawLine(vS,vE);
        Handles.DrawLine(hS,hE);
        
    }

    private bool DrawButton(Vector2 pos,Vector2 size, string tooltip)
    {
        pos -= size * .5f;

        return GUI.Button(new Rect(pos.x,pos.y,size.x,size.y),new GUIContent("",tooltip));
    }

    private void DrawListItem(string name, int deep)
    {
        GUILayout.BeginHorizontal();

        GUILayout.Space((float)(deep * 10f));
        Rect drop_area = GUILayoutUtility.GetRect(0, 20.0f,GUILayout.ExpandWidth (true));
        GUIStyle style = new GUIStyle(GUI.skin.box);
        style.alignment = TextAnchor.MiddleLeft;
        style.normal.textColor = Color.white;
        GUI.Box (drop_area, name, style);

        GUILayout.EndHorizontal();

        GUILayout.Space(5f);
    }

    private void PingTarget(GameObject obj)
    {
        EditorGUIUtility.PingObject(obj);
        Selection.activeGameObject = obj;
    }

    private ChildObjectItem SetItems(Transform root, int parentDeep,int parentPos)
    {
        ChildObjectItem item = GetChildObjectItemCache();
        item.transform = root;
        item.parent = parentPos;
        item.deep = parentDeep + 1;

        if(item.parent == -1)
        {
            _root = item;
        }

        _currentItemCache.Add(item);
        int back = _currentItemCache.Count - 1;

        for(int i = 0; i < root.childCount; ++i)
        {
            SetItems(root.GetChild(i),parentDeep,back);
        }

        return item;
    }

    private ChildObjectItem GetChildObjectItemCache()
    {
        //cached

        return new ChildObjectItem();
    }

    private void Dispose()
    {
        foreach(var item in _currentItemCache)
        {
            ClearItem(item);
        }
        _currentItemCache.Clear();
    }

    private ChildObjectItem GetItem(int pos) 
    {
        return _currentItemCache[pos];
    }

    private void ClearItem(ChildObjectItem item)
    {
        item.transform = null;
    }

    private void DropAreaGUI ()
    {
        Event evt = Event.current;
        Rect drop_area = GUILayoutUtility.GetRect (0.0f, 50.0f, GUILayout.ExpandWidth (true));
        GUI.Box (drop_area, "Object Drop");
     
        switch (evt.type) {
        case EventType.DragUpdated:
        case EventType.DragPerform:
            if (!drop_area.Contains (evt.mousePosition))
                return;
             
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
         
            if (evt.type == EventType.DragPerform) {
                DragAndDrop.AcceptDrag ();
             
                var obj = ((GameObject)DragAndDrop.objectReferences[0]);
                if(obj != null)
                {
                    var transform = obj.GetComponent<Transform>();
                    Dispose();
    
                    SetItems(transform, -1,-1);
                }

                EditorUtility.SetDirty(this);
                
            }
            break;
        }
    }

    void OnFocus() 
    {
        SceneView.duringSceneGui -= this.OnSceneGUI;
        SceneView.duringSceneGui += this.OnSceneGUI;
    }
 
    void OnDestroy() 
    {
        Dispose();
        SceneView.duringSceneGui -= this.OnSceneGUI;
    }
 
    void OnSceneGUI(SceneView sceneView) 
    {
        if(Event.current.type == EventType.Repaint)
        {
            Handles.color = Color.green;
        }

        foreach(var item in _currentItemCache)
        {
            if(item.parent != -1)
                Handles.DrawLine(item.transform.position,GetItem(item.parent).transform.position);
            
            Handles.CapFunction del = (controlID, position, rotation, size, eventType)=>{
                Handles.RectangleHandleCap(controlID, position, rotation, size, eventType);
                if(eventType == EventType.Repaint && HandleUtility.nearestControl == controlID)
                {
                    position.y += 0.05f;
                    Handles.Label(position, item.transform.name);
                }  
            };

            if(Handles.Button(item.transform.position,Camera.current.transform.rotation,_sceneButtonScale,_sceneButtonScale * 2f,del))
            {
                PingTarget(item.transform.gameObject);
                Repaint();
            }

        }
    

    }

}
