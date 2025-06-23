using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : Singleton<ObjectManager>
{
    [SerializeField]
    private PoolDataSO[] poolDataArray;
    private Dictionary<string, object> pools = new Dictionary<string, object>();

    public void CreatePool<T>(string key, T prefab, int size = 10) where T : MonoBehaviour, IPoolable
    {
        if (!pools.ContainsKey(key))
        {
            var pool = new GenericObjectPool<T>(prefab, size, this.transform);
            pools.Add(key, pool);
        }
    }

    public T Get<T>(string key) where T : MonoBehaviour, IPoolable
    {
        if (pools.TryGetValue(key, out object obj) && obj is GenericObjectPool<T> pool)
        {
            return pool.Get();
        }

        Debug.LogError($"Pool for {key} not found");
        return null;
    }

    public void Return<T>(string key, T item) where T : MonoBehaviour, IPoolable
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
            if (string.IsNullOrEmpty(data.poolKey) || data.prefab == null)
            {
                Debug.LogWarning($"풀 데이터 누락: {data.name}");
                continue;
            }

            if (!pools.ContainsKey(data.poolKey))
            {
                var mono = data.prefab.GetComponent<IPoolable>();
                if (mono == null)
                {
                    Debug.LogError($"Prefab '{data.prefab.name}'은 IPoolable을 구현하지 않음");
                    continue;
                }
                var componentType = data.componentToPool.GetType();
                if (componentType == null)
                {
                    Debug.LogError($"'{data.componentToPool.GetType()}' 타입을 찾을 수 없음");
                    continue;
                }
                var createPoolMethod = typeof(ObjectManager).GetMethod(nameof(CreatePool)).MakeGenericMethod(componentType);

                var component = data.prefab.GetComponent(componentType);
                
                createPoolMethod.Invoke(this, new object[] { data.poolKey, component, data.initSize });
            }
        }
    }
}
