using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    #region Inspector

    public GameObject prefab;

    #endregion

    private List<GameObject> _pool = null;

    private void Awake()
    {
        prefab.SetActive(false);
        Create();
    }

    public void Create()
    {
        int makeCnt = 0;
        _pool ??= new List<GameObject>();

        int addCnt = makeCnt >= _pool.Count ? makeCnt - _pool.Count : 0;

        for (int i = 0; i < addCnt; ++i) // 늘어난 갯수만큼 생성
        {
            _pool.Add(CreateObject());
        }
    }

    public void HideAll()
    {
        if (_pool != null)
        {
            for (int i = 0; i < _pool.Count; ++i)
            {
                GameObject go = _pool[i];
                if (go)
                {
                    go.transform.SetParent(this.transform);
                    go.SetActive(false);
                }
                else
                {
                    Debug.LogError("ObjectPool Error.");
                }
            }
        }
    }

    private GameObject CreateObject()
    {
        var go = Instantiate(prefab);
        go.transform.SetParent(this.transform, false);
        go.transform.localPosition = Vector3.zero;
        go.transform.localScale = Vector3.one;
        go.SetActive(false);
        return go;
    }

    public GameObject Get()
    {
        int index = GetIndex();
        return _pool[index];
    }

    public T Get<T>() where T : Component
    {
        int index = GetIndex();

        return _pool[index].GetComponent<T>();
    }

    private int GetIndex()
    {
        int index = 0;
        bool isFind = false;

        for (int i = 0; i < _pool.Count; ++i)
        {
            if (_pool[i] != null && !_pool[i].activeSelf)
            {
                index = i;
                isFind = true;
                break;
            }
        }

        if (!isFind)
        {
            _pool.Add(CreateObject());
            index = _pool.Count - 1;
        }

        return index;
    }

    public GameObject GetItem(int index)
    {
        if (_pool != null)
        {
            return _pool[index];
        }

        return null;
    }
}