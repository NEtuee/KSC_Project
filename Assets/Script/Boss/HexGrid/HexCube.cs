using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCube : MonoBehaviour
{
    public Vector3Int cubePoint;
    public Vector2Int axialPoint;
    public float cubeSize;
    public int key;

    public void Init(int q, int r, int mapSize, float size)
    {
        SetPoint(q,r);
        SetKeyFromAxial(mapSize);
        cubeSize = size;
    }
    
    public void SetPoint(int q, int r)
    {
        axialPoint = new Vector2Int(q,r);
        cubePoint = HexGridHelperEx.AxialToCube(axialPoint);
    }

    public void SetKeyFromAxial(int mapSize)
    {
        key = HexGridHelperEx.GetKeyFromAxial(axialPoint.x,axialPoint.y,mapSize);
        Debug.Log(key);
    }
}
