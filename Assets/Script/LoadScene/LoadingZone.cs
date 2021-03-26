using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingZone : MonoBehaviour
{
    private bool _isLoaded = false;
    private AsynSceneManager _sceneManager;
    public void Start()
    {
        _sceneManager = GameObject.FindObjectOfType<AsynSceneManager>();
        _sceneManager.RegisterBeforeLoadOnStart(BeforeLoading);
        _sceneManager.RegisterAfterLoadOnStart(AfterLoading);
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.O) && !_isLoaded)
        {
            _sceneManager.LoadNextlevel();
        }
    }

    public void BeforeLoading()
    {
        DontDestroyOnLoad(this.gameObject);

        if(!_isLoaded)
        {
            GameManager.Instance.player.transform.SetParent(this.transform);
            Camera.main.transform.SetParent(this.transform);    
        }
        else
        {
            _sceneManager.CancelBeforeLoad(BeforeLoading);
            _sceneManager.CancelAfterLoad(AfterLoading);

            _sceneManager = null;
        }
        
    }

    public void AfterLoading()
    {
        GameManager.Instance.player.transform.SetParent(this.transform);
        Camera.main.transform.SetParent(this.transform); 
        SceneManager.MoveGameObjectToScene(this.gameObject, SceneManager.GetActiveScene());

        var stage = GameObject.FindObjectOfType<StageManager>();
        if(stage != null)
        {
            GameObject.FindObjectOfType<StageManager>().RegisterLoadingZone(this);
        }
        
        GameManager.Instance.player.transform.SetParent(null);
        Camera.main.transform.SetParent(null);
        
        _isLoaded = true;
    }
}
