// 2020_09_13_이우민

using UnityEngine;


//레이케스팅 헬퍼
public class RayEx
{
    public Ray RayInfo;
    public float Distance;
    public LayerMask CheckLayer;

    public RayEx(Ray ray, float dist, LayerMask layer)
    {
        RayInfo = ray;
        Distance = dist;
        CheckLayer = layer;
    }

    public void SetDirection(Vector3 direction)
    {
        RayInfo.direction = direction;
    }

    public virtual void Draw(Vector3 position,Color color)
    {
        var origin = position + RayInfo.origin;
        Debug.DrawLine(origin,origin + RayInfo.direction * Distance,color);
    }

    public virtual bool Cast(Vector3 position, out RaycastHit hit)
    {
        return Physics.Raycast(position + RayInfo.origin,RayInfo.direction,out hit,Distance, CheckLayer);
    }
}
