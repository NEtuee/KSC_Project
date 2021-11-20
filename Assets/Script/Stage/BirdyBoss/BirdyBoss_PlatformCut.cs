using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdyBoss_PlatformCut : MonoBehaviour
{
    public HexCubeGrid grid;
    public float cubeTerm = .3f;
    public float cubeSpeed = 1f;

    public int safeZone = 2;
    public int cubeDistance = 2;

    private Dictionary<int,List<HexCube>> _downCubes = new Dictionary<int, List<HexCube>>();
    private List<HexCube> _ring = new List<HexCube>();

    public void PatternStart(Transform player)
    {
        var cube = grid.GetCubePointFromWorld(player.position);
        //_downCubes.Clear();

        int count = 0;

        for(int i = safeZone; i < grid.mapSize; i += cubeDistance)
        {
            _ring.Clear();
            grid.GetCubeRing(ref _ring,cube,i);

            if(!_downCubes.ContainsKey(count))
            {
                _downCubes.Add(count, new List<HexCube>());
            }
            else if(_downCubes[count] == null)
            {
                _downCubes[count] = new List<HexCube>();
            }

            _downCubes[count].Clear();

            for (int j = 0; j < _ring.Count; ++j)
            {
                _ring[j].SetMove(false, (float)count * cubeTerm, cubeSpeed);
                _ring[j].SetAlertTime(1f);
                _downCubes[count].Add(_ring[j]);
            }

            ++count;
        }
    }

    public void PatternEnd()
    {
        for(int i = 0; i < _downCubes.Keys.Count; ++i)
        {
            for(int j = 0; j < _downCubes[i].Count; ++j)
            {
                _downCubes[i][j].SetMove(true, (float)i * cubeTerm, cubeSpeed);
            }
        }
    }
}
