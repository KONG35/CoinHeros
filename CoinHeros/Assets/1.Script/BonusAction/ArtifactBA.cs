using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactBA : BonusAction
{
    [SerializeField] private ArtifactObject[] artifactObjArr;
    [SerializeField] private Collider boundCol;
    [Button]
    public override void Show()
    {
        // 우선 한개
        int i = Random.Range(0, artifactObjArr.Length);
        GameObject go = Instantiate(artifactObjArr[i].gameObject, ObjectManager.Instance.gameObject.transform);

        float x = Random.Range(boundCol.bounds.min.x, boundCol.bounds.max.x);
        float z = Random.Range(boundCol.bounds.min.z, boundCol.bounds.max.z);
        Vector3 pos = new Vector3(x, boundCol.bounds.center.y, z);
        pos += Vector3.up * 30f;

        go.transform.position = pos;
    }
}
