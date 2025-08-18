using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusCapsuleBA : BonusAction
{
    [SerializeField] private CapsuleObject capsuleObj;
    [SerializeField] private Collider boundCol;
    [Button]
    public override void Show()
    {
        // 우선 한개
        float x = Random.Range(boundCol.bounds.min.x, boundCol.bounds.max.x);
        float z = Random.Range(boundCol.bounds.min.z, boundCol.bounds.max.z);
        Vector3 pos = new Vector3(x, boundCol.bounds.center.y, z);
        pos += Vector3.up * 30f;
        var w = ObjectManager.Instance.Get<CapsuleObject>(capsuleObj.PoolData);

        w.transform.position = pos;
    }
}
