using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanMakerPool : ObjectPoolBase<ScanMaker>
{
    public void Awake()
    {
        _activeDelegate += (t, position, rotation) =>
        {
            t.gameObject.SetActive(true);
            t.transform.SetParent(this.transform);
        };

        _deleteProgressDelegate += t =>
        {
            t.gameObject.SetActive(false);
        };

        _deleteCondition += t =>
        {
            return (t.Visible == false);
        };
    }
}
