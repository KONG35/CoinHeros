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

        Debug.LogError($"'{key}'에 대한 풀을 찾을 수 없습니다.");
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
            Debug.LogError($"'{key}'에 대한 풀을 찾을 수 없습니다.");
        }
    }

    protected override void Awake()
    {
        base.Awake();

        InitializePools(poolDataArray);
    }
    public void InitializePools(PoolDataSO[] poolDataList)
    {
        foreach (var data in poolDataList)
        {
            if (data==null||data.prefab == null)
            {
                Debug.LogError("프리팹이 null입니다.");
                continue;
            }
            var poolables = data.prefab.GetComponents<MonoBehaviour>()
           .Where(m => m is IPoolable)
           .ToArray();

            if (poolables.Length == 0)
            {
                Debug.LogError($"Prefab '{data.prefab.name}'에 IPoolable을 구현한 컴포넌트가 존재하지 않습니다.");
                continue;
            }

            if (poolables.Length > 1)
            {
                Debug.LogWarning($"Prefab '{data.prefab.name}'에 IPoolable 구현체가 여러 개 있습니다. 첫 번째만 사용합니다.");
            }

            var poolable = poolables[0];
            var type = poolable.GetType();

            var createPoolMethod = typeof(ObjectManager)
                .GetMethod(nameof(CreatePool))
                .MakeGenericMethod(type);

            createPoolMethod.Invoke(this, new object[] { data, poolable });
        }
    }
    public void AllReturn()
    {
        Debug.Log("모든 풀의 오브젝트들을 반환합니다.");
        
        foreach (var kvp in pools)
        {
            var pool = kvp.Value;
            
            // GenericObjectPool의 ReturnAll 메서드 호출
            var poolType = pool.GetType();
            var returnAllMethod = poolType.GetMethod("ReturnAll");
            
            if (returnAllMethod != null)
            {
                returnAllMethod.Invoke(pool, null);
            }
        }
        
        Debug.Log("모든 풀의 오브젝트 반환 완료");
    }
}
