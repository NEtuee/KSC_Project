using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImmortalJirungE_V2_AI : MonoBehaviour
{
    public List<IKLegMovement> legs = new List<IKLegMovement>();

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.L))
        {
            foreach(var leg in legs)
            {
                leg.Hold(true,false);
            }
        }

        if(Input.GetKeyDown(KeyCode.K))
        {
            foreach(var leg in legs)
            {
                leg.Hold(false,false);
            }
        }
    }
}
