using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class Shield_ColPosition : MonoBehaviour
{
    private GameObject fxObject;
    public Material fxMaterial;

    private Vector4 HitPosition;

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
            Debug.Log("Hit LocalPoint" + HitLocalPoint);

            ColHit(HitLocalPoint);
        }
    }

    
    void ColHit (Vector3 hitpos)
    {
        HitPosition = hitpos;
        HitPosition.w = 1.0f;
        fxMaterial.SetVector("_Hitpos",HitPosition);
    }

}
