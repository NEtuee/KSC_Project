//https://www.redblobgames.com/grids/hexagons/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HexGridHelperEx
{
    private static readonly Vector3Int[] _cubeDirection = 
    {
        new Vector3Int(+1, -1, 0), new Vector3Int(+1, 0, -1), new Vector3Int(0, +1, -1), 
        new Vector3Int(-1, +1, 0), new Vector3Int(-1, 0, +1), new Vector3Int(0, -1, +1), 
    };

    public static int GetKeyFromCube(Vector3Int cube, int size)
    {
        //return q >= r ? q * q + q + r : q + r * r;
        return GetKeyFromAxial(cube.x,cube.z,size);
    }

    public static int GetKeyFromAxial(int q,int r, int size)
    {
        //return q >= r ? q * q + q + r : q + r * r;
        return q * size + r;
    }

    public static Vector3Int WorldToCube(float size, Vector3 world)
    {
        var hex = WorldToAxial(size,world);
        return AxialToCube(hex);
    }
    

    public static Vector2Int WorldToAxial(float size, Vector3 world)
    {
        var q = (Mathf.Sqrt(3f) / 3f * world.x - 1f / 3f * -world.z) / size;
        var r = (2f / 3f * -world.z) / size;

        return HexRound(new Vector2(q,r));
    }
    
    public static Vector3 AxialToWorld(float cubeWidth, float cubeHeight, Vector2Int axial)
    {
        float floatY = (float)axial.y;
        float z = -floatY * (cubeHeight * .75f);
        float x = floatY * (cubeWidth * .25f) + ((float)axial.x) * (cubeWidth * .5f);

        return new Vector3(x, 0f, z);
    }

    public static Vector3 CubeToWorld(float cubeWidth, float cubeHeight, Vector3Int cube)
    {
        float cubeZ = ((float) cube.z);
        float z = -cubeZ * (cubeHeight * .75f);
        float x = cubeZ * (cubeWidth * .25f) + ((float)cube.x) * (cubeWidth * .5f);

        return new Vector3(x, 0f, z);
    }

    public static Vector3 AxialToCube(Vector2 hex)
    {
        var x = hex.x;
        var z = hex.y;
        var y = -x-z;
        return new Vector3(x,y,z);
    }

    public static Vector3Int AxialToCube(Vector2Int hex)
    {
        var x = hex.x;
        var z = hex.y;
        var y = -x-z;
        return new Vector3Int(x,y,z);
    }

    public static Vector3Int CubeRotateLeft(Vector3Int cube,int times = 1)
    {
        for(int i = 0; i < times; ++i)
        {
            cube = new Vector3Int(-cube.y,-cube.z,-cube.x);
        }
        return cube;
    }

    public static Vector3Int CubeRotateRight(Vector3Int cube,int times = 1)
    {
        for(int i = 0; i < times; ++i)
        {
            cube = new Vector3Int(-cube.z,-cube.x,-cube.y);
        }
        return cube;
    }

    public static Vector2 CubeToAxial(Vector3 cube){return new Vector2(cube.x,cube.z);}
    public static Vector2Int CubeToAxial(Vector3Int cube){return new Vector2Int(cube.x,cube.z);}
    public static Vector3Int GetCubeDirection(int direction){return _cubeDirection[direction];}
    public static Vector3Int GetCubeNeighbor(Vector3Int cube, int direction){return cube + GetCubeDirection(direction);}

   

    public static Vector3 GetFlatHexCorner(Vector3 center, float size, int target)
    {
        var deg = 60f * (float)target;
        var rad = Mathf.PI / 180f * deg;
        return new Vector3(center.x + size * Mathf.Cos(rad),center.y,center.z + size * Mathf.Sin(rad));
    }

    public static Vector3 CubeLerp(Vector3 a, Vector3 b, float t)
    {
        return Vector3.Lerp(a,b,t);
    }

    public static void GetCubeNear(ref List<Vector3Int> result,Vector3Int point, int direction, int rotation = 1)
    {
        for(int i = direction; i < direction + rotation; ++i)
        {
            var cube = GetCubeNeighbor(point,i);
            result.Add(cube);
        }
    }

    public static void GetCubeLine(ref List<Vector3Int> result, Vector3Int a, Vector3Int b)
    {
        var dist = GetManhattanDistance(a,b);
        for(int i = 0; i <= dist; ++i)
        {
            result.Add(CubeRound(CubeLerp((Vector3)a, (Vector3)b,1f / dist * i)));
        }
        
    }

    public static void GetCubeRange(ref List<Vector3Int> result, Vector3Int center,int range)
    {
        for(int x = -range; x <= range; ++x)
        {
            for(int y = Mathf.Max(-range, -x - range); y <= Mathf.Min(range, -x + range); ++y)
            {
                var z = -x - y;
                result.Add(center + new Vector3Int(x,y,z));
            }
        }

    }

    public static void GetCubeSectorCycle(ref List<Vector3Int> result, Vector3Int center,int sector,int radius)
    {
        for(int i = 0; i < radius; ++i)
        {
            GetCubeSectorRing(ref result,center,i);
        }

        for(int i = 0; i < result.Count; ++i)
        {
            result[i] = CubeRotateLeft(result[i],sector);
        }
    }

    public static void GetCubeSectorRing(ref List<Vector3Int> result, Vector3Int center,int radius)
    {
        var cube = center + (GetCubeDirection(4) * radius);
        for(int i = 0; i <= radius; ++i)
        {
            result.Add(cube);
            cube = GetCubeNeighbor(cube,0);
        }
    }

    public static void GetCubeSectorRing(ref List<Vector3Int> result, Vector3Int center,int sector,int radius)
    {
        var cube = center + (GetCubeDirection(4) * radius);
        for(int i = 0; i <= radius; ++i)
        {
            result.Add(cube);
            cube = GetCubeNeighbor(cube,0);
        }

        for(int i = 0; i < result.Count; ++i)
        {
            result[i] = CubeRotateRight(result[i],sector);
        }
    }

    public static void GetCubeRing(ref List<Vector3Int> result, Vector3Int center,int radius)
    {
        var cube = center + (GetCubeDirection(4) * radius);
        for(int i = 0; i < 6; ++i)
        {
            for(int j = 0; j < radius; ++j)
            {
                result.Add(cube);
                cube = GetCubeNeighbor(cube,i);
            }
        }
    }

    public static Vector2Int HexRound(Vector2 hex){return CubeToAxial(CubeRound(AxialToCube(hex)));}

    public static Vector3Int CubeRound(Vector3 cube)
    {
        var rx = Mathf.Round(cube.x);
        var ry = Mathf.Round(cube.y);
        var rz = Mathf.Round(cube.z);

        var xDiff = MathEx.abs(rx - cube.x);
        var yDiff = MathEx.abs(ry - cube.y);
        var zDiff = MathEx.abs(rz - cube.z);

        if(xDiff > yDiff && xDiff > zDiff)
            rx = -ry-rz;
        else if(yDiff > zDiff)
            ry = -rx-rz;
        else
            rz = -rx-ry;

        return new Vector3Int((int)rx,(int)ry,(int)rz);
    }

    public static Vector3Int GetCubeReflectMirror(Vector3Int cube) {return -cube;}
    public static Vector3Int GetCubeReflectX(Vector3Int cube) {return new Vector3Int(cube.x,cube.z,cube.y);}
    public static Vector3Int GetCubeReflectY(Vector3Int cube) {return new Vector3Int(cube.z,cube.y,cube.x);}
    public static Vector3Int GetCubeReflectZ(Vector3Int cube) {return new Vector3Int(cube.y,cube.x,cube.z);}

    public static int GetManhattanDistance(Vector3Int a, Vector3Int b)
    {
        return (MathEx.abs(a.x - b.x) + MathEx.abs(a.y - b.y) + MathEx.abs(a.z - b.z)) / 2;
    }


    public static float GetWidth(float size){return Mathf.Sqrt(3f) * size;}
    public static float GetHeight(float size){return size;}
    public static float GetHorizontalDistance(float width) {return width * (3f / 4f);}
    public static float GetVerticalDistance(float height) {return height;}
}
