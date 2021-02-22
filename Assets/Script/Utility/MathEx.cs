using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathEx : MonoBehaviour {

	public static Vector3 PerpendicularPoint(Vector3 start, Vector3 end, Vector3 point)
	{
		return Vector3.Project(point - start, end - start) + start;
	}

	public static Vector3 GetSlidingVector(Vector3 velocity, Vector3 normal)
	{
		return (velocity - Vector3.Dot(velocity, normal) * normal);
	}

	public static Vector3 DeleteYPos(Vector3 value)
	{
		return new Vector3(value.x,0f,value.z);
	}

	public static Vector3 LerpAngle(Vector3 start, Vector3 end, float time)
	{
		return new Vector3(Mathf.LerpAngle(start.x,end.x,time),Mathf.LerpAngle(start.y,end.y,time),Mathf.LerpAngle(start.z,end.z,time));
	}

	public static float PlaneAngle(Vector3 one, Vector3 two, Vector3 axis)
	{
		var o = Vector3ToVector2(one,axis);
		var t = Vector3ToVector2(two,axis);

		return Vector2.Angle(o,t);
	}

	public static Vector3 Vector3ToVector2(Vector3 value, Vector3 axis)
	{
		if(axis.x == 1f)
			return new Vector3(value.z,value.y);
		else if(axis.y == 1f)
			return new Vector3(value.x,value.z);
		else if(axis.z == 1f)
			return new Vector3(value.x,value.y);
		
		return value;
	}

	public static Vector2 Vector3ToVector2(Vector3 value)
	{
		return new Vector2(value.x,value.z);
	}

	public static Vector3 Vector2ToVector3(Vector2 value, float y = 0f)
	{
		return new Vector3(value.x,y,value.y);
	}

	public static float DistanceFromPointToLine(Vector2 point, Vector2 line0, Vector2 line1)
    {
        Vector2 l1 = line0;
        Vector2 l2 = line1;

    	return abs((l2.x - l1.x)*(l1.y - point.y) - (l1.x - point.x)*(l2.y - l1.y))/
                Mathf.Sqrt(Mathf.Pow(l2.x - l1.x, 2) + Mathf.Pow(l2.y - l1.y, 2));
    }

	public static float GetBezierLengthStupidWay2D(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int accur)
	{
		return GetBezierLengthStupidWay(Vector3ToVector2(p0),
									Vector3ToVector2(p1),
									Vector3ToVector2(p2),
									Vector3ToVector2(p3),accur);
	}

	public static float GetBezierLengthStupidWay(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, int accur)
	{
		var start = p0;
		float len = 0f;
		for(int i = 1; i < accur; ++i)
		{
			var point = GetPointOnBezierCurve(p0,p1,p2,p3,(float)i / accur);
			len += Vector3.Distance(start,point);

			start = point;
		}

		return len;
	}

	public static Vector2 GetPointOnBezierCurve2D(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{
		return GetPointOnBezierCurve(Vector3ToVector2(p0),
									Vector3ToVector2(p1),
									Vector3ToVector2(p2),
									Vector3ToVector2(p3),t);
	}

	public static Vector3 GetPointOnBezierCurve(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
	{	
		float tt = t * t;
		float ttt = t * tt;
		float u = 1.0f - t;
		float uu = u * u;
		float uuu = u * uu;
		
		Vector3 B = new Vector3();
		B = uuu * p0;
		B += 3.0f * uu * t * p1;
		B += 3.0f * u * tt * p2;
		B += ttt * p3;
		
		return B;
	}

	public static void Swap<T>(ref T one, ref T two)
	{
		T save = one;
		one = two;
		two = save;
	}
	public static int abs(int value) {return value < 0 ? - value : value;}
	public static float abs(float value) {return value < 0 ? -value : value;}
	public static float normalize(float value) {return value < 0 ? -1 : (value == 0 ? 0 : 1);}
	public static float tiltZero(float value, float factor)
	{
		var sign = normalize(value);
		var result = clampOverZero(abs(value) - factor);
		return result * sign;
	}
	public static float nearZero(float value) {return abs(value) < 0.0001f ? 0 : value;}
	public static float distance(float x1, float x2) {return abs(x1 - x2);}
	public static float vectorScale(Vector3 v) {return (abs(v.x) + abs(v.y));}
	//public static float directionToAngle(Vector2 dir) {return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;}
	public static float xzDistance(Vector3 one, Vector3 two)
	{
		one.y = 0f;
		two.y = 0f;

		return Vector3.Distance(one,two);
	}
	public static Vector3 xzDirection(Vector3 one, Vector3 two)
	{
		one.y = 0f;
		two.y = 0f;

		return one - two;
	}
	public static float directionToAngle(Vector2 dir) 
	{
		float val = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

		return clamp360Degree(val);
	}
	public static Vector3 angleToDirection(float angle) {return new Vector3(Mathf.Cos(angle),Mathf.Sin(angle));}
	public static Vector3 clampOverZero(Vector3 value) {return new Vector3(clampOverZero(value.x),clampOverZero(value.y),clampOverZero(value.z));}
 	public static float clampOverZero(float value) {return value < 0 ? 0f : value;}
	public static float clamp360Degree(float eulerAngle)
    {
        //  float val = eulerAngle - Mathf.CeilToInt(eulerAngle / 360f) * 360f;
		//  val = val < 0 ? val + 360f : val;
		float val = eulerAngle + ((float)((int)-eulerAngle / 360) * 360f);
		val = val < 0 ? val + 360f : val;
		return val;
    }
	public static float FlipLeftAngle(float angle) //clamp angle only
	{
		float a = 180f - angle;
		a = 180 + a;
		return a;
	}
	public static Vector2 easeOutCubicVector2(Vector2 start, Vector2 end ,float time)
	{
		return new Vector2(easeOutCubic(start.x,end.x,time),easeOutCubic(start.y,end.y,time));
	}
	public static Vector3 easeOutCubicVector3(Vector3 start, Vector3 end ,float time)
	{
		return new Vector3(easeOutCubic(start.x,end.x,time),easeOutCubic(start.y,end.y,time),easeOutCubic(start.z,end.z,time));
	}
	public static float easeOutCubic(float start, float end, float value)
	{
		value--;
		end -= start;
		return end * (value * value * value + 1) + start;
	}
	public static float easeInCubic(float start, float end, float value)
	{
		end -= start;
		return end * value * value * value + start;
	}
	public static bool LineIntersection(out Vector2 intersection, Vector2 linePoint1, Vector2 lineVec1, Vector2 linePoint2, Vector2 lineVec2)
	{
	        Vector3 lineVec3 = linePoint2 - linePoint1;
	        Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
	        Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);
	
	        float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);
	
	        //is coplanar, and not parrallel
	        if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
	        {
	            float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
	            intersection = linePoint1 + (lineVec1 * s);
	            return true;
	        }
	        else
	        {
	            intersection = Vector3.zero;
	            return false;
	        }
	}
	public static Vector3 Lemniscate_Gerono(float factor, float time)
	{
		return new Vector3(Mathf.Cos(time), Mathf.Sin(2*time) * .5f) * factor;
	}
	public static void nearZero(ref Vector3 value) {value.x = nearZero(value.x); value.y = nearZero(value.y);}
	public static bool halfCompare(float one,float two) {return abs((one - two)) <= 0.0001f;}
	public static int Vector3Compare(Vector3 one, Vector3 two)
	{
		float x = abs(one.x) + abs(one.y);
		float y = abs(two.x) + abs(two.y);

		return x == y ? 0 : (x > y ? 1 : 2);
	}
	public static bool Vector3ValueEqual(Vector3 one, Vector3 two)
	{
		return one.x == two.x ? (one.y == two.y ? (one.z == two.z) : false) : false;
	}

	public static Vector3 RandomVector3(Vector3 min, Vector3 max)
	{
		return new Vector3(Random.Range(min.x,max.x),Random.Range(min.y,max.y),Random.Range(min.z,max.z));
	}

	public static Vector3 RandomVector3(float xmin, float xmax, float ymin, float ymax, float zmin, float zmax)
	{
		return new Vector3(Random.Range(xmin,xmax),Random.Range(ymin,ymax),Random.Range(zmin,zmax));
	}

	public static Vector3 RandomVector3(float xmin, float xmax, float ymin, float ymax)
	{
		return new Vector3(Random.Range(xmin,xmax),Random.Range(ymin,ymax),0f);
	}

	public static Vector3 RandomCircle(float radius)
	{
		return new Vector3(Random.Range(-radius,radius),Random.Range(-radius,radius),0f);
	}

	public static Vector3 RandomVector3(float min, float max)
	{
		return new Vector3(Random.Range(min,max),Random.Range(min,max),0f);
	}

	public static int RandomInt(int start, int end)
	{
		return Random.Range(start,end + 1);
	}
}
