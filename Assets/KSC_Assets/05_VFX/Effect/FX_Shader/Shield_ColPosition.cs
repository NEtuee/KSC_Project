using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Shield_ColPosition : MonoBehaviour
{
    private GameObject fxObject;
    public Material fxMaterial;

    private Vector4 HitPosition= new Vector4 ();


    [SerializeField]
    int CurrentPos = 0;
    float CurrentTime = 0;


    [Header("VFX Setting")]
   //public string FXpostion = "FXXPos";
    public float DecreaseTime = 1.0f;
    public float ActionSpeed = 0.1f;


        private void OnEnable()
    {
        fxObject = this.gameObject;
        fxMaterial = fxObject.GetComponent<Renderer>().material;
    }

    void OnCollisionEnter (Collision coll)
   {
        foreach (ContactPoint contact in coll.contacts)
        {
            Vector3 HitLocalPoint = fxObject.transform.InverseTransformPoint(contact.point);

            ColHit(HitLocalPoint);
        }
    }

    
    void ColHit (Vector3 hitpos)
    {
        if ( CurrentTime >= ActionSpeed)
        {
            HitPosition = hitpos;
            HitPosition.w = 1.0f;

            fxMaterial.SetVector("_Hitpos", HitPosition);

            CurrentTime = 0.0f;


        }


    }
    void FXmask()
    {
        if (HitPosition.w > 0.0f)
        {
            HitPosition.w = Mathf.Lerp(HitPosition.w, 0.0f, Time.deltaTime * DecreaseTime);

            fxMaterial.SetVector("_Hitpos", HitPosition);
        }
    }

    void Update()
    {
        CurrentTime += Time.deltaTime;
        FXmask();
    }

}
