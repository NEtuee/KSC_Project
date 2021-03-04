using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexColorRay : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray,out hit))
            {
                int[] poly = hit.collider.GetComponent<MeshFilter>().mesh.triangles;
                Color[] vertexcolor = hit.collider.GetComponent<MeshFilter>().mesh.colors;

                Debug.Log(vertexcolor[poly[hit.triangleIndex * 3 + 0]]);
                Debug.Log(vertexcolor[poly[hit.triangleIndex * 3 + 1]]);
                Debug.Log(vertexcolor[poly[hit.triangleIndex * 3 + 2]]);

            }

        }
    }
}
