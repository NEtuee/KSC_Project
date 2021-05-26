using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boogie_GridControll : MonoBehaviour
{
    public HexCubeGrid cubeGrid;
    public Material prev;
    public Material curr;


    public HexCube centerCube;
    public Transform _target;
    private List<HexCube> _targetCubes;

    private Vector3 _prevCheck;
    private Vector3 _prevTargetPosition;
    private Vector3 _coreOriginPosition;

    private HexCube _coreCube;

    private float _pathAngle = 0f;


    public void Init()
    {
        _targetCubes = new List<HexCube>();
        centerCube = cubeGrid.GetCube(Vector3Int.zero,false);
        _target = GameManager.Instance.player.transform;
    }

    public void Update()
    {
        Progress(Time.deltaTime);
    }

    public void Progress(float deltaTime)
    {
        //CoreCubeUpdate(deltaTime);
        if(_prevCheck != _target.position)
        {
            _prevTargetPosition = _prevCheck;
            _prevCheck = _target.position;
        }
        
        // foreach(var n in _targetCubes)
        // {
        //     if(n == null)
        //         continue;
        //     n.GetComponent<MeshRenderer>().material = prev;
        // }

        // foreach(var n in _targetCubes)
        // {
        //     if(n == null)
        //         continue;
        //     n.GetComponent<MeshRenderer>().material = curr;
        // }
    }

    public HexCube GetRandomActiveCube(bool ignoreSpecial) {return cubeGrid.GetRandomActiveCube(ignoreSpecial);}
    public List<HexCube> GetTargetCubes() {return _targetCubes;}
    public HexCube GetCoreCube() {return _coreCube;}

    public void SetCubesActive(bool active, bool timer = false,float time = 0f)
    {
        SetCubesActive(ref _targetCubes,active,timer,time);
    }

    public void SetCubesActive(ref List<HexCube> list, bool active,bool timer = false,float time = 0f)
    {
        foreach(var cube in list)
        {
            if(cube.special || cube.IsActive() == active)
                continue;

            cube.SetActive(active,timer,time);
            //cube.gameObject.SetActive(active);
        }
    }

    public void CoreCubeUpdate(float deltaTime)
    {
        if(_coreCube != null)
        {
            var targetPosition = _coreOriginPosition + (_coreCube.special ? new Vector3(0f,3f,0f) : Vector3.zero);
            _coreCube.transform.localPosition = Vector3.Lerp(_coreCube.transform.localPosition,targetPosition,deltaTime * 20f);
        }
    }

    public void GetCube_Near(Vector3Int target,int loopCount, bool ignoreSpecial)
    {
        _targetCubes.Clear();
        cubeGrid.GetCubeNear(ref _targetCubes,target,0,loopCount,ignoreSpecial);
    }

    public void GetCube_WeekPoint()
    {
        _targetCubes.Clear();
        var dir = -(MathEx.DeleteYPos(_target.position) - MathEx.DeleteYPos(centerCube.transform.position)).normalized;
        var point = cubeGrid.GetCubePointFromWorld(dir * (cubeGrid.mapSize * (cubeGrid.cubeSize * 0.5f)));
        cubeGrid.GetCubeLineHeavy(ref _targetCubes,centerCube.cubePoint,point,0,6);

        if(_coreCube != null)
        {
            _coreCube.special = false;
            _coreCube.SetTargetPosition(Vector3.zero);
        }

        if(_targetCubes.Count == 0)
        {
            _coreCube = null;
            return;
        }

        _coreCube = _targetCubes[Random.Range(0, _targetCubes.Count)];
        _coreCube.special = true;
        _coreCube.SetTargetPosition(new Vector3(0f,3f,0f));
    }

    public void GetCube_CurrentPoint()
    {
        _targetCubes.Clear();
        _targetCubes.Add(cubeGrid.GetCubeFromWorld(_target.position));
    }

    public void GetCube_Ring(int radius)
    {
        _targetCubes.Clear();
        cubeGrid.GetCubeRing(ref _targetCubes,centerCube.cubePoint,radius);
    }

    public void GetCube_Sector()
    {
        _targetCubes.Clear();

        var dir = (MathEx.DeleteYPos(_target.position) - MathEx.DeleteYPos(centerCube.transform.position)).normalized;
        var angle = Vector3.SignedAngle(dir,Vector3.right,Vector3.up);

        int sector = 4;
        int range = (cubeGrid.mapSize) / 2 + 1;

        if(angle <= 60f && angle >= -60f)
            sector = 1;
        else if(angle <= 120f && angle >= 60f)
            sector = 3;
        else if(angle <= -60f && angle >= -120f)
            sector = 0;

        if(sector == 0)
        {
            cubeGrid.GetCubeSectorCycle(ref _targetCubes,centerCube.cubePoint,sector,range);
        }
        else if(sector == 1)
        {
            cubeGrid.GetCubeSectorCycle(ref _targetCubes,centerCube.cubePoint,sector,range);
            cubeGrid.GetCubeSectorCycle(ref _targetCubes,centerCube.cubePoint,sector + 1,range);
        }
        else if(sector == 3)
        {
            cubeGrid.GetCubeSectorCycle(ref _targetCubes,centerCube.cubePoint,sector,range);
        }
        else if(sector == 4)
        {
            cubeGrid.GetCubeSectorCycle(ref _targetCubes,centerCube.cubePoint,sector,range);
            cubeGrid.GetCubeSectorCycle(ref _targetCubes,centerCube.cubePoint,sector + 1,range);
        }
    }

    public void GetCube_Walkway()
    {
        _targetCubes.Clear();

        var currDir = (MathEx.DeleteYPos(_target.position) - MathEx.DeleteYPos(centerCube.transform.position)).normalized;
        var prevDir = (MathEx.DeleteYPos(_prevTargetPosition) - MathEx.DeleteYPos(centerCube.transform.position)).normalized;

        var angle = Vector3.SignedAngle(currDir,prevDir,Vector3.up);
        if(angle != 0f)
        {
            _pathAngle = angle;
        }

        var rotateDir = Quaternion.Euler(0f,MathEx.normalize(_pathAngle) * -20f,0f) * currDir;

        var startPoint = cubeGrid.GetCubePointFromWorld(-rotateDir * (cubeGrid.mapSize * (cubeGrid.cubeSize * 0.5f)));
        var endPoint = cubeGrid.GetCubePointFromWorld(rotateDir * (cubeGrid.mapSize * (cubeGrid.cubeSize * 0.5f)));
        cubeGrid.GetCubeLineHeavy(ref _targetCubes,startPoint,endPoint,_pathAngle < 0 ? 3 : 0, 2);
    }
}
