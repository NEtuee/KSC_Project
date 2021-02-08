using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesignTestBoss : MonoBehaviour
{
    public Transform player;
    public Transform rayPoint;
    public Transform shell;

    public CannonShot mouseCannon;
    public List<CannonShot> cannons = new List<CannonShot>();

    public LayerMask rayMask;

    public float moveSpeed;
    public float turnSpeed;
    public float rushAngle = 30f;
    public float shotDistanceMin = 10f;
    public float shotDistanceMax = 100f;
    public float hitGrogy = 10f;

    public int scrapFindCount = 1;

    private float _targetAngle;
    private float _spinTimer = 0.1f;

    private bool _rush = false;
    private bool _hit = false;
    private bool _mouseCannon = false;
    public bool _progress = true;

    private int _scrapEatCount = 0;
    private int _scrapFindCount = 0;

    private int _destroyedCannonCount = 0;

    private List<ScrapObject> _scrapObjects;

    private Transform _target;
    private Animator _animator;

    private Vector3 _moveFactor;
    private Vector3 _rushDirection;

    private SphereRayEx headRay;

    private void Start()
    {
        headRay = new SphereRayEx(new Ray(Vector3.zero,Vector3.zero),2f,.5f,rayMask);

        _animator = GetComponent<Animator>();

        _scrapObjects = new List<ScrapObject>(GameObject.FindObjectsOfType(typeof(ScrapObject)) as ScrapObject[]);

        var portals = GameObject.FindObjectsOfType(typeof(PortalProgress)) as PortalProgress[];
        foreach(var portal in portals)
        {
            portal.whenHit += Hit;
        }

        foreach(var cannon in cannons)
        {
            cannon.whenDestroy += CannonDestroy;
        }
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            Hit();
        }

        if(Input.GetKeyDown(KeyCode.T))
        {
            foreach(var cannon in cannons)
            {
                cannon.DestroyProgress();
            }
        }

        if(!_progress)
            return;

        foreach(var cannon in cannons)
        {
            if(cannon.gameObject.activeInHierarchy)
            {
                cannon.canDestroy = _hit;
                cannon.CanShot(player);
            }
        }

        mouseCannon.CanShot(player);

        if(_hit)
        {
            _spinTimer += Time.deltaTime;

            if(_spinTimer >= hitGrogy)
            {
                _spinTimer = 1f;
                _hit = false;
                _animator.SetLayerWeight(1,1f);
                _animator.SetLayerWeight(2,1f);
            }

            return;
        }

        if(_rush)
        {
            _moveFactor = _rushDirection * moveSpeed * Time.deltaTime;

            RaycastHit hit;
            headRay.SetDirection(_rushDirection);
            //headRay.Draw(rayPoint.position,Color.red);
            if(headRay.Cast(rayPoint.position,out hit))
            {
                ScrapObject scrap = null;
                if(hit.transform.TryGetComponent<ScrapObject>(out scrap))
                {
                    _animator.SetBool("Eat",true);
                    scrap.Eat();
                    _scrapEatCount++;
                }
                else
                {
                    _animator.SetBool("Crash",true);
                    _animator.SetLayerWeight(1,0f);
                }
                
                _animator.SetBool("Move",false);

                _spinTimer = 5f;
                _rush = false;
            }
        }
        else
        {
            if(_spinTimer != 0f)
            {
                _spinTimer -= Time.deltaTime;

                if(_spinTimer <= 0f)
                {
                    _spinTimer = 0f;
                    _animator.SetBool("Crash",false);
                    _animator.SetBool("Eat",false);
                    _animator.SetBool("Move",true);

                    _animator.SetLayerWeight(1,1f);

                    if(_scrapFindCount++ >= scrapFindCount)
                    {
                        var obj = FindFarthestScrapObject();
                        _target = obj == null ? player : obj.transform;
                        _scrapFindCount = 0;
                    }
                    else
                    {
                        _target = player;
                    }
                }
                
                
            }
            else
            {
                var direction = _target.position - transform.position;
                var forward = -Vector3.Cross(transform.forward,Vector3.up).normalized;
                direction.y = 0f;

                _targetAngle = Vector3.Angle(direction.normalized,forward);

                if(_targetAngle <= rushAngle)
                {
                    _rushDirection = forward;
                    _rushDirection.y = 0f;
                    _rush = true;
                }
                else
                {
                    var rotate = Quaternion.LookRotation(direction.normalized,Vector3.up);
                    rotate.eulerAngles -= new Vector3(0f,90f,0f);
                    transform.rotation = Quaternion.RotateTowards(transform.rotation,rotate,Time.deltaTime * turnSpeed);
                }
            }

        }
    }

    private void FixedUpdate()
    {
        transform.position += _moveFactor * Time.deltaTime;
        _moveFactor = Vector3.zero;
    }

    public void Explosion()
    {
        _animator.enabled = false;
        
        for(int i = 0; i < transform.childCount; ++i)
        {
            Explosion(transform.GetChild(i));
        }

        this.enabled = false;
    }

    public void Explosion(Transform root)
    {
        Rigidbody rig = null;
        if(!root.TryGetComponent<Rigidbody>(out rig))
        {
            if(root.GetComponent<Collider>() != null)
            {
                //root.parent = null;
                rig = root.gameObject.AddComponent<Rigidbody>();
                rig.AddForce((root.position - transform.position).normalized * 4000f,ForceMode.Force);
                rig.AddTorque(MathEx.RandomVector3(new Vector3(-10f,-10f,-10f),new Vector3(10f,10f,10f)));
            }
        }

        if(root.tag == "Player")
            return;
        
        for(int i = 0; i < root.childCount; ++i)
        {
            Explosion(root.GetChild(i));
        }
    }

    public void MouseCannonShot()
    {
        if(_mouseCannon)
            mouseCannon.Shot();
    }

    public void CannonDestroy()
    {
        _spinTimer = 8f;
        _hit = false;
        _animator.SetLayerWeight(1,1f);
        _animator.SetLayerWeight(2,1f);
        _animator.SetBool("Crash",false);
        _animator.SetBool("Eat",false);
        _animator.SetBool("Move",false);
        _animator.SetTrigger("CannonDestroy");

        ++_destroyedCannonCount;

        if(_destroyedCannonCount >= cannons.Count)
        {
            _mouseCannon = true;
            shell.gameObject.SetActive(false);
        }
    }

    public void Hit()
    {
        if(!_progress)
            return;

        if(_mouseCannon)
        {
            _progress = false;
            _animator.SetTrigger("Flip");

            _animator.SetLayerWeight(1,0f);
            _animator.SetLayerWeight(2,0f);
        }
        else
        {
            if(!_hit)
            {
                _animator.SetBool("Crash",false);
                _animator.SetBool("Eat",false);
                _animator.SetBool("Move",false);
                _animator.SetTrigger("Hit");

                _animator.SetLayerWeight(1,0f);
                _animator.SetLayerWeight(2,0f);
            }
            _rush = false;
            _hit = true;

            _target = player;
            _spinTimer = 0f;
        }
        
    }

    public ScrapObject FindFarthestScrapObject()
    {
        if(_scrapObjects.Count <= 0)
            return null;
        
        float max = -1f;
        ScrapObject target = null;

        for(int i = 0; i < _scrapObjects.Count; ++i)
        {
            if(!_scrapObjects[i].isReady)
                continue;

            float dist = Vector3.Distance(transform.position, _scrapObjects[i].transform.position);
            if(dist > max || max == -1f)
            {
                max = dist;
                target = _scrapObjects[i];
            }
        }

        return target;
    }

    public ScrapObject FindNearestScrapObject()
    {
        if(_scrapObjects.Count <= 0)
            return null;
        
        float min = -1f;
        ScrapObject target = null;

        for(int i = 0; i < _scrapObjects.Count; ++i)
        {
            if(!_scrapObjects[i].isReady)
                continue;

            float dist = Vector3.Distance(transform.position, _scrapObjects[i].transform.position);
            if(dist < min || min == -1f)
            {
                min = dist;
                target = _scrapObjects[i];
            }
        }

        return target;
    }

    public void Shot(int point)
    {
        var one = transform.position;
        var two = player.transform.position;
        one.y = 0f;
        two.y = 0f;

        var distance = Vector3.Distance(one,two);


        if(distance <= shotDistanceMax && distance >= shotDistanceMin)
        {
            cannons[point].Shot();
        }
    }
}
