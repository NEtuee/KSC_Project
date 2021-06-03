using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    public bool isOver = false;
    public TutorialType tutorialType;
    private OptionMenuCtrl _uiManager;
    
    void Start()
    {
        if (TryGetComponent<Collider>(out Collider collider) == false)
        {
            Debug.LogWarning("Not Exist Collider");
            return;
        }

        if (GameManager.Instance == null)
        {
            Debug.LogWarning("Not Exist GameManager");
            return;
        }
        
        if (GameManager.Instance.optionMenuCtrl == null)
        {
            Debug.LogWarning("Not Exist UiManager");
            return;
        }

        _uiManager = GameManager.Instance.optionMenuCtrl;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isOver)
            return;
        
        if (other.CompareTag("Player") == false)
            return;

        //if (_uiManager.TutorialEvent(key) == false)
        //    return;
        _uiManager.InGameTutorial(tutorialType);
        
        isOver = true;
        gameObject.SetActive(false);
    }
}
