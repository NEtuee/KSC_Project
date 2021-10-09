using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridPickTest : MonoBehaviour
{
    public HexCubeGrid grid;
    public Material prev;
    public Material curr;
    public Camera cam;

    private HexCube _cube;

    void Update()
    {
        if(_cube != null)
        {
            _cube.GetRenderer().material = prev;
        }

        _cube = grid.GetCubeFromWorld(transform.position);

        if(_cube != null)
        {
            _cube.GetRenderer().material = curr;
        }

        if(Mouse.current == null)
            Debug.Log("Check");

        
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        var pos = MathEx.PlaneLineIntersect(grid.transform.position,Vector3.up,ray.origin,ray.direction);

        transform.position = pos;
    }
}
