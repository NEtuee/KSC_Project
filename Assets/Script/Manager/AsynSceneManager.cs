using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsynSceneManager : MonoBehaviour
{
    public struct LocalInfo
    {
        public Vector3 localPosition;
        public Vector3 localSize;
        public Quaternion localRotation;

        public LocalInfo(Vector3 lp,Vector3 ls,Quaternion lr)
        {
            localPosition = lp;
            localSize = ls;
            localRotation = lr;
        }
    };

    public delegate void del_SceneLoaded();

    public List<string> levels = new List<string>();
    public int currentLevel = 0;

    public StageManager currentStageManager;

    private string _currentScene;
    private List<Scene> _unloadScenes = new List<Scene>();

    private List<del_SceneLoaded> _afterLoadRegisterLine = new List<del_SceneLoaded>();
    private List<del_SceneLoaded> _beforeLoadRegisterLine = new List<del_SceneLoaded>();
    private bool _isLoaded = true;
    private int _loadedScenes = 0;
    
    private del_SceneLoaded _beforeLoad = ()=>{};
    private del_SceneLoaded _afterLoad = ()=>{};

    private LocalInfo _playerLocalTarget;
    private LocalInfo _cameraLocalTarget;
    private LocalInfo _followLocalTarget;

    private PlayerCtrl _player;
    private Camera _cam;
    private Transform _follow;

    public void Start()
    {
        _cam = Camera.main;
        _follow = GameManager.Instance.followTarget.transform;
        _player = GameManager.Instance.player;

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

    public void LoadPrevlevel()
    {
        currentLevel = (--currentLevel < 0 ? levels.Count - 1 : currentLevel);
        _currentScene = levels[currentLevel];
        RegisterProgress();
        StartCoroutine(SceneLoadingProgress(true));
    }

    public void LoadNextlevelFrom()
    {
        currentLevel = (++currentLevel >= levels.Count ? 0 : currentLevel);
        _currentScene = levels[currentLevel];
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
        if(!_isLoaded)
            yield break;

        GameManager.Instance.PAUSE = true;
        _isLoaded = false;

        if(currentStageManager != null)
            UpdateLocalTargets(currentStageManager.exitElevator.transform);

        SetTargetObjectParent(null);
        
        DontDestroyOnLoad(Camera.main.transform);
        DontDestroyOnLoad(GameManager.Instance.followTarget.transform);
        DontDestroyOnLoad(GameManager.Instance.player.transform);

        _beforeLoad();

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

        StartCoroutine(LoadSceneCoroutine(setPos));
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

        var scene = SceneManager.GetSceneByName(_currentScene);
        SceneManager.SetActiveScene(scene);
        _unloadScenes.Add(scene);

        RegisterProgress();

        var stage = GameObject.FindObjectOfType<StageManager>();

        if(stage != null)
        {
            if(setPos)
            {
                stage.ObjectTeleportToLoadedPos(_player.transform,_player.transform.position);
                stage.ObjectTeleportToLoadedPos(_cam.transform,_player.transform.position);
                stage.ObjectTeleportToLoadedPos(_follow,_player.transform.position);
            }
            else
            {
                stage.entranceElevator.ObjectTeleport(_playerLocalTarget.localPosition,_playerLocalTarget.localRotation,_player.transform);
                stage.entranceElevator.ObjectTeleport(_cameraLocalTarget.localPosition,_cameraLocalTarget.localRotation,_cam.transform);
                stage.entranceElevator.ObjectTeleport(_followLocalTarget.localPosition,_followLocalTarget.localRotation,_follow.transform);
            }
            
        }

        currentStageManager = stage;
        GameManager.Instance.PAUSE = false;
        _isLoaded = true;
    }

    public void UpdateLocalTargets(Transform target)
    {
        var player = _player.transform;
        var cam = _cam.transform;
        var follow = _follow.transform;

        SetTargetObjectParent(target);
        _playerLocalTarget = new LocalInfo(player.localPosition,player.localScale,player.localRotation);
        _cameraLocalTarget = new LocalInfo(cam.localPosition,cam.localScale,cam.localRotation);
        _followLocalTarget = new LocalInfo(follow.localPosition,follow.localScale,follow.localRotation);
    }

    public void SetTargetObjectParent(Transform parent)
    {
        _cam.transform.SetParent(parent);
        _follow.transform.SetParent(parent);
        _player.transform.SetParent(parent);
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
