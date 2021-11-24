using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdyBoss_PlatformCutV2 : MonoBehaviour
{
    public HexCubeGrid grid;
    public float cubeTerm = 2f;
    public float cubeSpeed = 1f;

    public int safeZone = 7;

    private Dictionary<int, List<HexCube>> _downCubes = new Dictionary<int, List<HexCube>>();
    private List<HexCube> _ring = new List<HexCube>();

    public void PatternStart(Transform player)
    {
        var cube = grid.GetCubePointFromWorld(Vector3.zero);

        int count = 0;

        for (int i = grid.mapSize; i > safeZone; --i)
        {
            _ring.Clear();
            grid.GetCubeRing(ref _ring, cube, i);

            if (!_downCubes.ContainsKey(count))
            {
                _downCubes.Add(count, new List<HexCube>());
            }
            else if (_downCubes[count] == null)
            {
                _downCubes[count] = new List<HexCube>();
            }

            _downCubes[count].Clear();

            for (int j = 0; j < _ring.Count; ++j)
            {
                _ring[j].SetMove(false, (float)count * cubeTerm, cubeSpeed);
                _ring[j].SetAlertTime(1f);
                _ring[j].special = true;
                _downCubes[count].Add(_ring[j]);
            }

            ++count;
        }
    }

    public void PatternEnd()
    {
        for (int i = 0; i < _downCubes.Keys.Count; ++i)
        {
            for (int j = 0; j < _downCubes[i].Count; ++j)
            {
                _downCubes[i][j].SetMove(true, (float)i * cubeTerm, cubeSpeed);
                _downCubes[i][j].special = false;
            }
        }
    }
}
