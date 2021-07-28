using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneManagerEx : ManagerBase
{
    public SceneInfoItem sceneInfo;

    public int currentLevel = 0;

    //public StageManager currentStageManager;
    //public SceneLoadUI sceneLoadUI;

    public string nullScene;

    private SceneInfoItem.SceneInformation _currentScene;
    private List<Scene> _unloadScenes = new List<Scene>();

    private bool _isLoaded = true;
    private int _loadedScenes = 0;

    public override void Assign()
    {
        SaveMyNumber("SceneManager");

        AddAction(MessageTitles.scene_loadCurrentLevel,LoadCurrentLevel);
        AddAction(MessageTitles.scene_loadPrevLevel,LoadPrevlevel);
        AddAction(MessageTitles.scene_loadNextLevel,LoadNextLevel);
    }

    public override void Initialize()
    {
        LoadCurrentLevel();
    }

    public override void Progress(float deltaTime)
    {
        if(Keyboard.current.kKey.wasPressedThisFrame)
        {
            SendMessageEx(MessageTitles.scene_loadNextLevel,GetSavedNumber("SceneManager"),null);
        }
    }


#region messageCallback

    public void LoadCurrentLevel(Message msg)
    {
        LoadCurrentLevel();
    }

    public void LoadNextLevel(Message msg)
    {
        LoadNextlevel();
    }

    public void LoadPrevlevel(Message msg)
    {
        LoadPrevlevel();
    }

#endregion



    public void LoadLevel(int level)
    {
        if(!_isLoaded)
            return;
            
        currentLevel = level;
        
        _currentScene = sceneInfo.FindScene(currentLevel);
        StartCoroutine(SceneLoadingProgress(true));
    }

    public void LoadCurrentLevel()
    {
        LoadLevel(currentLevel);
        // if(!_isLoaded)
        //     return;

        // _currentScene = sceneInfo.FindScene(currentLevel);
        // StartCoroutine(SceneLoadingProgress(true));
    }

    public void LoadPrevlevel()
    {
        var level = (currentLevel - 1 < 0 ? sceneInfo.scenes.Count - 1 : currentLevel - 1);
        
        LoadLevel(level);
    }

    public void LoadNextlevel()
    {
        var level = (currentLevel + 1 >= sceneInfo.scenes.Count ? 0 : currentLevel + 1);

        LoadLevel(level);
    }

    // public LevelInfo FindLevel(string code)
    // {
    //     return levels.Find(x => x.levelCode == code);
    // }

    IEnumerator SceneLoadingProgress(bool setPos)
    {
        if(!_isLoaded)
            yield break;

        _isLoaded = false;


        SendBroadcastMessage(MessageTitles.scene_beforeSceneChange,_currentScene,false);



        StartCoroutine(LoadNullScene());

        _loadedScenes = _unloadScenes.Count;

        for(int i = 0; i < _unloadScenes.Count; ++i)
        {
            StartCoroutine(UnloadSceneCoroutine(_unloadScenes[i]));
        }

        while(_loadedScenes != 0)
        {
            yield return null;
        }

        _unloadScenes.Clear();


        _loadedScenes = _currentScene.targetScenes.Count;

        for(int i = 0; i < _currentScene.targetScenes.Count; ++i)
        {
            StartCoroutine(LoadSceneCoroutine(setPos,i == 0,_currentScene.targetScenes[i]));
        }
        
        while(_loadedScenes != 0)
        {
            yield return null;
        }


        Debug.Log("Load");

        SendBroadcastMessage(MessageTitles.scene_afterSceneChange,_currentScene,false);

        Debug.Log("AfterLoad");

        yield return CoroutineUtilities.WaitForRealTime(2f);

        StartCoroutine(UnLoadNullScene());

        yield return CoroutineUtilities.WaitForRealTime(3f);


        _isLoaded = true;

        SendBroadcastMessage(MessageTitles.scene_sceneChanged,_currentScene,false);
        Debug.Log("Load Complite");
    }

    IEnumerator LoadNullScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(nullScene,LoadSceneMode.Additive);
        operation.allowSceneActivation = false;
        
        while (operation.isDone == false)
        {
            if (operation.progress >= 0.9f)
            {
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }

    IEnumerator UnLoadNullScene()
    {
        AsyncOperation operation = SceneManager.UnloadSceneAsync(nullScene);
        operation.allowSceneActivation = false;

        while(operation.isDone == false)
        {
            yield return null;
        }

        operation.allowSceneActivation = true;
    }

    IEnumerator UnloadSceneCoroutine(Scene scene)
    {
        AsyncOperation operation = SceneManager.UnloadSceneAsync(scene);
        operation.allowSceneActivation = false;

        while(operation.isDone == false)
        {
            yield return null;
        }

        operation.allowSceneActivation = true;
        --_loadedScenes;
    }

    IEnumerator LoadSceneCoroutine(bool setPos, bool sceneActive, string target)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(target,LoadSceneMode.Additive);
        operation.allowSceneActivation = false;
        
        while (operation.isDone == false)
        {
            if (operation.progress >= 0.9f)
                operation.allowSceneActivation = true;

            if (sceneActive == true)
            {
                SendMessageEx(MessageTitles.uimanager_setloadinggagevalue, GetSavedNumber("UIManager"), operation.progress);
            }

            yield return null;
        }

        if (sceneActive == true)
            SendMessageEx(MessageTitles.uimanager_setloadinggagevalue, GetSavedNumber("UIManager"), 1f);

        var scene = SceneManager.GetSceneByName(target);

        if(sceneActive)
            SceneManager.SetActiveScene(scene);
        _unloadScenes.Add(scene);

        //var stage = GameObject.FindObjectOfType<StageManager>();

        // if(stage != null)
        // {
            
        // }

        --_loadedScenes;
    }

}
