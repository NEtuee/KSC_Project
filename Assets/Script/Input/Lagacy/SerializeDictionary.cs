using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializeDictionary<K,V> : Dictionary<K,V>, ISerializationCallbackReceiver
{
    [SerializeField]
    List<K> keys = new List<K>();

    [SerializeField]
    List<V> values = new List<V>();

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach(KeyValuePair<K,V> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        this.Clear();

        for(int i = 0, count = keys.Count; i< count; ++i)
        {
            this.Add(keys[i], values[i]);
        }
    }
}
