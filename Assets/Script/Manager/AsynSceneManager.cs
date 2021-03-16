using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AsynSceneManager : MonoBehaviour
{
    public string unloadScene;
    public string loadScene;

    public GameObject wall;
    private bool isOver = false;
    
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    IEnumerator UnloadSceneCoroutine()
    {
        AsyncOperation operation = SceneManager.UnloadSceneAsync(unloadScene);
        operation.allowSceneActivation = false;

        while(operation.isDone == false)
        {
            yield return null;
        }

        operation.allowSceneActivation = true;

        StartCoroutine(LoadSceneCoroutine());
    }

    IEnumerator LoadSceneCoroutine()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(loadScene,LoadSceneMode.Additive);
        operation.allowSceneActivation = false;

        while (operation.isDone == false)
        {
            if (operation.progress >= 0.9f)
                operation.allowSceneActivation = true;

            yield return null;
        }
        wall.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isOver == false)
        {
            isOver = true;
            wall.SetActive(true);
            StartCoroutine(UnloadSceneCoroutine());
        }
    }
}
