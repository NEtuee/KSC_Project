using UnityEngine;
using UnityEditor;
using System.Collections;
using UnityEditorInternal;
using System.Collections.Generic;

public class BuildSceneEditor : EditorWindow
{
    private static BuildScenesSetEx _setData;
    private TriggerEx _openTriggers = new TriggerEx();
    private TriggerEx _setTriggers = new TriggerEx();

    [MenuItem("CustomWindow/Build Scene Editor")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(BuildSceneEditor),false,"Build Scene View");
        _setData = (BuildScenesSetEx)AssetDatabase.LoadAssetAtPath("Assets/Settings/BuildScenesSetEx.asset",typeof(BuildScenesSetEx));
    }

    void OnGUI()
    {
        if(_setData == null)
        {
            Debug.Log("Set data is missing");
            Close();
            return;
        }

        foreach(var item in _setData.scenes)
        {
            ShowSceneSet(item);
        }
    }

    public void ShowSceneSet(SceneInfoEx data)
    {
        var trigger = _openTriggers.GetTrigger(data.setName);
        var setTrigger = _setTriggers.GetTrigger(data.setName,true);

        GUILayout.BeginHorizontal();
        if(GUILayout.Button(trigger ? "▼" : "▶",GUILayout.Width(25f)))
        {
            trigger = !trigger;
            _openTriggers.SetTrigger(data.setName,trigger);
        }

        if(GUILayout.Button("···",GUILayout.Width(25f)))
        {
            Selection.activeObject = data;
        }

        setTrigger = GUILayout.Toggle(setTrigger,data.setName);
        if(_setTriggers.SetTrigger(data.setName,setTrigger))
        {
            var sceneTarget = EditorBuildSettings.scenes;
            foreach(var item in data.targetScenes)
            {
                var path = AssetDatabase.GetAssetPath(item.target);
                
                foreach(var scene in sceneTarget)
                {
                    if(scene.path == path)
                    {
                        scene.enabled = setTrigger;
                        item.canLoad = setTrigger;
                        break;
                    }
                }
    
    
            }
    
            EditorBuildSettings.scenes = sceneTarget;
        }

        GUILayout.EndHorizontal();

        // if(!trigger)
        //     return;
        
        var scenes = EditorBuildSettings.scenes;
        int stateCount = 0;
        
        foreach(var item in data.targetScenes)
        {
            var path = AssetDatabase.GetAssetPath(item.target);
            
            foreach(var scene in scenes)
            {
                if(scene.path == path)
                {
                    if(trigger)
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Space(25f);
                        if(GUILayout.Button("···",GUILayout.Width(25f)))
                        {
                            Selection.activeObject = item.target;
                        }
                        scene.enabled = GUILayout.Toggle(scene.enabled,item.target.name);
                        item.canLoad = scene.enabled;
                        GUILayout.EndHorizontal();
                    }
                    


                    stateCount = scene.enabled ? stateCount + 1 : stateCount;

                    // if(scene.enabled && !setTrigger)
                    // {
                    //     _setTriggers.SetTrigger(data.setName,true);
                    // }
                    // else if(!scene.enabled && setTrigger)
                    // {
                    //     _setTriggers.SetTrigger(data.setName,false);
                    // }

                    break;
                }
            }

        }

        _setTriggers.SetTrigger(data.setName,stateCount != 0);
        EditorBuildSettings.scenes = scenes;
        

    }
}
