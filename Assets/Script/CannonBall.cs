using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    public GameObject explosionParticle;
    public float speed;
    public float height;
    private float _timer = 0f;

    private Vector3 _startPosition;
    private Vector3 _targetPosition;

    private GameObject child;

    public void Shot(Vector3 position, Vector3 target)
    {
        _timer = 0f;
        _startPosition = position;
        _targetPosition = target;

        child = transform.GetChild(0).gameObject;
    }

    void Update()
    {
        _timer += speed * Time.deltaTime;
        if(_timer >= 1f)
        {
            child.transform.parent = null;
            child.transform.localScale = Vector3.one;

            Destroy(Instantiate(explosionParticle,transform.position,Quaternion.identity),3.5f);
            Destroy(child,1f);
            Destroy(this.gameObject);
        }
    }

    private void FixedUpdate()
    {
        var pos = Vector3.Lerp(_startPosition,_targetPosition,_timer);
        pos.y += height * Mathf.Sin(_timer * Mathf.PI);
        transform.position = pos;
    }
}
