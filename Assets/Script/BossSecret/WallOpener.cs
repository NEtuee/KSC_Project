using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallOpener : MonoBehaviour
{
    public AnimationCurve openCurve;
    public ImpactTarget target;

    public float speed = 1f;


    private bool _open = false;
    private bool defferdOpen = false;
    [SerializeField]private bool defferdMode = true;
    private float _timer = 0f;

    private Vector3 _startPosition;

    private void Start()
    {
        _startPosition = transform.position;

        if (defferdMode == true)
        {
            target.whenTriggerOn += DefferdOpen;
        }
        else
        {
            target.whenTriggerOn += Open;
        }
    }

    private void Update()
    {
        if(_open)
        {
            _timer += speed * Time.deltaTime;
            transform.position = _startPosition + new Vector3(0f,openCurve.Evaluate(_timer),0f);

            if(_timer >= 1f)
            {
                _open = false;
            }
        }

        if(Input.GetKeyDown(KeyCode.H))
        {
            Open();
        }
    }

    public void Open()
    {
        _open = true;
        _timer = 0f;
    }

    public void DefferdOpen()
    {
        if(defferdOpen == false)
        {
            defferdOpen = true;
            StartCoroutine(OpenCoroutine(2f));
        }
    }

    IEnumerator OpenCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        Open();
        defferdOpen = false;
    }
}
