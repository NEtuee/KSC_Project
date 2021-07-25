using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SceneInformation", menuName = "Options/SceneInformation")]
public class SceneInfoItem : ScriptableObject
{
    [System.Serializable]
    public class SceneInformation
    {
        public string name;

        [SerializeField]
        public List<string> targetScenes; 
    }


    public List<SceneInformation> scenes;


    public SceneInformation FindScene(int position)
    {
        return scenes[position];
    }

    public SceneInformation FindScene(string name)
    {
        return scenes.Find((x)=>{return x.name == name;});
    }


}
