using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using TMPro;

public class TextUpdate : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private TextMeshProUGUI chargeText;
    [SerializeField] private ActiveUi aimEnergyBar;

    // Start is called before the first frame update
    void Start()
    {
        ((PlayerCtrl_Ver2)GameManager.Instance.player).loadCount.Subscribe(value =>
        valueText.text = value.ToString());
        ((PlayerCtrl_Ver2)GameManager.Instance.player).chargeTime.Subscribe(value =>
        chargeText.text = ((int)(value * 100f)).ToString());
        GameManager.Instance.player.energy.Subscribe(value => aimEnergyBar.SetValue(value));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
