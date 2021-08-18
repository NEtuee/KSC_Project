using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MenuPage : MonoBehaviour
{
    public List<GameObject> uiElements = new List<GameObject>();    
    [SerializeField] private Canvas _canvas;
    [SerializeField] private CanvasGroup _canvasGroup;

    private bool _isFirst = false;

    public UnityEvent onFisrtActive;
    public UnityEvent onActive;
    public UnityEvent onDisable;

    public List<Appearable> appearables = new List<Appearable>();


    protected void Awake()
    {
        _canvas = GetComponent<Canvas>();
        if(_canvas == null)
        {
            Debug.LogWarning("Not Exist Canvas");
            return;
        }

        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            Debug.LogWarning("Not Exist CanvasGroup");
            return;
        }
    }

    private void Start()
    {
        Init();

        //onFisrtActive.Invoke();
    }

    public void Init()
    {
        foreach (var uiObject in uiElements)
        {
            if (uiObject.TryGetComponent<Appearable>(out Appearable appearable))
            {
                appearables.Add(appearable);
            }
        }

        foreach (var ui in appearables)
        {
            ui.Init();
        }

        _canvas.enabled = false;
        _canvasGroup.interactable = false;
        _canvasGroup.blocksRaycasts = false;
    }

    public void Active(bool active)
    {
        if (active)
        {
            onActive.Invoke();
            if (_isFirst == false)
            {
                _isFirst = true;
                onFisrtActive.Invoke();
            }
        }
        else
        {
            onDisable.Invoke();
        }

        _canvas.enabled = active;
        _canvasGroup.interactable = active;
        _canvasGroup.blocksRaycasts = active;
    }
}
