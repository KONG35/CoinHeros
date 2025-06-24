using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : Singleton<ObjectManager>
{
    [SerializeField]
    private PoolDataSO[] poolDataArray;
    private Dictionary<PoolDataSO, object> pools = new Dictionary<PoolDataSO, object>();

    public void CreatePool<T>(PoolDataSO key, T prefab) where T : MonoBehaviour, IPoolable
    {
        if (!pools.ContainsKey(key))
        {
            var pool = new GenericObjectPool<T>(prefab, key.initSize, this.transform);
            //var pool = new GenericObjectPool<T>(key.prefab as T, key.initSize, this.transform);
            pools.Add(key, pool);
        }
    }

    public T Get<T>(PoolDataSO key) where T : MonoBehaviour, IPoolable
    {
        if (pools.TryGetValue(key, out object obj) && obj is GenericObjectPool<T> pool)
        {
            return pool.Get();
        }

        Debug.LogError($"Pool for {key} not found");
        return null;
    }

    public void Return<T>(PoolDataSO key, T item) where T : MonoBehaviour, IPoolable
    {
        if (pools.TryGetValue(key, out object obj) && obj is GenericObjectPool<T> pool)
        {
            pool.Return(item);
        }
        else
        {
            Debug.LogError($"Pool for {key} not found");
        }
    }

    override protected void Awake()
    {
        InitializePools(poolDataArray);
    }
    public void InitializePools(PoolDataSO[] poolDataList)
    {
        foreach (var data in poolDataList)
        {
            var mono = data.prefab.GetComponent<IPoolable>();
            if (mono == null)
            {
                Debug.LogError($"Prefab '{data.prefab.name}'은 IPoolable을 구현하지 않음");
                continue;
            }
            //var componentType = data.componentToPool.GetType();
            //if (componentType == null)
            //{
            //    Debug.LogError($"'{data.componentToPool.GetType()}' 타입을 찾을 수 없음");
            //    continue;
            //}

            //var component = data.prefab.GetComponent(componentType);
            //var createPoolMethod = typeof(ObjectManager).GetMethod(nameof(CreatePool)).MakeGenericMethod(componentType);

            //createPoolMethod.Invoke(this, new object[] { data , component });
            var componentType = Type.GetType(data.componentTypeName);
            if (componentType == null)
            {
                Debug.LogError($"'{data.componentTypeName}' 타입을 찾을 수 없음");
                continue;
            }

            var createPoolMethod = typeof(ObjectManager).GetMethod(nameof(CreatePool)).MakeGenericMethod(componentType);
            var component = data.prefab.GetComponent(componentType);
            createPoolMethod.Invoke(this, new object[] { data, component });
        }
    }
}
