using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerCtrl : MonoBehaviour
{
    [SerializeField] protected bool isPause;
    public FloatReactiveProperty stamina = new FloatReactiveProperty(100);
    public FloatReactiveProperty hp = new FloatReactiveProperty(100f);
    public FloatReactiveProperty charge = new FloatReactiveProperty(0.0f);
    public FloatReactiveProperty energy = new FloatReactiveProperty(0.0f);

    public delegate void WhenPlayerDead();

    public WhenPlayerDead whenPlayerDead;
    
    public void Pause() { isPause = true; }

    public void PauseControl(bool result) { isPause = result; }
    public void Resume() { isPause = false; }

    public virtual void TakeDamage(float damage)
    {
        hp.Value -= damage;
    }

    public virtual void InitStatus()
    {
        stamina.Value = 100.0f;
        hp.Value = 100.0f;
        energy.Value = 0.0f;
    }
}
