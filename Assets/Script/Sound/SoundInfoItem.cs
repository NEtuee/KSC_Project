using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SoundInfoItem", order = 1)]
public class SoundInfoItem : ScriptableObject
{
    [System.Serializable]
    public class SoundParameter
    {
        public string name;
        public int id;
        public float min;
        public float max;
    }
    [System.Serializable]
    public class SoundInfo
    {
        public string path;
        public string name;
        public int id;
        public float defaultVolume = 1f;
        public bool overrideAttenuation = false;
        public Vector3 overrideDistance;
        public string type;
        public List<SoundParameter> parameters;

        public SoundParameter FindParameter(int id)
        {
            return parameters.Find(x=> x.id == id);
        }

        public SoundInfo()
        {
            parameters = new List<SoundParameter>();
        }

    };

    public TextAsset csvData;

    [SerializeField]
    public List<SoundInfo> soundData;

    public SoundInfo FindSound(int id)
    {
        return soundData.Find(x => x.id == id);
    }

#if UNITY_EDITOR
    public void CreateInfoFromCSV()
    {
        CreateInfoFromCSV(csvData);
    }


    public void CreateInfoFromCSV(TextAsset csv)
    {
        var saveData = new List<SoundInfo>();
        //soundData = new List<SoundInfo>();
        IOControl.ReadRangeFromCSV(csv.text,1,-1,0,2,out var data);
        IOControl.ReadRangeFromCSV(csv.text,1,-1,4,8,out var param);

        var globalData = new SoundInfo();
        globalData.id = 0;
        globalData.path = "global data";
        globalData.type = "Global";

        saveData.Add(globalData);
        
        if(data != null && param != null)
        {
            foreach(var d in data)
            {
                if(d[0] == "")
                    continue;

                var item = new SoundInfo();
                item.id = int.Parse(d[0]);
                item.path = d[1];
                var split = item.path.Split('/');
                item.name = split[split.Length - 1];
                item.type = d[2];


                if(soundData != null)
                {
                    var sound = FindSound(item.id);
                    if(sound != null)
                    {
                        item.defaultVolume = sound.defaultVolume;
                        item.overrideAttenuation = sound.overrideAttenuation;
                        item.overrideDistance = sound.overrideDistance;
                    }
                }


                saveData.Add(item);
            }

            foreach(var p in param)
            {
                if(p[0] == "")
                    continue;

                int group = int.Parse(p[0]);
                int id = int.Parse(p[1]);
                float min = float.Parse(p[3]);
                float max = float.Parse(p[4]);

                var find = saveData.Find(x=> x.id == group);

                if(find == null)
                {
                    Debug.Log("parameter group id not found");
                    Debug.Log("group id : " + group + " name : " + p[2]);
                    break;
                }

                var item = new SoundParameter();
                item.id = id;
                item.name = p[2];
                item.min = min;
                item.max = max;

                find.parameters.Add(item);
            }
        }
        else
        {
            Debug.Log("file error");
        }

        saveData.Sort((x,y)=>{return x.id > y.id ? 1 : (x.id < y.id ? -1 : 0);});

        soundData = new List<SoundInfo>(saveData);
        EditorUtility.SetDirty(this);
    }
#endif

}
