using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using TMPro;

public class GunStateUi : MonoBehaviour
{
    [SerializeField] private Canvas stateUi;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private TextMeshProUGUI chargeText;
    [SerializeField] private ActiveUi aimEnergyBar;

    // Start is called before the first frame update
    void Start()
    {
        PlayerCtrl_Ver2 player = (PlayerCtrl_Ver2) GameManager.Instance.player;

        player.loadCount.Subscribe(value =>
            valueText.text = value.ToString());
        player.chargeTime.Subscribe(value =>
            chargeText.text = ((int) (value * 100f)).ToString());
        GameManager.Instance.player.energy.Subscribe(value => aimEnergyBar.SetValue(value));

        Active(false);
        player.activeAimEvent += () => { Active(true); };
        player.releaseAimEvent += () => { Active(false); };
    }

    private void Active(bool active)
    {
        stateUi.enabled = active;
    }
}
