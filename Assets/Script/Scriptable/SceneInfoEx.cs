using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
#endif

[CreateAssetMenu(fileName = "SceneSet", menuName = "Options/SceneSet")]
public class SceneInfoEx : ScriptableObject
{
    [System.Serializable]
    public class TargetItem
    {
        [SerializeField]
        public Object target;

        [SerializeField]
        public bool canLoad = true;
    }

    [SerializeField]
    public string setName;

    [SerializeField]
    public List<TargetItem> targetScenes; 

}

#if UNITY_EDITOR

[CustomEditor(typeof(SceneInfoEx))]
public class SceneInfoExEditor : Editor
{
    private SceneInfoEx infoEx;
    public void OnEnable()
    {
        infoEx = (SceneInfoEx)target;
    }
 
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if(GUILayout.Button("Auto Fill"))
        {
            AutoFill();
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }

        if(GUILayout.Button("Add To Build Set"))
        {
            AddToBuildSettings();
            ((BuildScenesSetEx)AssetDatabase.
                LoadAssetAtPath("Assets/Settings/BuildScenesSetEx.asset",typeof(BuildScenesSetEx))).
                AddSceneSet(infoEx);
        }


        GUILayout.Space(10f);


        if(GUILayout.Button("Open Scenes By Additive"))
        {
            LoadScenes(OpenSceneMode.Additive);
        }
        if(GUILayout.Button("Open Scenes By Single"))
        {
            LoadScenes(OpenSceneMode.Single);
        }
    }

    public void LoadScenes(OpenSceneMode mode)
    {
        for(int i = 0; i < infoEx.targetScenes.Count; ++i)
        {
            var path = AssetDatabase.GetAssetPath(infoEx.targetScenes[i].target);
            var scene = EditorSceneManager.OpenScene(path,mode == OpenSceneMode.Single ? 
                                                    (i == 0 ? mode : OpenSceneMode.Additive) : OpenSceneMode.Additive);
            if(i == 0)
            {
                EditorSceneManager.SetActiveScene(scene);
            }
        }
        
    }

    public void AddToBuildSettings()
    {
        var sceneList = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        foreach(var item in infoEx.targetScenes)
        {
            var path = AssetDatabase.GetAssetPath(item.target);

            if(sceneList.Find((x)=>{return x.path == path;}) == null)
            {
                var sceneItem = new EditorBuildSettingsScene(path, true);
                sceneList.Add(sceneItem);
            }
        }

        EditorBuildSettings.scenes = sceneList.ToArray();
    }

    public void AutoFill()
    {
        if(infoEx == null)
            return;
        
        infoEx.targetScenes.Clear();

        var path = AssetDatabase.GetAssetPath(infoEx);
        path = path.Substring(0, path.LastIndexOf('/') + 1);
        path = Application.dataPath.Replace("Assets",path);

        string[] scenes = Directory.GetFiles(path, "*.Unity", SearchOption.TopDirectoryOnly);
        foreach(string scenePath in scenes)
        {
            string assetPath = "Assets" + scenePath.Replace(Application.dataPath, "").Replace('\\', '/');
            SceneAsset scene = (SceneAsset)AssetDatabase.LoadAssetAtPath(assetPath, typeof(SceneAsset));

            var item = new SceneInfoEx.TargetItem();
            item.target = scene;

            infoEx.targetScenes.Add(item);
        }

    }
}

#endif
