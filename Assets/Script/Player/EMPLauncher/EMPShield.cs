using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EMPShield : Scanable
{
    public UnityEvent whenDestroy;
    public GameObject destroyEffect;
    public bool isOver = false;
    public bool isCore = false;
    [SerializeField] private bool debug;
    [SerializeField] private float hp = 100f;
    public Color scanColor;
    private float shakeTime = 0.0f;
    private Vector3 originalPosition;

    private Collider collider;

    private Material mat;

    void Start()
    {
        base.Start();
        collider = GetComponent<Collider>();
        mat = renderer.material;
    }

    // Update is called once per frame
    void Update()
    {
        if (shakeTime > 0.0f)
        {
            transform.position = (Vector3)Random.insideUnitCircle * 0.2f + originalPosition;
            shakeTime -= Time.deltaTime;
            if (shakeTime <= 0.0f)
            {
                transform.localPosition = originalPosition;
            }
        }

        if(debug)
        {
            //Vector3 cameraPosition = GameManager.Instance.GetPlayerPosition();
            Vector3 cameraPosition = ((PlayerCtrl_Ver2)GameManager.Instance.player).GetPlayerCenter();

            Bounds bound = renderer.bounds;
            Vector3 extents = bound.extents;
            Vector3 point;
            point = bound.center + new Vector3(-extents.x, -extents.y, extents.z);
            Debug.DrawLine(cameraPosition, point, Color.red);
            //2
            point = bound.center + new Vector3(extents.x, -extents.y, extents.z);
            Debug.DrawLine(cameraPosition, point, Color.red);

            //3
            point = bound.center + new Vector3(-extents.x, -extents.y, -extents.z);
            Debug.DrawLine(cameraPosition, point, Color.red);

            //4
            point = bound.center + new Vector3(extents.x, extents.y, -extents.z);
            Debug.DrawLine(cameraPosition, point, Color.red);

            //5
            point = bound.center + new Vector3(-extents.x, extents.y, extents.z);
            Debug.DrawLine(cameraPosition, point, Color.red);

            //6
            point = bound.center + new Vector3(extents.x, extents.y, extents.z);
            Debug.DrawLine(cameraPosition, point, Color.red);

            //7
            point = bound.center + new Vector3(-extents.x, extents.y, -extents.z);
            Debug.DrawLine(cameraPosition, point, Color.red);

            //8
            point = bound.center + new Vector3(extents.x, extents.y, -extents.z);
            Debug.DrawLine(cameraPosition, point, Color.red);

        }
    }

    private void FixedUpdate()
    {
        CheckVisible();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("EMP"))
        {
            Hit();
        }
    }

    public void Hit()
    {
        originalPosition = transform.localPosition;
        hp -= 100f;
        shakeTime = 0.1f;
        if (hp <= 0f)
        {
            Destroy(Instantiate(destroyEffect, transform.position, transform.rotation), 3.5f);
            collider.enabled = false;
            renderer.enabled = false;
            isOver = true;

            whenDestroy.Invoke();
            //Destroy(gameObject);
        }
    }

    public void Hit(float damage)
    {
        originalPosition = transform.localPosition;
        hp -= damage;
        shakeTime = 0.1f;
        if (hp <= 0f)
        {
            Destroy(Instantiate(destroyEffect, transform.position, transform.rotation), 3.5f);
            collider.enabled = false;
            renderer.enabled = false;
            isOver = true;

            whenDestroy.Invoke();
            //Destroy(gameObject);
        }
    }

    public void Hit(float damage, out bool isDestroy)
    {
        originalPosition = transform.localPosition;

        if (isCore == true)
        {
            hp -= 100.0f;
        }
        else
        {
            hp -= damage;
        }
        shakeTime = 0.1f;
        isDestroy = false;
        if (hp <= 0f)
        {
            isDestroy = true;
            Destroy(Instantiate(destroyEffect, transform.position, transform.rotation), 3.5f);
            collider.enabled = false;
            renderer.enabled = false;
            isOver = true;

            whenDestroy.Invoke();
            //Destroy(gameObject);
        }
    }

    public override void Scanned()
    {
        mat.SetColor("_BaseColor", scanColor);
    }

    //private void OnBecameVisible()
    //{
    //    //if (visibleCheck == false)
    //    //    return;

    //    //Debug.Log("visibe");

    //    visible = true;
    //}

    //private void OnBecameInvisible()
    //{
    //    //if (visibleCheck == false)
    //    //    return;
    //    //        Debug.Log("Invisible");
    //    //#if UNITY_EDITOR
    //    //        if (Camera.current != null)
    //    //        {
    //    //            if (Camera.current.name == "SceneCamera")
    //    //                return;
    //    //        }
    //    //#endif

    //    visible = false;
    //}

    private void OnWillRenderObject()
    {
        //if(Camera.current != null)
        //{
        //    visible = false;
        //    currentRenderCamera = Camera.current.name;
        //    if (Camera.current.name == "Preview Camera")
        //    {
        //        visible = true;
        //    }
        //}
        //else
        //{
        //    currentRenderCamera = "null";
        //    visible = true;
        //}
    }

    public bool GetIsVisible()
    {
        return visible;
    }

    public bool CheckInCamera()
    {
        Vector3 cameraPosition = ((PlayerCtrl_Ver2)GameManager.Instance.player).GetPlayerCenter();
        Bounds bound = renderer.bounds;
        Vector3 extents = bound.extents;

        Vector3 point;

        //1
        point = bound.center + new Vector3(-extents.x, -extents.y, extents.z);
        if(Physics.Linecast(point,cameraPosition, visibleCastLayer) == false)
        {
            return true;
        }
        //2
        point = bound.center + new Vector3(extents.x, -extents.y, extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            return true;
        }
        //3
        point = bound.center + new Vector3(-extents.x, -extents.y, -extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            return true;
        }
        //4
        point = bound.center + new Vector3(extents.x, extents.y, -extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            return true;
        }
        //5
        point = bound.center + new Vector3(-extents.x, extents.y, extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            return true;
        }
        //6
        point = bound.center + new Vector3(extents.x, extents.y, extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            return true;
        }
        //7
        point = bound.center + new Vector3(-extents.x, extents.y, -extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            return true;
        }
        //8
        point = bound.center + new Vector3(extents.x, extents.y, -extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            return true;
        }

        return false;
    }

    private void CheckVisible()
    {
        //Vector3 cameraPosition = GameManager.Instance.cameraManger.GetCameraPosition();
        //Vector3 cameraPosition = GameManager.Instance.GetPlayerPosition();
        Vector3 cameraPosition = ((PlayerCtrl_Ver2)GameManager.Instance.player).GetPlayerCenter();
        Bounds bound = renderer.bounds;
        Vector3 extents = bound.extents;

        RaycastHit hit;

        Vector3 point;

        //1
        point = bound.center + new Vector3(-extents.x, -extents.y, extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            visible = true;
            return;
        }

        //2
        point = bound.center + new Vector3(extents.x, -extents.y, extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            visible = true;
            return;
        }
        //3
        point = bound.center + new Vector3(-extents.x, -extents.y, -extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            visible = true;
            return;
        }
        //4
        point = bound.center + new Vector3(extents.x, extents.y, -extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            visible = true;
            return;
        }
        //5
        point = bound.center + new Vector3(-extents.x, extents.y, extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            visible = true;
            return;
        }
        //6
        point = bound.center + new Vector3(extents.x, extents.y, extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            visible = true;
            return;
        }
        //7
        point = bound.center + new Vector3(-extents.x, extents.y, -extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            visible = true;
            return;
        }
        //8
        point = bound.center + new Vector3(extents.x, extents.y, -extents.z);
        if (Physics.Linecast(point, cameraPosition, visibleCastLayer) == false)
        {
            visible = true;
            return;
        }

        visible = false;
        return;
    }
}
