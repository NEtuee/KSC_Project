using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerTest : MonoBehaviour
{
    public LayerMask layer;

    void Start()
    {
        Debug.Log(layer.value);
        Debug.Log(this.gameObject.layer);

        Debug.Log(layer == (layer | (1 << this.gameObject.layer)));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
