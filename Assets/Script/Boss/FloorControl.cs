using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorControl : MonoBehaviour
{
    public List<Rotator> floors = new List<Rotator>();
    public Vector3 firstPosition;
    private List<Vector3> origins = new List<Vector3>();

    public bool _launch = false;

    public void Start()
    {
        for(int i = 0; i < floors.Count; ++i)
        {
            origins.Add(floors[i].transform.position);
            Debug.Log(floors[i].transform.position);
            floors[i].transform.position = firstPosition;
            Debug.Log(firstPosition);
        }
    }

    public void Update()
    {
        if (GameManager.Instance.PAUSE == true)
            return;

        if (_launch)
        {
            for(int i = 0; i < floors.Count; ++i)
            {
                floors[i].transform.position = Vector3.Slerp(floors[i].transform.position,origins[i],0.01f);
            }
        }
    }


    public void Launch()
    {
        _launch = true;
        foreach(var floor in floors)
        {
            floor.play = true;
        }
    }
}
