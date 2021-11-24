using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using MD;
public class DroneScaner : UnTransfromObjectBase
{
    public Transform forward;
    public Transform scanStart;
    public Material scanMat;
    public float arc = 30f;
    public float scanSpeed = 80f;

    private Vector3 scanStartPosition;

    [SerializeField] private bool scaning = false;
    [SerializeField] private float maxRange = 1000.0f;
    [SerializeField] private float _range = 0f;
    private Vector3 scanForward;

    [SerializeField] private List<string> scanableTags = new List<string>();
    [SerializeField] private List<Scanable> scanableObjects = new List<Scanable>();

    public List<MessageReceiver> _scanableMessageObjects = new List<MessageReceiver>();
    private HashSet<int> _scannedNumbers = new HashSet<int>();

    private RaycastHit hit;
    protected override void Awake()
    {
        base.Awake();
        FindScanableObjects();
        SceneManager.sceneLoaded += (s, w) => 
        { 
            FindScanableObjects(); 
            //_scanableMessageObjects.Clear();
            _scannedNumbers.Clear();
        };
    }

    public override void Initialize()
    {
        base.Initialize();
        RegisterRequest(GetSavedNumber("PlayerManager"));

        scanMat.SetFloat("_Distance", 0f);
    }

    public void ResetEffect()
    {
        scaning = false;
        scanMat.SetFloat("_Distance", 0f);
    }

    public void Scanning()
    {
        scaning = true;
        _range = 0f;
        scanMat.SetFloat("_ScanArc", arc);
        scanStartPosition = transform.position;
        scanMat.SetVector("_WorldSpaceScannerPos", scanStart.position);
        scanForward = forward.forward;
        scanForward.y = 0.0f;
        scanForward.Normalize();
        scanMat.SetVector("_ForwardDirection", scanForward);
            
        //GameManager.Instance.soundManager.Play(1301, Vector3.zero, transform);
        AttachSoundPlayData soundData = MessageDataPooling.GetMessageData<AttachSoundPlayData>();
        soundData.id = 1301; soundData.localPosition = Vector3.zero; soundData.parent = transform; soundData.returnValue = false;
        SendMessageEx(MessageTitles.fmod_attachPlay, GetSavedNumber("FMODManager"), soundData);

        _scannedNumbers.Clear();

        EffectActiveData effectData = MessageDataPooling.GetMessageData<EffectActiveData>();
        effectData.key = "BirdyScanning";
        effectData.position = scanStart.position;
        effectData.rotation = transform.rotation;
        effectData.parent = transform;
        SendMessageEx(MessageTitles.effectmanager_activeeffectsetparent,GetSavedNumber("EffectManager"),effectData);
    }

    void Update()
    {
        //if (GameManager.Instance.PAUSE == true)
        //    return;

        // if (Input.GetKeyDown(KeyCode.V))
        // {
        //     Scanning();
        // }

        if (scaning == true)
        {
            _range += scanSpeed * Time.deltaTime;
            scanMat.SetFloat("_Distance", _range);

            if (_range > maxRange)
                scaning = false;
        }
    }

    private void FixedUpdate()
    {
        //if (GameManager.Instance.PAUSE == true)
        //    return;

        if (scaning == true)
        {
            //ScanObject();
            ScanMessageObject();
        }
    }

    public void AddScanMessageObject(MessageReceiver receiver)
    {
        if(receiver == null)
        {
            Debug.Log("MIkk");
        }
        _scanableMessageObjects.Add(receiver);
    }

    public void ScanMessageObject()
    {
        for (int i = 0; i < _scanableMessageObjects.Count;)
        { 
            if(_scanableMessageObjects[i] == null)
            {
                _scanableMessageObjects.RemoveAt(i);
                continue;
            }

            if(_scannedNumbers.Contains(_scanableMessageObjects[i].uniqueNumber) || !_scanableMessageObjects[i].gameObject.activeInHierarchy)
            {
                ++i;
                continue;
            }


            Vector3 scanObjPosition = _scanableMessageObjects[i].transform.position;
            Vector3 startPosition = scanStartPosition;
            scanObjPosition.y = startPosition.y = 0.0f;

            if (Mathf.Acos(Vector3.Dot(scanForward, (scanObjPosition - startPosition).normalized)) * Mathf.Rad2Deg < arc)
            {
                float mag = (scanObjPosition - startPosition).magnitude;

                if (mag <= _range && mag >= _range - 3f)
                {
                    _scannedNumbers.Add(_scanableMessageObjects[i].uniqueNumber);
                    SendMessageEx(_scanableMessageObjects[i],MessageTitles.scan_scanned,null);

                }
            }
         
            i++;
        }
    }

    public void ScanObject()
    {
        for (int i = 0; i < scanableObjects.Count;)
        { 
            if(scanableObjects[i] == null)
            {
                scanableObjects.RemoveAt(i);
                continue;
            }

            if(_scannedNumbers.Contains(scanableObjects[i].uniqueNumber))
            {
                ++i;
                continue;
            }

            Vector3 scanObjPosition = scanableObjects[i].transform.position;
            Vector3 startPosition = scanStartPosition;
            scanObjPosition.y = startPosition.y = 0.0f;

            //Vector3 forwardDir = forward.forward;
            //forwardDir.y = 0f;
            //forwardDir.Normalize();

            if (Mathf.Acos(Vector3.Dot(scanForward, (scanObjPosition - startPosition).normalized)) * Mathf.Rad2Deg < arc)
            {
                float mag = (scanObjPosition - startPosition).magnitude;

                if (mag <= _range && mag >= _range - 3f)
                {
                    if(!scanableObjects[i].IsTriggered())
                    {
                        Debug.Log(scanableObjects[i].name);
                        scanableObjects[i].Scanned();
                        _scannedNumbers.Add(scanableObjects[i].uniqueNumber);
                    }
                    

                    // if (scanableObjects[i].CheckInAngle(scanStartPosition))
                    // {
                    //     if(!scanableObjects[i].IsTriggered())
                    //         scanableObjects[i].Scanned();
                        
                    //     if(scanableObjects[i].removeCheck)
                    //         scanableObjects.Remove(scanableObjects[i]);
                    //     else
                    //     {
                    //         ++i;
                    //     }
                        
                    //     continue;
                    // }

                }
            }
         
            i++;
        }
    }

    public void FindScanableObjects()
    {
        scanableObjects.Clear();
        foreach(string tag in scanableTags)
        {
            GameObject[] scanableObj= GameObject.FindGameObjectsWithTag(tag);
            Scanable currentScanable;
            for(int i = 0; i < scanableObj.Length; i++)
            {
                if(scanableObj[i].TryGetComponent<Scanable>(out currentScanable))
                {
                    
                    scanableObjects.Add(currentScanable);
                }
            }
        }
    }

    public void AddScanableObjets(GameObject scanable)
    {
        if (scanable == null)
            return;

        if (scanable.TryGetComponent<Scanable>(out Scanable currentScanable))
        {
            scanableObjects.Add(currentScanable);
        }
    }
}
