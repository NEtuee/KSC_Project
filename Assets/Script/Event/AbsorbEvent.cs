using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AbsorbEvent : MonoBehaviour
{
    [SerializeField]private UnityEvent pierceEvent;
    [SerializeField]private UnityEvent pullEvent;
    [SerializeField]private UnityEvent absorbEndEvent;

    void Start()
    {
        //if(GameObject.FindGameObjectWithTag("Player")!=null)
        //{
        //    PlayerAnimCtrl playerAnim = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerAnimCtrl>();
        //    playerAnim.BindPierceEvent(pierceEvent);
        //    playerAnim.BindPullEvent(pullEvent);
        //    playerAnim.BindAbsorbEndEvent(absorbEndEvent);
        //}
    }

    public UnityEvent GetPierceEvent() { return pierceEvent; }
    public UnityEvent GetPullEvent() { return pullEvent; }
    public UnityEvent GetAbsorbEndEvent() { return absorbEndEvent; }
}
