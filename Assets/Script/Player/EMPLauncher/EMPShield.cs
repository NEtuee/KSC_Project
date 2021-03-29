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
    [SerializeField] private float hp = 100f;
    public Color scanColor;
    private float shakeTime = 0.0f;
    private Vector3 originalPosition;

    private Renderer renderer;
    private Collider collider;

    private Material mat;

    [SerializeField] private bool visibleCheck;
    [SerializeField] private string currentRenderCamera;

    void Start()
    {
        renderer = GetComponent<Renderer>();
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
}
