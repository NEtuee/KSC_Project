using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HexCubeGrid : MonoBehaviour
{
    public GameObject gridOrigin;
    public int mapSize;
    public float cubeSize;

    [SerializeField] private List<HexCube> _serializedCubeMap;
    private Dictionary<int,HexCube> _cubeMap;
    private float _cubeWidth;
    private float _cubeHeight;
    private List<Vector3Int> _cubeSaveList;

    public void Start()
    {
        _cubeSaveList = new List<Vector3Int>();
        CubeListToDictionary();
    }

    public void CubeListToDictionary()
    {
        _cubeMap = new Dictionary<int, HexCube>();
        foreach(var cube in _serializedCubeMap)
        {
            AddCube(cube);
        }
    }

    public void GetRangeHexs(ref List<HexCube> list,Vector3 position,int range)
    {
        var cube = GetCubeFromWorld(position);
        _cubeSaveList.Clear();
        HexGridHelperEx.GetCubeRange(ref _cubeSaveList,cube.cubePoint,range);

        foreach(var hex in _cubeSaveList)
        {
            var target = GetCube(hex);
            if(target == null)
                continue;

            list.Add(target);
        }
    }

    public void GetRingHexs(ref List<HexCube> list,Vector3 position,float radius)
    {
        var cube = GetCubeFromWorld(position);
        _cubeSaveList.Clear();
        HexGridHelperEx.GetCubeRing(ref _cubeSaveList,cube.cubePoint,radius);

        foreach(var hex in _cubeSaveList)
        {
            var target = GetCube(hex);
            if(target == null)
                continue;

            list.Add(target);
        }
    }

    public void GetNearHexs(ref List<HexCube> list,Vector3 position, int direction, int rotation = 1)
    {
        var cube = GetCubeFromWorld(position);
        _cubeSaveList.Clear();
        for(int i = direction; i < direction + rotation; ++i)
        {
            var cubePoint = HexGridHelperEx.GetCubeNeighbor(cube.cubePoint,i);
            var target = GetCube(cubePoint);
            if(target == null)
                continue;
                
            list.Add(target);
        }
        
    }

    public void CreateCubeMap()
    {
        if(_serializedCubeMap == null)
            _serializedCubeMap = new List<HexCube>();
        else
            DisposeGrid();

        mapSize = mapSize % 2 == 0 ? mapSize + 1 : mapSize;
        _cubeWidth = HexGridHelperEx.GetWidth(cubeSize);
        _cubeHeight = HexGridHelperEx.GetHeight(cubeSize);
        
        for(int i = 0; i < mapSize; ++i)
        {
            int max = ((mapSize - 1) / 2) - i;
            int j = Mathf.Max(0,max);
            int size = mapSize + Mathf.Min(0,max);
            for(; j < size; ++j)
            {
                AddCubeToList(j,i);
            }
        }
    }

    public void AddCubeToList(int q, int r)
    {
        var cube = CreateCube(q,r);
        _serializedCubeMap.Add(cube);
    }
    
    public void AddCube(int q,int r)
    {
        var cube = CreateCube(q,r);
        AddCube(cube);
    }

    public void AddCube(HexCube cube)
    {
        _cubeMap.Add(cube.key,cube);
    }

    public void DisposeGrid()
    {
        foreach(var cube in _serializedCubeMap)
        {
            DestroyImmediate(cube.gameObject);
        }

        if(_cubeMap != null)
            _cubeMap.Clear();
        _serializedCubeMap.Clear();
    }

    public HexCube GetCubeFromWorld(Vector3 world)
    {
        world = world - transform.position;
        var hex = HexGridHelperEx.WorldToAxial(cubeSize * .5f, world);
        return GetCube(hex);
        
    }

    public HexCube GetCube(Vector3Int cube)
    {
        var hex = HexGridHelperEx.CubeToAxial(cube);
        return GetCube(hex);
    }

    public HexCube GetCube(Vector2Int hex)
    {
        var key = HexGridHelperEx.GetKeyFromAxial(hex.x,hex.y,mapSize);

        if(_cubeMap.ContainsKey(key))
            return _cubeMap[key];
        else
        {
            Debug.Log("Out Of Range");
            return null;
        }
    }

    public HexCube CreateCube(int q,int r)
    {
        var cube = CreateCube();
        int half = (mapSize - 1) / 2;

        cube.Init(q - half,r - half,mapSize,cubeSize);
        cube.transform.position = HexGridHelperEx.AxialToWorld(_cubeWidth,_cubeHeight,cube.axialPoint);

        return cube;
    }

    public HexCube CreateCube()
    {
        var obj = Instantiate(gridOrigin);
        var cube = obj.GetComponent<HexCube>();

        obj.transform.SetParent(this.transform);

        return cube;
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
            changer.CreateCubeMap();
        }
        if (GUILayout.Button("Delete"))
        {
            changer.DisposeGrid();
        }
    }
}

#endif