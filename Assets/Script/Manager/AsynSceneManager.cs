using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsynSceneManager : MonoBehaviour
{
    public delegate void del_SceneLoaded();

    public List<string> levels = new List<string>();
    public int currentLevel = 0;

    private string _currentScene;
    private List<string> _unloadScenes = new List<string>();

    private List<del_SceneLoaded> _afterLoadRegisterLine = new List<del_SceneLoaded>();
    private List<del_SceneLoaded> _beforeLoadRegisterLine = new List<del_SceneLoaded>();
    private bool _isLoaded = false;
    private int _loadedScenes = 0;
    
    private del_SceneLoaded _beforeLoad = ()=>{};
    private del_SceneLoaded _afterLoad = ()=>{};

    public void Start()
    {
        LoadCurrentlevel();
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            LoadCurrentlevel();
        }
    }

    public void LoadLevel(int level)
    {
        currentLevel = level;
        RegisterProgress();
        StartCoroutine(SceneLoadingProgress(true));
    }

    public void LoadCurrentlevel()
    {
        _currentScene = levels[currentLevel];
        RegisterProgress();
        StartCoroutine(SceneLoadingProgress(true));
    }

    public void LoadNextlevel()
    {
        currentLevel = (++currentLevel >= levels.Count ? 0 : currentLevel);
        _currentScene = levels[currentLevel];

        StartCoroutine(SceneLoadingProgress(false));
    }

    IEnumerator SceneLoadingProgress(bool setPos)
    {
        _beforeLoad();

        _loadedScenes = _unloadScenes.Count;

        Debug.Log(_loadedScenes);
        for(int i = 0; i < _unloadScenes.Count; ++i)
        {
            StartCoroutine(UnloadSceneCoroutine(_unloadScenes[i]));
        }

        while(_loadedScenes != 0)
        {
            yield return null;
        }

        _unloadScenes.Clear();

        StartCoroutine(LoadSceneCoroutine(setPos));
    }

    IEnumerator UnloadSceneCoroutine(string scene)
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

    IEnumerator LoadSceneCoroutine(bool setPos)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(_currentScene,LoadSceneMode.Additive);
        operation.allowSceneActivation = false;
        
        while (operation.isDone == false)
        {
            if (operation.progress >= 0.9f)
                operation.allowSceneActivation = true;

            yield return null;
        }

        _afterLoad();

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(_currentScene));
        _unloadScenes.Add(_currentScene);

        RegisterProgress();

        if(setPos)
        {
            var stage = GameObject.FindObjectOfType<StageManager>();
            if(stage != null)
            {
                GameObject.FindObjectOfType<StageManager>().SetPlayerToPosition();
            }
        }
    }

    public void RegisterBeforeLoadOnStart(del_SceneLoaded func){_beforeLoadRegisterLine.Add(func);}
    public void RegisterAfterLoadOnStart(del_SceneLoaded func){_afterLoadRegisterLine.Add(func);}

    public void RegisterBeforeLoad(del_SceneLoaded func){_beforeLoad += func;}
    public void RegisterAfterLoad(del_SceneLoaded func){_afterLoad += func;}
    public void CancelBeforeLoad(del_SceneLoaded func){_beforeLoad -= func;}
    public void CancelAfterLoad(del_SceneLoaded func){_afterLoad -= func;}

    public void RegisterProgress()
    {
        foreach(var func in _afterLoadRegisterLine)
        {
            _afterLoad += func;
        }
        foreach(var func in _beforeLoadRegisterLine)
        {
            _beforeLoad += func;
        }

        _afterLoadRegisterLine.Clear();
        _beforeLoadRegisterLine.Clear();
    }

}
