using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoogieCannonball : EMPShield
{
    public Vector3Int targetCube;

    private float _height;
    private float _speed;

    private float _timer = 0f;
    private Vector3 _end;
    private Vector3 _start;

    public bool Progress(float deltaTime)
    {
        _timer += deltaTime * _speed;
        var pos = Vector3.Lerp(_start,_end,_timer);
        pos.y = pos.y + Mathf.Sin(_timer * Mathf.PI) * _height;
        transform.position = pos;

        if(_timer >= 1f)
        {
            Hit();
            return true;
        }

        return false;
    }

    public override void Hit()
    {
        gameObject.SetActive(false);
        GameManager.Instance.effectManager.Active("CannonExplosion",transform.position,transform.rotation);
    }

    public override void Hit(float damage)
    {
        Hit();
    }

    public void Active(Vector3Int target, Vector3 start, Vector3 end, float speed, float height)
    {
        transform.position = start;

        targetCube = target;
        _start = start;
        _end = end;
        _speed = speed;
        _height = height;

        _timer = 0f;

        gameObject.SetActive(true);
    }
}
