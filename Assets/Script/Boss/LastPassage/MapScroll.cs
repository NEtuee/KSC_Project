using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScroll : ObjectBase
{
    public enum ScrollDirection
    {
        UP,
        DOWN,
    }

    public ScrollDirection direction;
    public GameObject wallOrigin;

    public Material gridOne;
    public Material gridTwo;

    public float wallHeight;
    public float swapHeight;

    public float scrollSpeed = 10f;
    public int wallCount;


    private List<Transform> _walls = new List<Transform>();
    private Transform _bottom;


    public override void Assign()
    {
        base.Assign();
        CreateWalls();

    }

    public override void Initialize()
    {
        base.Initialize();
        
        RegisterRequest(GetSavedNumber("StageManager"));
    }

    public override void Progress(float deltaTime)
    {
        base.Progress(deltaTime);
        Scroll(deltaTime);
    }

    public void CreateWalls()
    {
        for(int i = 0; i < wallCount; ++i)
        {
            var wall = Instantiate(wallOrigin,Vector3.zero,Quaternion.identity).transform;
            wall.SetParent(transform);
            wall.GetComponent<MeshRenderer>().material = (i % 2) == 0 ? gridOne : gridTwo;
            wall.localPosition = new Vector3(0f,(float)i * wallHeight,0f);
            _walls.Add(wall);
        }

        _bottom = _walls[0];
    }

    public void Scroll(float deltaTime)
    {
        var speed = new Vector3(0f,scrollSpeed,0f) * deltaTime;
        foreach(var wall in _walls)
        {
            if(direction == ScrollDirection.UP)
            {
                wall.transform.localPosition += speed;
            }
            else if(direction == ScrollDirection.DOWN)
            {
                wall.transform.localPosition -= speed;
            }

        }

        foreach(var wall in _walls)
        {
            if(direction == ScrollDirection.UP)
            {
                if(wall.transform.localPosition.y >= swapHeight)
                {
                    wall.transform.localPosition = _bottom.transform.localPosition - new Vector3(0f,wallHeight,0f);
                    _bottom = wall;
                }
            }
            else if(direction == ScrollDirection.DOWN)
            {
                if(wall.transform.localPosition.y <= swapHeight)
                {
                    wall.transform.localPosition = _bottom.transform.localPosition + new Vector3(0f,wallHeight,0f);
                    _bottom = wall;
                }
            }

        }


    }
}
