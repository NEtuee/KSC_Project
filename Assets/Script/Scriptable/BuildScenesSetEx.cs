using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "BuildScenesSetEx", menuName = "Options/BuildScenesSetEx")]
public class BuildScenesSetEx : ScriptableObject
{
    [SerializeField]
    public List<SceneInfoEx> scenes;

    public SceneInfoEx FindScene(int position)
    {
        return scenes[position];
    }

    public SceneInfoEx FindScene(string name)
    {
        return scenes.Find((x)=>{return x.setName == name;});
    }

    public void AddSceneSet(SceneInfoEx data)
    {
        if(data == null)
        {
            Debug.Log("Data is missing");
            return;
        }
        else if(data.setName == "")
        {
            Debug.Log("Data name is NULL");
            return;
        }

        if(scenes.Find((x)=>{return x.setName == data.setName;}) == null)
        {
            scenes.Add(data);
#if UNITY_EDITOR
            EditorUtility.SetDirty(this);
#endif
        }
        else
        {
            Debug.Log("Data is already Exsits");
        }
    }
}
