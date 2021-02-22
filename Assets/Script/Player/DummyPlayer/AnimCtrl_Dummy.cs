using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimCtrl_Dummy : MonoBehaviour
{
    private PlayerCtrl_Dummy owner;
    private void Start()
    {
        owner=GetComponent<PlayerCtrl_Dummy>();
    }
    private void EndTurnBack()
    {
        owner.ChangeState(PlayerCtrl_Dummy.DummyState.Default);
    }

    private void EndRunToStop()
    {
        owner.ChangeState(PlayerCtrl_Dummy.DummyState.Default);
    }
}
