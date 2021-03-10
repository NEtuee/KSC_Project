using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using TMPro;

public class TextUpdate : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI valueText;

    // Start is called before the first frame update
    void Start()
    {
        ((PlayerCtrl_Ver2)GameManager.Instance.player).loadCount.Subscribe(value =>
        valueText.text = value.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
