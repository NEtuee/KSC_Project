using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
        public int id;
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

    public void CreateInfoFromCSV()
    {
        CreateInfoFromCSV(csvData);
    }

    public void CreateInfoFromCSV(TextAsset csv)
    {
        soundData = new List<SoundInfo>();
        IOManager.ReadRangeFromCSV(csv.text,1,-1,0,2,out var data);
        IOManager.ReadRangeFromCSV(csv.text,1,-1,4,8,out var param);
        
        if(data != null && param != null)
        {
            foreach(var d in data)
            {
                if(d[0] == "")
                    continue;

                var item = new SoundInfo();
                item.id = int.Parse(d[0]);
                item.path = d[1];
                item.type = d[2];

                soundData.Add(item);
            }

            foreach(var p in param)
            {
                if(p[0] == "")
                    continue;

                int group = int.Parse(p[0]);
                int id = int.Parse(p[1]);
                float min = float.Parse(p[3]);
                float max = float.Parse(p[4]);

                var find = soundData.Find(x=> x.id == group);

                if(find == null)
                {
                    Debug.Log("parameter group id not found");
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
    }
}
