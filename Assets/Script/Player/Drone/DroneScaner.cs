using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneScaner : MonoBehaviour
{
    public Transform forward;
    public Transform scanStart;
    public Material scanMat;
    public float arc = 30f;
    public float scanSpeed = 80f;

    private Vector3 scanStartPosition;

    [SerializeField] private bool scaning = false;
    [SerializeField] private float maxRange = 1000.0f;
    [SerializeField] private float _range = 0f;
    private Vector3 scanForward;

    [SerializeField] private List<string> scanableTags = new List<string>();
    [SerializeField] private List<Scanable> scanableObjects = new List<Scanable>();

    private RaycastHit hit;
    private void Start()
    {
        foreach(string tag in scanableTags)
        {
            GameObject[] scanableObj= GameObject.FindGameObjectsWithTag(tag);
            Scanable currentScanable;
            for(int i = 0; i < scanableObj.Length; i++)
            {
                if(scanableObj[i].TryGetComponent<Scanable>(out currentScanable))
                {
                    scanableObjects.Add(currentScanable);
                }
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            scaning = true;
            _range = 0f;
            scanMat.SetFloat("_ScanArc", arc);
            scanStartPosition = scanStart.position;
            scanMat.SetVector("_WorldSpaceScannerPos", scanStart.position);
            scanForward = forward.forward;
            scanForward.y = 0.0f;
            scanForward.Normalize();
            scanMat.SetVector("_ForwardDirection", scanForward);
        }

        if (scaning == true)
        {
            _range += scanSpeed * Time.deltaTime;
            scanMat.SetFloat("_Distance", _range);

            if (_range > maxRange)
                scaning = false;
        }
    }

    private void FixedUpdate()
    {
        if (scaning == true)
        {
            for (int i = 0; i < scanableObjects.Count;)
            { 
                Vector3 scanObjPosition = scanableObjects[i].transform.position;
                Vector3 startPosition = scanStartPosition;
                scanObjPosition.y = startPosition.y = 0.0f;

                //Vector3 forwardDir = forward.forward;
                //forwardDir.y = 0f;
                //forwardDir.Normalize();

                if (Mathf.Acos(Vector3.Dot(scanForward, (scanObjPosition - startPosition).normalized)) * Mathf.Rad2Deg < arc)
                {
                    if ((scanObjPosition - startPosition).magnitude <= _range)
                    {
                        //Physics.Raycast(scanStartPosition, scanableObjects[i].transform.position - scanStartPosition, out hit, maxRange);

                        //if (hit.collider.gameObject == scanableObjects[i].gameObject)
                        //{
                        //    scanableObjects[i].Scanned();
                        //    scanableObjects.Remove(scanableObjects[i]);
                        //    continue;
                        //}

                        if (scanableObjects[i].CheckInAngle(scanStartPosition))
                        {
                            scanableObjects[i].Scanned();
                            scanableObjects.Remove(scanableObjects[i]);
                            continue;
                        }

                    }
                }
             
                i++;
            }
        }
    }
}
