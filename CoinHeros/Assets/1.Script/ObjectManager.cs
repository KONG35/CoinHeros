using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    void Start()
    {
        //InitializePools(poolDataArray);
    }
    public void InitializePools(PoolDataSO[] poolDataList)
    {
        foreach (var data in poolDataList)
        {
            if (data.prefab == null)
            {
                Debug.LogError("Prefab is null");
                continue;
            }
            var poolables = data.prefab.GetComponents<MonoBehaviour>()
           .Where(m => m is IPoolable)
           .ToArray();

            if (poolables.Length == 0)
            {
                Debug.LogError($"Prefab '{data.prefab.name}'은 IPoolable을 구현한 컴포넌트를 포함하지 않음");
                continue;
            }

            if (poolables.Length > 1)
            {
                Debug.LogWarning($"Prefab '{data.prefab.name}'에 IPoolable 구현체가 여러 개 있음. 첫 번째만 사용");
            }

            var poolable = poolables[0];
            var type = poolable.GetType();

            var createPoolMethod = typeof(ObjectManager)
                .GetMethod(nameof(CreatePool))
                .MakeGenericMethod(type);

            createPoolMethod.Invoke(this, new object[] { data, poolable });
        }
    }
}
