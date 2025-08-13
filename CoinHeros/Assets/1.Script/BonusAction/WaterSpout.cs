using NaughtyAttributes;
using UnityEngine;

public class WaterSpout : BonusAction
{
    [SerializeField] private WaterSpoutParticle waterSpoutParticle;
    [SerializeField] private Collider boundCol;
    [Button]
    public override void Show()
    {
        // particle 갯수, 위치 set
        // 일단 1개
        // int n = Random.Range(0, initTr.Length);
        // Vector3 basePos = boundCol.gameObject.transform.position;
        float x = Random.Range(boundCol.bounds.min.x, boundCol.bounds.max.x);
        float z = Random.Range(boundCol.bounds.min.z, boundCol.bounds.max.z);
        Vector3 pos = new Vector3(x, boundCol.bounds.center.y, z);        
        var w = ObjectManager.Instance.Get<WaterSpoutParticle>(waterSpoutParticle.PoolData);
        
        
        
        w.transform.position = pos;
        w.Play();
    }
}
