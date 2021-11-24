using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using MD;
public class SceneManagerEx : ManagerBase
{
    public BuildScenesSetEx sceneInfo;

    public int currentLevel = 0;

    //public StageManager currentStageManager;
    //public SceneLoadUI sceneLoadUI;

    public string nullScene;

    public bool loadOnStart = true;

    private SceneInfoEx _currentScene;
    private List<Scene> _unloadScenes = new List<Scene>();

    private bool _isLoaded = true;
    private int _loadedScenes = 0;

    public override void Assign()
    {
        SaveMyNumber("SceneManager");

        AddAction(MessageTitles.scene_loadCurrentLevel,LoadCurrentLevel);
        AddAction(MessageTitles.scene_loadPrevLevel,LoadPrevlevel);
        AddAction(MessageTitles.scene_loadNextLevel,LoadNextLevel);
        AddAction(MessageTitles.scene_loadSpecificLevel,LoadSpecificLevel);
        AddAction(MessageTitles.scene_loadSceneNotAsync, (msg) =>
        {
            StringData data = MessageDataPooling.CastData<StringData>(msg.data);
            StartCoroutine(LoadSceneNotAsync(data.value));
        });

        AddAction(MessageTitles.scene_loadRestartLevel, (msg) =>
         {
             LoadRestart();
         });

    }

    public override void Initialize()
    {
        if(loadOnStart)
            LoadCurrentLevel();
    }

    public override void Progress(float deltaTime)
    {
        //if(Keyboard.current.kKey.wasPressedThisFrame)
        //{
        //    SendMessageEx(MessageTitles.scene_loadNextLevel,GetSavedNumber("SceneManager"),null);
        //}
    }


#region messageCallback

    public void LoadSpecificLevel(Message msg)
    {
        var key = MessageDataPooling.CastData<StringData>(msg.data);
        LoadSpecificLevel(key.value);
    }

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


    public void LoadSpecificLevel(string key)
    {
        if(!_isLoaded)
            return;

        int level = -1;
        for(int i = 0; i < sceneInfo.scenes.Count; ++i)
        {
            if(sceneInfo.scenes[i].setName == key)
            {
                level = i;
            }
        }

        if(level == -1)
        {
            Debug.Log("Level not found");
            return;
        }

        currentLevel = level;
        
        _currentScene = sceneInfo.FindScene(currentLevel);
        StartCoroutine(SceneLoadingProgress(true));
    }

    public void LoadLevel(int level, bool restart = false)
    {
        if(!_isLoaded)
            return;
            
        currentLevel = level;
        
        _currentScene = sceneInfo.FindScene(currentLevel);
        StartCoroutine(SceneLoadingProgress(true, restart));
    }

    public void LoadCurrentLevel()
    {
        LoadLevel(currentLevel);
        // if(!_isLoaded)
        //     return;

        // _currentScene = sceneInfo.FindScene(currentLevel);
        // StartCoroutine(SceneLoadingProgress(true));
    }

    public void LoadRestart()
    {
        LoadLevel(currentLevel, true);
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

    IEnumerator SceneLoadingProgress(bool setPos ,bool restart = false)
    {
        if(!_isLoaded)
            yield break;

        _isLoaded = false;


        SendBroadcastMessage(MessageTitles.scene_beforeSceneChange,_currentScene,false);

        yield return CoroutineUtilities.WaitForRealTime(2f);

        BoolData timeStop = MessageDataPooling.GetMessageData<BoolData>();
        timeStop.value = false;
        SendMessageEx(MessageTitles.timemanager_timestop, GetSavedNumber("TimeManager"), timeStop);

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
            if(_currentScene.targetScenes[i].canLoad)
                StartCoroutine(LoadSceneCoroutine(setPos,i == 0,_currentScene.targetScenes[i].targetName));
            else
                --_loadedScenes;
        }
        
        while(_loadedScenes != 0)
        {
            yield return null;
        }


        SendBroadcastMessage(MessageTitles.scene_afterSceneChange,_currentScene,false);
        StartCoroutine(UnLoadNullScene());

        yield return CoroutineUtilities.WaitForRealTime(2f);


        _isLoaded = true;

        if(restart)
            SendBroadcastMessage(MessageTitles.scene_restarted, _currentScene, false);

        SendBroadcastMessage(MessageTitles.scene_sceneChanged,_currentScene,false);
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
                FloatData data = MessageDataPooling.GetMessageData<FloatData>();
                data.value = operation.progress;
                SendMessageEx(MessageTitles.uimanager_setloadinggagevalue, GetSavedNumber("UIManager"), data);
            }

            yield return null;
        }

        if (sceneActive == true)
        {
            FloatData data = MessageDataPooling.GetMessageData<FloatData>();
            data.value = 1f;
            SendMessageEx(MessageTitles.uimanager_setloadinggagevalue, GetSavedNumber("UIManager"), data);
        }

        var scene = SceneManager.GetSceneByName(target);

        if(sceneActive)
        {
            SceneManager.SetActiveScene(scene);
            LightProbes.Tetrahedralize();
        }
        _unloadScenes.Add(scene);

        //var stage = GameObject.FindObjectOfType<StageManager>();

        // if(stage != null)
        // {
            
        // }

        --_loadedScenes;
    }

    IEnumerator LoadSceneNotAsync(string sceneName)
    {
        SendBroadcastMessage(MessageTitles.scene_beforeSceneChangeNotAsync, null, false);

        yield return null;

        SceneManager.LoadScene(sceneName);
    }
}
