using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Pooling/PoolData")]
public class PoolDataSO : ScriptableObject
{
    public GameObject prefab;
    public int initSize;

}
