using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HexCubeGrid : MonoBehaviour
{
    public int gridMapSize;

    private Dictionary<int,int> _cubeMap;
    private int _mapSize;

    public void CreateCubeMap(int mapSize)
    {
        if(_cubeMap == null)
            _cubeMap = new Dictionary<int, int>();
        else
            DisposeGrid();

        _mapSize = mapSize;
        _mapSize = _mapSize % 2 == 0 ? _mapSize + 1 : _mapSize;

        
        for(int i = 0; i < _mapSize; ++i)
        {
            int max = ((_mapSize - 1) / 2) - i;
            int j = Mathf.Max(0,max);
            int size = _mapSize + Mathf.Min(0,max);
            for(; j < size; ++j)
            {
                AddCube(j,i);
            }
        }
    }

    public void AddCube(int q,int r)
    {
        Debug.Log(q + "," + r);
        _cubeMap.Add(HexGridHelperEx.GetKeyFromAxial(q,r,_mapSize),1);
    }

    public void DisposeGrid()
    {
        _cubeMap.Clear();
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(HexCubeGrid)),CanEditMultipleObjects]
public class HexCubeGridEdit : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        HexCubeGrid changer = (HexCubeGrid)target;
        if (GUILayout.Button("Create"))
        {
            changer.CreateCubeMap(changer.gridMapSize);
        }
    }
}

#endif