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

    public void DebugWireCube(Vector3 poistion, Transform criteria, Vector3 size,Color color)
    {
        Vector3 extents = size / 2;

        Vector3 v1 = poistion - criteria.forward * extents.z + criteria.up * extents.y - criteria.right * extents.x;
        Vector3 v2 = poistion - criteria.forward * extents.z + criteria.up * extents.y + criteria.right * extents.x;
        Vector3 v3 = poistion - criteria.forward * extents.z - criteria.up * extents.y - criteria.right * extents.x;
        Vector3 v4 = poistion - criteria.forward * extents.z - criteria.up * extents.y + criteria.right * extents.x;
        Vector3 v5 = poistion + criteria.forward * extents.z + criteria.up * extents.y - criteria.right * extents.x;
        Vector3 v6 = poistion + criteria.forward * extents.z + criteria.up * extents.y + criteria.right * extents.x;
        Vector3 v7 = poistion + criteria.forward * extents.z - criteria.up * extents.y - criteria.right * extents.x;
        Vector3 v8 = poistion + criteria.forward * extents.z - criteria.up * extents.y + criteria.right * extents.x;

        Gizmos.color = color;
        Gizmos.DrawLine(v1,v5);
        Gizmos.DrawLine(v2,v6);
        Gizmos.DrawLine(v3,v7);
        Gizmos.DrawLine(v4,v8);
        
        Gizmos.DrawLine(v1,v2);
        Gizmos.DrawLine(v3,v4);
        Gizmos.DrawLine(v1,v3);
        Gizmos.DrawLine(v2,v4);

        Gizmos.DrawLine(v5,v6);
        Gizmos.DrawLine(v7,v8);
        Gizmos.DrawLine(v5,v7);
        Gizmos.DrawLine(v6,v8);

    }
}
