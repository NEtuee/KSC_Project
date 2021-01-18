//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using Cinemachine;

//public class CinemachineFreeLookZoom : MonoBehaviour
//{
//    private CinemachineFreeLook freeLook;
//    private CinemachineFreeLook.Orbit[] originalOrbits;

//    [Range(0.5f, 1.0f)]
//    public float zoomPrecent;

//    private void Awake()
//    {
//        freeLook = GetComponent<CinemachineFreeLook>();
//        originalOrbits = new CinemachineFreeLook.Orbit[freeLook.m_Orbits.Length];

//        for (int i = 0; i < freeLook.m_Orbits.Length; i++)
//        {
//            originalOrbits[i].m_Height = freeLook.m_Orbits[i].m_Height;
//            originalOrbits[i].m_Radius = freeLook.m_Orbits[i].m_Radius;
//        }
//    }

//    void Update()
//    {
//        float wheel = Input.GetAxis("Mouse ScrollWheel");
//        zoomPrecent += wheel * 10f * Time.deltaTime;
//        zoomPrecent = Mathf.Clamp(zoomPrecent, 0.5f, 1.0f);

//        for (int i = 0; i < freeLook.m_Orbits.Length; i++)
//        {
//            freeLook.m_Orbits[i].m_Height = originalOrbits[i].m_Height * zoomPrecent;
//            freeLook.m_Orbits[i].m_Radius = originalOrbits[i].m_Radius * zoomPrecent;
//        }
//    }
//}
