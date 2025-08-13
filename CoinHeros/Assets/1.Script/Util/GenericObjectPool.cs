using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IPoolable
{
    void OnSpawn(); 
    void OnDespawn();
}

public class GenericObjectPool<T> where T : MonoBehaviour, IPoolable
{
    private Queue<T> pool = new Queue<T>();
    private T prefab;
    private Transform parent;

    public GenericObjectPool(T prefab, int initSize, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;

        for(int i=0;i<initSize;i++)
        {
            T obj = GameObject.Instantiate(prefab, parent);
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }

    public T Get()
    {
        T obj;
        if (pool.Count > 0)
        {
            obj = pool.Dequeue();
        }
        else
        {
            obj = GameObject.Instantiate(prefab, parent);
        }

        obj.gameObject.SetActive(true);
        obj.OnSpawn();
        return obj;
    }
    public void Return(T obj)
    {
        obj.OnDespawn();
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
}
