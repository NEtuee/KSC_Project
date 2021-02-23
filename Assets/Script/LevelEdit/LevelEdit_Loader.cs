using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEdit_Loader : SingletonMono<LevelEdit_Loader>
{
    [SerializeField]private List<string> loadScenes;
    [SerializeField]private bool loadWhenAwake = true;
    public void Awake()
    {
        SetSingleton(this);
        if(loadWhenAwake)
        {
            foreach(var n in loadScenes)
            {
                SceneLoad(n);
            }
        }
        
    }

    public void SceneLoad(string scene, LoadSceneMode mode = LoadSceneMode.Additive)
    {
        SceneManager.LoadScene(scene, mode);
    }

    public void UnloadScene(string scene)
    {
        SceneManager.UnloadSceneAsync(scene);
    }


}
