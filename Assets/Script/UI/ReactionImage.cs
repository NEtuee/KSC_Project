using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ReactionImage : MonoBehaviour
{
    [SerializeField] private Image _targetGrahpic;

    [SerializeField] private Sprite _onHighLight;
    [SerializeField] private Sprite _offHighLight;

    public UnityEvent onFocus;
    public UnityEvent onDefocus;

    private void Start()
    {
        OnDefocus();   
    }

    public void OnFocus()
    {
        _targetGrahpic.sprite = _onHighLight;
        onFocus.Invoke();
    }

    public void OnDefocus()
    {
        _targetGrahpic.sprite = _offHighLight;
        onDefocus.Invoke();
    }
}
