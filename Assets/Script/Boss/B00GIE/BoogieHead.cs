using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BoogieHead : MonoBehaviour
{
    public UnityEvent whenBombHit;

    public void Hit()
    {
        whenBombHit.Invoke();
    }
}
