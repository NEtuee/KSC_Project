using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class BoneStructViewer : EditorWindow
{
    private class ChildObjectItem
    {
        public Transform transform;
        public List<ChildObjectItem> childs = new List<ChildObjectItem>();
    }

    private Queue<ChildObjectItem> objCache = new Queue<ChildObjectItem>();
    private ChildObjectItem lootObject;

    private Vector2 childScrollviewPos;
    private Vector2 structureScrollviewPos;

    private float viewScale = 1f;

    private Vector2 objectSize = new Vector2(10f,10f);


    [MenuItem("CustomWindow/ChildStructureViewer")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(BoneStructViewer),false,"ChildStructureViewer");

    }

    void OnGUI()
    {
        EditorGUILayout.Space ();
        DropAreaGUI ();
        EditorGUILayout.BeginVertical();

        HorizontalLine(new Vector2(20f, 20f));

        EditorGUILayout.EndVertical();

        GUI.Box(new Rect(5,100,position.width - 15, position.height * .5f),"");
        structureScrollviewPos = GUI.BeginScrollView(new Rect(5, 100, position.width - 15, position.height * .5f), 
                                                structureScrollviewPos, new Rect(0, 0, 300, 200));
        GUI.Button(new Rect(0, 0, 100, 20), "Top-left");
        GUI.Button(new Rect(200, 0, 100, 20), "Top-right");
        GUI.Button(new Rect(0, 180, 100, 20), "Bottom-left");
        GUI.Button(new Rect(120, 180, 100, 20), "Bottom-right");
        GUI.EndScrollView();

        //GUILayout.EndVertical();

        // GUILayout.Space(5f);
        // DrawTest();

    }

    void OnDestroy()
    {
        Dispose();
    }

    public void HorizontalLine(Vector2 margin) => HorizontalLine(Color.gray, 1f, margin);
    public void HorizontalLine(Color color, float height, Vector2 margin)
    {
        GUILayout.Space(margin.x);

        EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, height), color);

        GUILayout.Space(margin.y);
    }

    private void DrawTest()
    {
        GUILayout.BeginVertical("box");
        childScrollviewPos = GUILayout.BeginScrollView(childScrollviewPos);

        if(lootObject != null)
            DrawItem(lootObject,0);

        GUILayout.EndScrollView();
        GUILayout.EndVertical();
    }

    private void DrawItem(ChildObjectItem item, int deep)
    {
        Draw(item.transform.name,deep++);
        foreach(var child in item.childs)
        {
            DrawItem(child,deep);
        }
    }

    private void Draw(string name, int deep)
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

    private void SetItems(Transform loot)
    {
        ChildObjectItem item = GetChildObjectItemCache();
        item.transform = loot;
        item.childs = GetChildItems(item);

        lootObject = item;
    }

    private List<ChildObjectItem> GetChildItems(ChildObjectItem loot)
    {
        List<ChildObjectItem> list = new List<ChildObjectItem>();

        int count = loot.transform.childCount;
        for(int i = 0; i < count; ++i)
        {
            var item = GetChildObjectItemCache();
            item.transform = loot.transform.GetChild(i);
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
        if(lootObject != null)
            ClearList(lootObject);
        
        objCache.Clear();
        objCache = null;
    }

    private void ClearList(ChildObjectItem item)
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
        //ReturnCache(item);
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

    private void DropAreaGUI ()
    {
        Event evt = Event.current;
        Rect drop_area = GUILayoutUtility.GetRect (0.0f, 50.0f, GUILayout.ExpandWidth (true));
        GUI.Box (drop_area, "Add Trigger");
     
        switch (evt.type) {
        case EventType.DragUpdated:
        case EventType.DragPerform:
            if (!drop_area.Contains (evt.mousePosition))
                return;
             
            DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
         
            if (evt.type == EventType.DragPerform) {
                DragAndDrop.AcceptDrag ();
             
                var transform = ((GameObject)DragAndDrop.objectReferences[0]).GetComponent<Transform>();
                if(lootObject != null)
                    ClearList(lootObject);
                SetItems(transform);
            }
            break;
        }
    }
}
