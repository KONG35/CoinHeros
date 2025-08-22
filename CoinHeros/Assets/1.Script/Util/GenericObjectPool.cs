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
    private HashSet<T> inUseObjects = new HashSet<T>();
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
        inUseObjects.Add(obj);
        return obj;
    }
    public void Return(T obj)
    {
        if (obj == null)
        {
            Debug.LogWarning("반환하려는 오브젝트가 null입니다. 무시합니다.");
            return;
        }

        if (!inUseObjects.Contains(obj))
        {
            if(pool.Contains(obj))
            {
                Debug.LogWarning("중복 반환 객체입니다.");
                return;
            }
            else
            {
                obj.OnDespawn();
                obj.gameObject.SetActive(false);
                pool.Enqueue(obj);
                Debug.LogWarning("사용 중이 아닌 오브젝트가 반환되었습니다. 생성되지 않은 객체일 수 있습니다.");
                return;
            }
        }

        inUseObjects.Remove(obj);
        obj.OnDespawn();
        obj.gameObject.SetActive(false);
        pool.Enqueue(obj);
    }
    
    public void ReturnAll()
    {
        if (inUseObjects.Count == 0)
        {
            return;
        }
        
        // 사용 중인 오브젝트들을 복사해서 반환 (컬렉션을 순회하면서 수정하면 안됨)
        var objectsToReturn = new List<T>(inUseObjects);
        foreach (var obj in objectsToReturn)
        {
            if (obj != null)
            {
                Return(obj);
            }
        }
    }
    
}
