using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCube : MonoBehaviour
{
    public Vector3Int cubePoint;
    public Vector2Int axialPoint;
    public float cubeSize;

    public void Init(int q, int r, float size)
    {
        SetPoint(q,r);
        cubeSize = size;
    }
    
    public void SetPoint(int q, int r)
    {
        axialPoint = new Vector2Int(q,r);
        cubePoint = HexGridHelperEx.AxialToCube(axialPoint);
    }
}
