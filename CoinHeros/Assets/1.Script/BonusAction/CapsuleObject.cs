using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleObject : MonoBehaviour, IPoolable
{
    [SerializeField] private PoolDataSO poolData;
    public PoolDataSO PoolData => poolData;
    public void OnDespawn()
    {
    }

    public void OnSpawn()
    {
    }
}
