using UnityEngine;

public class DebugCastDetection 
{
    private static DebugCastDetection instance;
    private RaycastHit hit;
    public static DebugCastDetection Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new DebugCastDetection();
            }
            return instance;
        }
    }

    public bool DebugSphereCastDetection(Vector3 start, float radius, Vector3 direction, float maxDistance, int layerMask, Color defaultColor , Color detectionColor)
    {
        bool isHit = Physics.SphereCast(start, radius, direction, out hit, maxDistance, layerMask);
        Gizmos.color = defaultColor;
        if(isHit)
        {
            Gizmos.DrawRay(start, direction * hit.distance);
            Gizmos.DrawWireSphere(start + direction * hit.distance, radius);
            Gizmos.color = detectionColor;
            Gizmos.DrawWireSphere(hit.point, 0.15f);
        }
       
        return isHit;
    }

    public bool DebugBoxCastDetection(Vector3 start, Vector3 extents, Vector3 direction, Quaternion rotation, float distance,int layerMask, Color defaultColor, Color detectionColor)
    {
        bool isHit = Physics.BoxCast(start, extents, direction,out hit, rotation, distance, layerMask);
        Gizmos.color = defaultColor;
        if(isHit)
        {
            Gizmos.DrawRay(start, direction * hit.distance);
            Gizmos.DrawWireCube(start + direction * hit.distance, extents*2f);
            Gizmos.color = detectionColor;
            Gizmos.DrawWireSphere(hit.point, 0.15f);
        }

        return isHit;
    }
}
