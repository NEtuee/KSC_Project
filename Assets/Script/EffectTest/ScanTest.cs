using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScanTest : MonoBehaviour
{
    public Transform ScannerOrigin;
	public Material EffectMaterial;
    public Camera mainCamera;
	public float ScanDistance;

    bool _scanning;

    private Camera _camera;

    // Update is called once per frame
    void Start()
    {
        _camera = mainCamera;
        _camera.depthTextureMode = DepthTextureMode.Depth;
    }
    void Update()
    {
        if (_scanning)
		{
			ScanDistance += Time.deltaTime * 50f;
		}

		if (Input.GetKeyDown(KeyCode.C))
		{
			_scanning = true;
			ScanDistance = 0;
		}

		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit))
			{
				_scanning = true;
				ScanDistance = 0;
				ScannerOrigin.position = hit.point;

                EffectMaterial.SetVector("_WorldSpaceScannerPos", ScannerOrigin.position);
			}
		}

        EffectMaterial.SetFloat("_Distance", ScanDistance);
    }
}
