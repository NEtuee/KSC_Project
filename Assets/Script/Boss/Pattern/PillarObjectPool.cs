using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PillarObjectPool : ObjectPoolBase<FallPillar>
{
    public void Awake()
    {
        _activeDelegate += (t, position, rotation) =>
        {
            t.transform.SetPositionAndRotation(position, rotation);
            t.gameObject.SetActive(true);
            t.Appear(4f);
        };

        _deleteProgressDelegate += t =>
        {
            t.gameObject.SetActive(false);
        };

        _deleteCondition += t =>
        {
            return (t.Done == true);
        };
    }
}
