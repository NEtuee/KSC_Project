using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SimpleCache<T> where T : Behaviour
{
	private List<T> _mainList = new List<T>();
	private Queue<T> _cacheQueue = new Queue<T>();
	private System.Action<T> _firstSetting;
	private GameObject _baseObj;
	private Transform _parent = null;
	public void CreateObject(int count)
	{
		for(int i = 0; i < count; ++i)
		{
			T target = GameObject.Instantiate(_baseObj).GetComponent<T>();
			_firstSetting(target);
			// text.init();
			target.gameObject.SetActive(false);
			_cacheQueue.Enqueue(target);
			if(_parent != null)
				target.transform.SetParent(_parent);
		}
	}
	public void Loop(System.Action<int, T> callBack)
	{
		for(int i = 0; i < _mainList.Count;)
		{
			callBack(i,_mainList[i]);
			if(!_mainList[i].gameObject.activeSelf)
			{
				_cacheQueue.Enqueue(_mainList[i]);
				_mainList.RemoveAt(i);
			}
			else
				++i;
		}
	}
	public T ActiveObject(out int count)
	{
		if(_cacheQueue.Count == 0)
			CreateObject(1);
		T target = _cacheQueue.Dequeue();
        count = _mainList.Count;
		_mainList.Add(target);
		return target;
	}
	public void SetParent(Transform tp) {_parent = tp;}
	public void DisableAllObject()
	{
		for(int i = 0; i < _mainList.Count; ++i)
		{
			_mainList[i].gameObject.SetActive(false);
			_cacheQueue.Enqueue(_mainList[i]);
		}
		_mainList.Clear();
	}
	public SimpleCache(GameObject obj,System.Action<T> firstSetting)
	{
		_baseObj = obj;
		_firstSetting = firstSetting;
	}
}