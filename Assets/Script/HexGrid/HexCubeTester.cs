using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCubeTester : MonoBehaviour
{
    public Material prev;
    public Material curr;

    public HexCubeGrid grid;

    public int radius = 0;
    public int sector = 0;

    private HexCube currentCube;
    private List<HexCube> _nearHex;

    public void Start()
    {
        _nearHex = new List<HexCube>();
    }

    void Update()
    {
        Debug.Log(Vector3.Distance(Vector3.zero,transform.position));
        //var cube = grid.GetCubeFromWorld(transform.position);
        var cubePoint = HexGridHelperEx.WorldToCube(grid.cubeSize * .5f,transform.position);
        //if(cube != null && currentCube != cube)
        {
            if(currentCube != null)
            {
                currentCube.GetComponent<MeshRenderer>().material = prev;
            }

            foreach(var n in _nearHex)
            {
                n.GetComponent<MeshRenderer>().material = prev;
            }
            _nearHex.Clear();

            var reflect = grid.GetCubeReflectMirror((cubePoint));
            //grid.GetCubeLineHeavy(ref _nearHex,Vector3Int.zero,cubePoint,6);
            grid.GetCubeNear(ref _nearHex,cubePoint,0,6);

            // if(reflect != null)
            //     _nearHex.Add(reflect);

                
            foreach(var n in _nearHex)
            {
                n.GetComponent<MeshRenderer>().material = curr;
            }

            //cube.GetComponent<MeshRenderer>().material = curr;
            //currentCube = cube;
        }
    }
}
