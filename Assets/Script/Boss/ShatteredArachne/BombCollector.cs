using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombCollector : MonoBehaviour
{
    public List<EMPShield> empBombs = new List<EMPShield>();

    public void SetBomb(EMPShield bomb)
    {
        empBombs.Add(bomb);
    }

    public EMPShield FindNearestBomb(Vector3 pos)
    {
        if(empBombs.Count == 0)
            return null;

        int near = -1;
        float nearDist = -1f;
        //var nearDist = Vector3.Distance(empBombs[0].transform.position,pos);
        for(int i = 1; 0 < empBombs.Count;)
        {
            if(empBombs[i] == null || empBombs[i].isOver)
            {
                empBombs.RemoveAt(i);
                continue;
            }

            var dist = Vector3.Distance(empBombs[i].transform.position,pos);
            if(nearDist > dist || nearDist == -1f)
            {
                nearDist = dist;
                near = i;
            }

            ++i;
        }

        return near == -1 ? null : empBombs[near];
    }

    public EMPShield FindFarestBomb(Vector3 pos)
    {
        if(empBombs.Count == 0)
            return null;

        int far = -1;
        float farDist = -1f;
        //var nearDist = Vector3.Distance(empBombs[0].transform.position,pos);
        for(int i = 1; i < empBombs.Count;)
        {
            if(empBombs[i] == null || empBombs[i].isOver)
            {
                empBombs.RemoveAt(i);
                continue;
            }

            var dist = Vector3.Distance(empBombs[i].transform.position,pos);
            if(farDist < dist || farDist == -1f)
            {
                farDist = dist;
                far = i;
            }

            ++i;
        }

        return far == -1 ? null : empBombs[far];
    }
}
