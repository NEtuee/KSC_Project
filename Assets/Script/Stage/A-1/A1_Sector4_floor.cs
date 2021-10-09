using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class A1_Sector4_floor : MonoBehaviour
{
    public List<HexCube> groundGrid;
    public List<HexCube> entranceGrid;

    public float entranceMoveSpeed = 1f;

    public float groundMoveSpeed = 0.25f;

    public void EntranceDownMove()
    {
        foreach(var item in entranceGrid)
        {
            item.SetMove(false,0f,entranceMoveSpeed);
        }
    }

    public void GroundDownMove()
    {
        foreach(var item in groundGrid)
        {
            item.SetMove(false,0f,groundMoveSpeed);
        }
    }

}
