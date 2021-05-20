//https://www.redblobgames.com/grids/hexagons/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexGridHelperEx
{
    private static readonly Vector3Int[] _cubeDirection = {
        new Vector3Int(+1, -1, 0), new Vector3Int(+1, 0, -1), new Vector3Int(0, +1, -1), 
        new Vector3Int(-1, +1, 0), new Vector3Int(-1, 0, +1), new Vector3Int(0, -1, +1), 
    };

    public static int GetKeyFromAxial(int q,int r, int size)
    {
        return q * size + r;
    }

    public static Vector3Int AxialToCube(Vector2Int hex)
    {
        return new Vector3Int(hex.x,-hex.x - hex.y,hex.y);
    }

    public static Vector2Int CubeToAxial(Vector3Int cube)
    {
        return new Vector2Int(cube.x,cube.z);
    }

    public static Vector3Int GetCubeDirection(int direction)
    {
        return _cubeDirection[direction];
    }

    public static Vector3Int GetCubeNeighbor(Vector3Int cube, int direction)
    {
        return cube + GetCubeDirection(direction);
    }

    public static Vector3 GetFlatHexCorner(Vector3 center, float size, int target)
    {
        var deg = 60f * (float)target;
        var rad = Mathf.PI / 180f * deg;
        return new Vector3(center.x + size * Mathf.Cos(rad),center.y,center.z + size * Mathf.Sin(rad));
    }

    public static void GetCubeRing(ref List<Vector3Int> result, Vector3Int center,float radius)
    {
        var radInt = (int)radius;
        var cube = center + (GetCubeDirection(4) * radInt);
        for(int i = 0; i < 6; ++i)
        {
            for(int j = 0; j < radInt; ++j)
            {
                result.Add(cube);
                cube = GetCubeNeighbor(cube,i);
            }
        }
    }

    public static int GetManhattanDistance(Vector3Int a, Vector3Int b)
    {
        return (MathEx.abs(a.x - b.y) + MathEx.abs(a.y - b.y) + MathEx.abs(a.z - b.z)) / 2;
    }


    public static float GetWidth(float size){return 2f * size;}
    public static float GetHeight(float size){return Mathf.Sqrt(3f) * size;}
    public static float GetHorizontalDistance(float width) {return width * (3f / 4f);}
    public static float GetVerticalDistance(float height) {return height;}
}
