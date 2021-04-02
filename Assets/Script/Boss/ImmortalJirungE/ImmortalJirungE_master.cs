using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmortalJirungE_master : MonoBehaviour
{
    public List<ImmortalJirungE_AI> aIs;

    private int shieldCount = 0;

    public void Recovery()
    {
        shieldCount = aIs.Count;
    }

    public void DecreaseShieldCount()
    {
        --shieldCount;
        if(shieldCount == 0)
        {
            foreach(var ai in aIs)
            {
                ai.ChangeState(ImmortalJirungE_AI.State.Stun);

                Recovery();
            }
        }
    }
}
