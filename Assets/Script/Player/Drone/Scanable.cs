using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Scanable : MonoBehaviour
{
    public bool visible;
    public abstract void Scanned();

    private void OnBecameVisible()
    {
        visible = true;
    }

    private void OnBecameInvisible()
    {
        visible = false;
    }
}
