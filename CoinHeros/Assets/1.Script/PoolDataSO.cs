using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Pooling/PoolData")]
public class PoolDataSO : ScriptableObject
{
    public string poolKey;
    public GameObject prefab;
    public int initSize;

    [Tooltip("이 프리팹에 붙어있는 IPoolable을 구현한 컴포넌트를 직접 할당")]
    public MonoBehaviour componentToPool;
}
