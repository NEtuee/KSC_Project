using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Elevator : MonoBehaviour
{
    public bool isExit = false;

    private AsynSceneManager _sceneManager;
    private Animator _animator;

    public void Start()
    {
        _sceneManager = GameObject.FindObjectOfType<AsynSceneManager>();
        _animator = GetComponent<Animator>();

        if(!isExit)
        {
            Open();
        }
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

    public void ObjectTeleport(Vector3 localPos, Quaternion localRot, Transform target, bool attatch = false)
    {
        target.SetParent(this.transform);
        target.localPosition = localPos;
        target.rotation = localRot;

        if(!attatch)
        {
            target.SetParent(null); 
        }
    }

    public void ObjectTeleport(Elevator inElevator, Transform target, bool attatch = false)
    {
        target.SetParent(inElevator.transform);
        
        var localPos = target.localPosition;
        var localRot = target.localRotation;

        ObjectTeleport(localPos,localRot,target,attatch);
    }
}
