using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "SceneInformation", menuName = "Options/SceneInformation")]
public class SceneInfoItem : ScriptableObject
{
    [SerializeField]
    public class SceneInformation
    {
        public string name;

        [SerializeField]
        public List<Scene> targetScenes; 
    }

    [SerializeField]
    public List<SceneInformation> scenes = new List<SceneInformation>();

    public SceneInformation FindScene(int position)
    {
        return scenes[position];
    }

    public SceneInformation FindScene(string name)
    {
        return scenes.Find((x)=>{return x.name == name;});
    }


}
