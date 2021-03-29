using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditorInternal;
using System.Collections.Generic;

public class BoneStructViewer : EditorWindow
{
    private class ChildObjectItem
    {
        public Transform transform;
        public ChildObjectItem parent;
        public List<ChildObjectItem> childs = new List<ChildObjectItem>();
        public Rect currentRect;

        public bool ContainsChild(ChildObjectItem item)
        {
            return childs.Find((value)=> value == item) != null;
        }
    }

    private enum ViewType
    {
        XAxis,
        YAxis,
        ZAxis
    };

    private enum Alignment
    {
        LeftTop = 0,
        LeftMiddle,
        LeftBottom,
        MiddleTop,
        MiddleCenter,
        MiddleBottom,
        RightTop,
        RightCenter,
        RightBottom
    };

    private Queue<ChildObjectItem> objCache = new Queue<ChildObjectItem>();
    private ChildObjectItem rootObject;

    private MultiSelectPopupWindow tagSelectWindow;

    private Vector2 childScrollviewPos;
    private Vector2 structureScrollviewPos;

    private static Dictionary<ViewType,Vector2> scrollPositionDic = new Dictionary<ViewType, Vector2>();
    //private static Dictionary<string, bool> tagSelect = new Dictionary<string, bool>();

    private static string[] tagArray;
    private static string[] layerArray;

    private int selectedTag = ~0;
    private int selectedLayer = ~0;

    private float viewScale = 100f;

    private Vector2 objectSize = new Vector2(10f,10f);
    private Vector3 objectRotation = new Vector3();

    private Vector2 prevMousePos = new Vector2();


    [MenuItem("CustomWindow/ChildStructureViewer")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(BoneStructViewer),false,"ChildStructureViewer");
        scrollPositionDic.Add(ViewType.XAxis,new Vector2());
        scrollPositionDic.Add(ViewType.YAxis,new Vector2());
        scrollPositionDic.Add(ViewType.ZAxis,new Vector2());

        tagArray = InternalEditorUtility.tags;
        layerArray = InternalEditorUtility.layers;

        // if(tagSelect.Count == 0)
        // {
        //     var tags = UnityEditorInternal.InternalEditorUtility.tags;

        //     foreach(var tag in tags)
        //     {
        //         tagSelect.Add(tag,true);
        //     }
        // }
        
    }

    Rect buttonRect;
    void OnGUI()
    {
        EditorGUILayout.Space ();
        DropAreaGUI ();

        Event e = Event.current;
 
        if(e.type == EventType.MouseDown && e.button == 1)
        {
            prevMousePos = e.mousePosition;
        }
        if(e.type == EventType.MouseDrag && e.button == 1)
        {
            var factor = e.mousePosition - prevMousePos;

            AddRotation(new Vector3(0f,factor.x * 0.6f,0f));

            prevMousePos = e.mousePosition;
            e.Use();
        }

        EditorGUILayout.BeginVertical();
        HorizontalLine(new Vector2(20f, 20f));
        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginHorizontal("box");

        if(GUILayout.Button("Reload"))
        {
            Reload();
        }
        selectedTag = EditorGUILayout.MaskField(selectedTag, tagArray);
        selectedLayer = EditorGUILayout.MaskField(selectedLayer, layerArray);

        viewScale = EditorGUILayout.Slider(viewScale,1f,500f);
        objectRotation.y = EditorGUILayout.Slider(objectRotation.y,0f,360f);

        EditorGUILayout.EndHorizontal();

        

        DrawStructure("X Foward",new Vector2(1f,0.5f), new Vector2(5,130),objectRotation,ViewType.XAxis);
        // DrawStructure("Y Foward", new Vector2(5 + position.width * .33f,130),ViewType.YAxis);
        // DrawStructure("Z Foward", new Vector2(5 + position.width * .66f,130),ViewType.ZAxis);

        GUILayout.Space(position.height * .5f + 15f);
        DrawList();
    }

    void OnDestroy()
    {
        Dispose();
        scrollPositionDic.Clear();
        //tagSelect.Clear();
    }

    private void AddRotation(Vector3 euler)
    {
        objectRotation += euler;
        objectRotation.x = MathEx.clamp360Degree(objectRotation.x);
        objectRotation.y = MathEx.clamp360Degree(objectRotation.y);
        objectRotation.z = MathEx.clamp360Degree(objectRotation.z);
    }

    private void DrawStructure(string targetName, Vector2 ratio, Vector2 pos,Vector3 euler, ViewType view)
    {
        Rect rect = new Rect(pos.x,pos.y,position.width * ratio.x - 5f, position.height * ratio.y);
        Rect viewRect = Rect.zero;
        if(rootObject != null)
        {
            viewRect = CalcBound(view,30f,rect);
        }
        GUI.Box(rect,targetName);
        scrollPositionDic[view] = GUI.BeginScrollView(new Rect(pos.x, pos.y, position.width * ratio.x - 5f, position.height * ratio.y), 
                                                scrollPositionDic[view], viewRect);

        if(rootObject != null)
            DrawStructureItem(rootObject,rect,ConvertToCenterCoordinate(rect,Vector3.zero),euler,view,false);

        GUI.EndScrollView();
    }

    private void DrawStructure(string targetName, Vector2 pos, ViewType view)
    {
        Rect rect = new Rect(pos.x,pos.y,position.width * .33f - 5, position.height * .5f);
        Rect viewRect = Rect.zero;
        if(rootObject != null)
        {
            viewRect = CalcBound(view,30f,rect);
        }
        GUI.Box(rect,targetName);
        scrollPositionDic[view] = GUI.BeginScrollView(new Rect(pos.x, pos.y, position.width * .33f - 5, position.height * .5f), 
                                                scrollPositionDic[view], viewRect);

        if(rootObject != null)
            DrawStructureItem(rootObject,rect,ConvertToCenterCoordinate(rect,Vector3.zero),view,false);

        GUI.EndScrollView();
    }

    private Rect CalcBound(ViewType view, Vector3 euler,float margin, Rect rect)
    {
        Vector2 leftTop = Vector3.zero;
        Vector2 rightBottom = Vector3.zero;

        FindEdge(ref leftTop,ref rightBottom,rootObject,euler,view);
        leftTop *= viewScale;
        rightBottom *= viewScale;

        leftTop = ConvertToCenterCoordinate(rect,leftTop);
        rightBottom = ConvertToCenterCoordinate(rect,rightBottom);

        return new Rect(leftTop.x - margin,leftTop.y - margin,rightBottom.x - leftTop.x + margin * 2f, rightBottom.y - leftTop.y + margin * 2f);
    }

    private Rect CalcBound(ViewType view,float margin, Rect rect)
    {
        Vector2 leftTop = Vector3.zero;
        Vector2 rightBottom = Vector3.zero;

        FindEdge(ref leftTop,ref rightBottom,rootObject,view);
        leftTop *= viewScale;
        rightBottom *= viewScale;

        leftTop = ConvertToCenterCoordinate(rect,leftTop);
        rightBottom = ConvertToCenterCoordinate(rect,rightBottom);

        return new Rect(leftTop.x - margin,leftTop.y - margin,rightBottom.x - leftTop.x + margin * 2f, rightBottom.y - leftTop.y + margin * 2f);
    }

    private void FindEdge(ref Vector2 leftTop, ref Vector2 rightBottom, ChildObjectItem item, Vector3 euler, ViewType view)
    {
        Vector3 pos = rootObject.transform.position - item.transform.position;
        pos = RotateVector(pos,Quaternion.Euler(euler));
        pos = ConvertToViewPlane(view,pos);

        if(pos.x < leftTop.x)
            leftTop.x = pos.x;
        else if(pos.x > rightBottom.x)
            rightBottom.x = pos.x;

        if(pos.y < leftTop.y)
            leftTop.y = pos.y;
        else if(pos.y > rightBottom.y)
            rightBottom.y = pos.y;

        foreach(var child in item.childs)
        {
            FindEdge(ref leftTop, ref rightBottom, child,view);
        }
    }

    private void FindEdge(ref Vector2 leftTop, ref Vector2 rightBottom, ChildObjectItem item, ViewType view)
    {
        Vector3 pos = rootObject.transform.position - item.transform.position;
        pos = ConvertToViewPlane(view,pos);

        if(pos.x < leftTop.x)
            leftTop.x = pos.x;
        else if(pos.x > rightBottom.x)
            rightBottom.x = pos.x;

        if(pos.y < leftTop.y)
            leftTop.y = pos.y;
        else if(pos.y > rightBottom.y)
            rightBottom.y = pos.y;

        foreach(var child in item.childs)
        {
            FindEdge(ref leftTop, ref rightBottom, child,view);
        }
    }

    private void DrawStructureItem(ChildObjectItem item, Rect viewArea, Vector2 parent, ViewType view, bool unMask)
    {
        Vector3 rootPosition = rootObject.transform.position - item.transform.position;
        Vector2 pos = Vector2.zero;

        pos = ConvertToViewPlane(view,rootPosition);
        pos *= viewScale;
        pos = ConvertToCenterCoordinate(viewArea,pos);

        Rect rect = new Rect(pos.x,pos.y,10f,10f);

        bool tagMask = ((1 << FindTag(item.transform.tag)) & selectedTag) != 0;
        bool layerMask = ((1 << item.transform.gameObject.layer) & (selectedLayer)) != 0;

        if(tagMask && layerMask)
        {
            if(GUI.Button(GetAlignmentRect(rect,4),new GUIContent("",item.transform.name)))
            {
                EditorGUIUtility.PingObject(item.transform.gameObject);
                Selection.activeGameObject = item.transform.gameObject;
            }
            
            if(Event.current.type == EventType.Repaint)
            {
                item.currentRect = GUILayoutUtility.GetLastRect();
            }

            if(unMask)
                UnityEditor.Handles.DrawLine(parent,pos);
        }

        foreach(var child in item.childs)
        {
            DrawStructureItem(child,viewArea,pos,view, tagMask && layerMask);
        }

        GUIStyle style = new GUIStyle();

        Event evt = Event.current;

        if(evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)
        {
            if (!GetAlignmentRect(rect,4).Contains (evt.mousePosition))
                return;

            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (evt.type == EventType.DragPerform) 
            {
                DragAndDrop.AcceptDrag ();

                foreach(var obj in DragAndDrop.objectReferences)
                {
                    var transform = ((GameObject)obj).GetComponent<Transform>();
                    item.childs.Add(SetItems(transform,item));
                    transform.parent = item.transform;
                }
                
            }
        }
    }

    private void DrawStructureItem(ChildObjectItem item, Rect viewArea, Vector2 parent,Vector3 euler, ViewType view, bool unMask)
    {
        Vector3 rootPosition = rootObject.transform.position - item.transform.position;
        Vector2 pos = Vector2.zero;

        pos = ConvertToViewPlane(view,RotateVector(rootPosition,Quaternion.Euler(euler)));
        pos *= viewScale;
        pos = ConvertToCenterCoordinate(viewArea,pos);

        var scale = Mathf.Clamp((viewScale * 0.01f) * 0.5f, 1f, 5f) * 10f;

        Rect rect = new Rect(pos.x,pos.y,scale,scale);

        bool tagMask = ((1 << FindTag(item.transform.tag)) & selectedTag) != 0;
        bool layerMask = ((1 << item.transform.gameObject.layer) & (selectedLayer)) != 0;

        if(tagMask && layerMask)
        {
            if(GUI.Button(GetAlignmentRect(rect,4),new GUIContent("",item.transform.name)))
            {
                EditorGUIUtility.PingObject(item.transform.gameObject);
                Selection.activeGameObject = item.transform.gameObject;
            }
            
            if(Event.current.type == EventType.Repaint)
            {
                item.currentRect = GUILayoutUtility.GetLastRect();
            }

            if(unMask)
                UnityEditor.Handles.DrawLine(parent,pos);
        }

        foreach(var child in item.childs)
        {
            DrawStructureItem(child,viewArea,pos,euler,view, tagMask && layerMask);
        }

        GUIStyle style = new GUIStyle();

        Event evt = Event.current;

        if(evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform)
        {
            if (!GetAlignmentRect(rect,4).Contains (evt.mousePosition))
                return;

            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

            if (evt.type == EventType.DragPerform) 
            {
                DragAndDrop.AcceptDrag ();

                foreach(var obj in DragAndDrop.objectReferences)
                {
                    var transform = ((GameObject)obj).GetComponent<Transform>();
                    item.childs.Add(SetItems(transform,item));
                    transform.parent = item.transform;
                }
                
            }
        }
    }

    private Vector3 RotateVector(Vector3 vector, Quaternion quat)
    {
        return quat * vector;
    }

    private Vector2 ConvertToViewPlane(ViewType view, Vector3 target)
    {
        Vector2 pos = new Vector2();
        if(view == ViewType.XAxis)
        {
            pos = new Vector2(target.z,target.y);
        }
        else if(view == ViewType.YAxis)
        {
            pos = new Vector2(target.x,target.z);
        }
        else if(view == ViewType.ZAxis)
        {
            pos = new Vector2(target.x,target.y);
        }

        return pos;
    }

    private Vector2 ConvertToCenterCoordinate(Rect rect, Vector2 target)
    {
        return new Vector2(target.x + rect.width * 0.5f, target.y + rect.height * 0.5f);
    }

    private Rect GetAlignmentRect(Rect info, int alignment)
    {
        Vector2 pos = new Vector2();

        int vertical = alignment / 3;
        int horizontal = alignment - vertical * 3;

        pos = new Vector2(info.x - (info.width * (0.5f * (float)vertical)),info.y - (info.height * (0.5f * (float)horizontal)));

        return new Rect(pos.x,pos.y,info.width,info.height);
    }

    private void HorizontalLine(Vector2 margin) => HorizontalLine(Color.gray, 1f, margin);
    private void HorizontalLine(Color color, float height, Vector2 margin)
    {
        GUILayout.Space(margin.x);

        EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, height), color);

        GUILayout.Space(margin.y);
    }

    private void DrawList()
    {
        GUILayout.BeginVertical("box");
        childScrollviewPos = GUILayout.BeginScrollView(childScrollviewPos);

        if(rootObject != null)
            DrawListItem(rootObject,0);

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void DrawListItem(ChildObjectItem item, int deep)
    {
        DrawListItem(item.transform.name,deep++);
        foreach(var child in item.childs)
        {
            DrawListItem(child,deep);
        }
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

    private ChildObjectItem SetItems(Transform root, ChildObjectItem parent)
    {
        ChildObjectItem item = GetChildObjectItemCache();
        item.transform = root;
        item.parent = parent;
        item.childs = GetChildItems(item);

        if(parent == null)
            rootObject = item;
        
        return item;
    }

    private int FindTag(string tag)
    {
        for(int i = 0; i < tagArray.Length; ++i)
        {
            if(tagArray[i] == tag)
                return i;
        }

        return -2;
    }

    private List<ChildObjectItem> GetChildItems(ChildObjectItem root)
    {
        List<ChildObjectItem> list = new List<ChildObjectItem>();

        int count = root.transform.childCount;
        for(int i = 0; i < count; ++i)
        {
            var item = GetChildObjectItemCache();
            item.transform = root.transform.GetChild(i);
            if(item.transform.childCount != 0)
            {
                item.childs = GetChildItems(item);
            }

            list.Add(item);
        }

        return list;
    }

    private void Dispose()
    {
        if(rootObject != null)
            ClearList(rootObject);
        
        objCache.Clear();
        objCache = null;
    }

    private void ClearList(ChildObjectItem item, bool cache = false)
    {
        if(item.childs.Count != 0)
        {
            foreach(var child in item.childs)
            {
                ClearList(child);
            }
        }

        item.transform = null;
        item.childs.Clear();
        item.childs = null;

        if(cache)
            ReturnCache(item);
    }

    private void ReturnCache(ChildObjectItem item)
    {
        objCache.Enqueue(item);
    }

    private ChildObjectItem GetChildObjectItemCache()
    {
        if(objCache.Count == 0)
        {
            CreateChildObjectItem();
        }

        return objCache.Dequeue();
    }

    private void CreateChildObjectItem(int count = 1)
    {
        for(int i = 0; i < count; ++i)
        {
            objCache.Enqueue(new ChildObjectItem());
        }
    }

    private void Reload()
    {
        if(rootObject == null)
            return;
        
        var temp = rootObject.transform;
        ClearList(rootObject);
        SetItems(temp, null);
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
             
                var transform = ((GameObject)DragAndDrop.objectReferences[0]).GetComponent<Transform>();
                if(rootObject != null)
                    ClearList(rootObject);
                SetItems(transform, null);
            }
            break;
        }
    }
}
