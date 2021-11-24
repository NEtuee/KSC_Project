using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChanger : MonoBehaviour
{
    public Renderer targetRenderer;
    public Material A;
    public Material B;

    private Material _origin;

    public void Awake()
    {
        _origin = targetRenderer.material;
    }

    public void SwitchA() { targetRenderer.material = A; }
    public void SwitchB() { targetRenderer.material = B; }
    public void SwitchOrigin() { targetRenderer.material = _origin; }
}
