using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class HexCubeGrid : MonoBehaviour
{
    public AnimationCurve inCurve;
    public AnimationCurve outCurve;

    public GameObject gridOrigin;
    public int mapSize;
    public float cubeSize;

    public bool moveCubeToUp = true;

    [SerializeField] private List<HexCube> _serializedCubeMap;
    private Dictionary<int,HexCube> _cubeMap;
    [SerializeField] private float _cubeWidth;
    [SerializeField] private float _cubeHeight;
    private List<Vector3Int> _cubeSaveList;
    private List<HexCube> _hexCubeSaveList;
    private Dictionary<int,Vector3Int> _overlapCheckList;

    public void Awake()
    {
        _cubeSaveList = new List<Vector3Int>();
        _hexCubeSaveList = new List<HexCube>();
        _overlapCheckList = new Dictionary<int, Vector3Int>();
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

    public void MoveUpALL()
    {
        foreach(var item in _cubeMap.Values)
        {
            item.MoveToUp();
        }
    }

    public void GetCubeLineHeavy(ref List<HexCube> list,Vector3Int start ,Vector3Int end, int loopStart, int loopCount, bool overlapListClear = true)
    {
        _hexCubeSaveList.Clear();
        GetCubeLine(ref _hexCubeSaveList,start,end);

        if(overlapListClear)
            _overlapCheckList.Clear();

        foreach(var item in _hexCubeSaveList)
        {
            _cubeSaveList.Clear();
            HexGridHelperEx.GetCubeNear(ref _cubeSaveList,item.cubePoint,loopStart,loopCount);

            foreach(var cube in _cubeSaveList)
            {
                var key = HexGridHelperEx.GetKeyFromCube(cube,mapSize);

                if(!_overlapCheckList.ContainsKey(key))
                {
                    _overlapCheckList.Add(key,cube);
                }
            }
            
        }

        foreach(var item in _overlapCheckList)
        {
            var target = GetCube(item.Value);
            if(target == null)
                continue;

            list.Add(target);
        }
    }

    public void GetCubeLine(ref List<HexCube> list,Vector3 start ,Vector3 end)
    {
        var startInt = HexGridHelperEx.WorldToCube(cubeSize * .5f, start);
        var endInt = HexGridHelperEx.WorldToCube(cubeSize * .5f, end);

        GetCubeLine(ref list, startInt, endInt);
    }

    public void GetCubeLine(ref List<HexCube> list,Vector3Int start ,Vector3Int end)
    {
        _cubeSaveList.Clear();
        HexGridHelperEx.GetCubeLine(ref _cubeSaveList,start,end);

        foreach(var cube in _cubeSaveList)
        {
            var target = GetCube(cube);
            if(target == null)
                continue;

            list.Add(target);
        }
    }

    public void GetCubeRange(ref List<HexCube> list,Vector3 position,int range, bool ignoreSpecial)
    {
        var cubeObj = GetCubeFromWorld(position,ignoreSpecial);
        _cubeSaveList.Clear();
        HexGridHelperEx.GetCubeRange(ref _cubeSaveList,cubeObj.cubePoint,range);

        foreach(var cube in _cubeSaveList)
        {
            var target = GetCube(cube,ignoreSpecial);
            if(target == null)
                continue;

            list.Add(target);
        }
    }

    public void GetCubeSectorCycle(ref List<HexCube> list,Vector3Int point,int sector, int radius)
    {
        var cubeObj = GetCube(point,false);
        _cubeSaveList.Clear();
        HexGridHelperEx.GetCubeSectorCycle(ref _cubeSaveList,cubeObj.cubePoint,sector,radius);

        foreach(var cube in _cubeSaveList)
        {
            var target = GetCube(cube);
            if(target == null)
                continue;

            list.Add(target);
        }
    }

    public void GetCubeSectorCycle(ref List<HexCube> list,Vector3 position,int sector, int radius)
    {
        var cubeObj = GetCubeFromWorld(position,false);
        _cubeSaveList.Clear();
        HexGridHelperEx.GetCubeSectorCycle(ref _cubeSaveList,cubeObj.cubePoint,sector,radius);

        foreach(var cube in _cubeSaveList)
        {
            var target = GetCube(cube);
            if(target == null)
                continue;

            list.Add(target);
        }
    }

    public void GetCubeSectorRing(ref List<HexCube> list,Vector3 position,int sector, int radius)
    {
        var cubeObj = GetCubeFromWorld(position,false);
        _cubeSaveList.Clear();
        HexGridHelperEx.GetCubeSectorRing(ref _cubeSaveList,cubeObj.cubePoint,sector,radius);

        foreach(var cube in _cubeSaveList)
        {
            var target = GetCube(cube);
            if(target == null)
                continue;

            list.Add(target);
        }
    }

    public void GetCubeRing(ref List<HexCube> list,Vector3Int point,int radius)
    {
        var cubeObj = GetCube(point,false);
        _cubeSaveList.Clear();
        HexGridHelperEx.GetCubeRing(ref _cubeSaveList,cubeObj.cubePoint,radius);

        foreach(var cube in _cubeSaveList)
        {
            var target = GetCube(cube);
            if(target == null)
                continue;

            list.Add(target);
        }
    }

    public void GetCubeRing(ref List<HexCube> list,Vector3 position,int radius)
    {
        var cubeObj = GetCubeFromWorld(position,false);
        _cubeSaveList.Clear();
        HexGridHelperEx.GetCubeRing(ref _cubeSaveList,cubeObj.cubePoint,radius);

        foreach(var cube in _cubeSaveList)
        {
            var target = GetCube(cube);
            if(target == null)
                continue;

            list.Add(target);
        }
    }

    public void GetCubeNear(ref List<HexCube> list,Vector3Int point, int direction, int rotation = 1, bool ignoreSpecial = true)
    {
        _cubeSaveList.Clear();
        HexGridHelperEx.GetCubeNear(ref _cubeSaveList,point,direction,rotation);
        foreach(var cube in _cubeSaveList)
        {
            var target = GetCube(cube,ignoreSpecial);
            if(target == null)
                continue;

            list.Add(target);
        }
        
    }

    public void GetAllCube(ref List<HexCube> list,bool ignoreSpecial)
    {
        _cubeSaveList.Clear();
        foreach(var cube in _cubeMap.Values)
        {
            if((ignoreSpecial ? !cube.special : true))
            {
                list.Add(cube);
            }
        }

    }

    public HexCube GetRandomActiveCube(bool ignoreSpecial)
    {
        _cubeSaveList.Clear();
        foreach(var cube in _cubeMap.Values)
        {
            if(cube.IsActive() && (ignoreSpecial ? !cube.special : true) || 
                (cube.GetMoveStartTime() > 0f && (ignoreSpecial ? !cube.special : true)))
            {
                _cubeSaveList.Add(cube.cubePoint);
            }
        }

        return GetCube(_cubeSaveList[Random.Range(0,_cubeSaveList.Count)],false);
    }

    public HexCube GetCubeReflectMirror(Vector3Int point)
    {
        return GetCube(HexGridHelperEx.GetCubeReflectMirror(point),false);
    }

    public HexCube GetCubeReflectX(Vector3Int point)
    {
        return GetCube(HexGridHelperEx.GetCubeReflectX(point),false);
    }

    public HexCube GetCubeReflectY(Vector3Int point)
    {
        return GetCube(HexGridHelperEx.GetCubeReflectY(point),false);
    }

    public HexCube GetCubeReflectZ(Vector3Int point)
    {
        return GetCube(HexGridHelperEx.GetCubeReflectZ(point),false);
    }

    public void CreateCubeMap(bool moveToUp = true)
    {
        if(_serializedCubeMap == null)
            _serializedCubeMap = new List<HexCube>();
        else
            DisposeGrid();

        SetMapSize();
        
        for(int i = 0; i < mapSize; ++i)
        {
            int max = ((mapSize - 1) / 2) - i;
            int j = Mathf.Max(0,max);
            int size = mapSize + Mathf.Min(0,max);
            for(; j < size; ++j)
            {
                AddCubeToList(j,i,moveToUp);
            }
        }
    }

    public void MoveToDownAll()
    {
        foreach(var item in _cubeMap.Values)
        {
            item.MoveToDown();
        }
    }

    public void SetMapSize()
    {
        mapSize = mapSize % 2 == 0 ? mapSize + 1 : mapSize;
        _cubeWidth = HexGridHelperEx.GetWidth(cubeSize);
        _cubeHeight = HexGridHelperEx.GetHeight(cubeSize);
    }

    public void AddCubeToList(HexCube cube)
    {
        _serializedCubeMap.Add(cube);
    }

    public void AddCubeToList(int q, int r, bool moveToUp = true)
    {
        var cube = CreateCube(q,r,moveToUp);
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

    public Vector3Int GetCubePointFromWorld(Vector3 world)
    {
        world = world - transform.position;
        var cube = HexGridHelperEx.WorldToCube(cubeSize * .5f, world);
        return cube;
    }

    public Vector3 CubePointToWorld(Vector3Int point)
    {
        return HexGridHelperEx.CubeToWorld(_cubeWidth,_cubeHeight,point) + transform.position;
    }

    public Vector2Int CubePointToAxial(Vector3Int point)
    {
        return HexGridHelperEx.CubeToAxial(point);
    }

    public Vector2Int WorldToAxial(Vector3 world)
    {
        return HexGridHelperEx.WorldToAxial(cubeSize * .5f, world);
    }

    public HexCube GetCubeFromWorld(Vector3 world, bool ignoreSpecial = true)
    {
        world = world - transform.position;
        var hex = HexGridHelperEx.WorldToAxial(cubeSize * .5f, world);
        return GetCube(hex,ignoreSpecial);
        
    }

    public HexCube GetCubeFromList(Vector2Int hex, bool ignoreSpecial = true)
    {
        var key = HexGridHelperEx.GetKeyFromAxial(hex.x,hex.y,mapSize);
        return _serializedCubeMap.Find((x)=>{return x.key == key;});
    }

    public HexCube GetCube(Vector3Int cube, bool ignoreSpecial = true)
    {
        var hex = HexGridHelperEx.CubeToAxial(cube);
        return GetCube(hex,ignoreSpecial);
    }

    public HexCube GetCube(Vector2Int hex, bool ignoreSpecial = true)
    {
        if(MathEx.abs(hex.x) >= mapSize / 2f || MathEx.abs(hex.y) >= mapSize /2f)
            return null;
        
        var key = HexGridHelperEx.GetKeyFromAxial(hex.x,hex.y,mapSize);

        if(_cubeMap.ContainsKey(key))
        {
            var cube = _cubeMap[key];
            if(cube.special && ignoreSpecial)
                return null;
            else
                return cube;
        }
        else
        {
            //Debug.Log("Out Of Range");
            return null;
        }
    }

    public HexCube GetCube(int key, bool ignoreSpecial = true)
    {

        if(_cubeMap.ContainsKey(key))
        {
            var cube = _cubeMap[key];
            if(cube.special && ignoreSpecial)
                return null;
            else
                return cube;
        }
        else
        {
            //Debug.Log("Out Of Range");
            return null;
        }
    }

    public HexCube CreateCube(int q,int r, bool moveToUp = true)
    {
        var cube = CreateCube();
        int half = (mapSize - 1) / 2;

        cube.Init(q - half,r - half,mapSize,cubeSize);
        cube.transform.position = HexGridHelperEx.AxialToWorld(_cubeWidth,_cubeHeight,cube.axialPoint) + transform.position;

        if(moveToUp)
            cube.MoveToUp();
        else
            cube.MoveToDown();

        return cube;
    }

    public HexCube CreateCube()
    {
        var obj = Instantiate(gridOrigin);
        var cube = obj.GetComponent<HexCube>();

        obj.transform.SetParent(this.transform);

        cube.inCurve = inCurve;
        cube.outCurve = outCurve;

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
        if (GUILayout.Button("SetMapSize"))
        {
            changer.SetMapSize();
        }
        if (GUILayout.Button("Create"))
        {
            changer.CreateCubeMap(changer.moveCubeToUp);
        }
        if (GUILayout.Button("Delete"))
        {
            changer.DisposeGrid();
        }
    }
}

#endif