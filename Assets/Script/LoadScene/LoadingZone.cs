using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingZone : MonoBehaviour
{
    private bool _isLoaded = false;
    private AsynSceneManager _sceneManager;
    private Animator _animator;
    public void Start()
    {
        _sceneManager = GameObject.FindObjectOfType<AsynSceneManager>();
        _sceneManager.RegisterBeforeLoadOnStart(BeforeLoading);
        _sceneManager.RegisterAfterLoadOnStart(AfterLoading);

        _animator = GetComponent<Animator>();
    }

    public void LoadNextScene()
    {
        StartCoroutine(LoadNextSceneCoroutine());
    }

    public IEnumerator LoadNextSceneCoroutine()
    {
        WaitForSeconds se = new WaitForSeconds(2f);
        yield return se;
        _sceneManager.LoadNextlevel();
    }

    public void Open()
    {
        _animator.SetTrigger("OpenTrigger");
    }

    public void Close()
    {
        _animator.SetTrigger("CloseTrigger");
    }

    public void BeforeLoading()
    {
        if(!_isLoaded)
        {
            DontDestroyOnLoad(this.gameObject);
            GameManager.Instance.player.transform.SetParent(this.transform);
            Camera.main.transform.SetParent(this.transform);    
        }
        else
        {
            _sceneManager.CancelBeforeLoad(BeforeLoading);
            _sceneManager.CancelAfterLoad(AfterLoading);

            _sceneManager = null;

            Destroy(this.gameObject);
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

        StartCoroutine(Closeloading());
    }

    IEnumerator Closeloading()
    {
        WaitForSeconds se = new WaitForSeconds(1f);
        yield return se;

        Open();
    }
}
