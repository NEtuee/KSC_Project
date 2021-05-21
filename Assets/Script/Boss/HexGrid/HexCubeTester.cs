using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexCubeTester : MonoBehaviour
{
    public Material prev;
    public Material curr;

    public HexCubeGrid grid;

    private HexCube currentCube;
    private List<HexCube> _nearHex;

    public void Start()
    {
        _nearHex = new List<HexCube>();
    }

    void Update()
    {
        var cube = grid.GetCubeFromWorld(transform.position);
        if(cube != null && currentCube != cube)
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

            grid.GetRangeHexs(ref _nearHex,transform.position,3);
            foreach(var n in _nearHex)
            {
                n.GetComponent<MeshRenderer>().material = curr;
            }

            cube.GetComponent<MeshRenderer>().material = curr;
            currentCube = cube;
        }
    }
}
