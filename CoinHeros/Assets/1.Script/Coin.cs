using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CoinEnum
{
    Copper=0,
    Silver,
    Gold,
    Diamond,
    Count
}
public class Coin : MonoBehaviour, IPoolable
{
    [SerializeField]
    private PoolDataSO poolDataSO;
    public PoolDataSO PoolData => poolDataSO;

    [SerializeField]
    private CoinEnum coinEnum;

    private Rigidbody rigid;
    private void Awake()
    {
        rigid = gameObject.GetComponent<Rigidbody>();
    }
    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Basket")
        {
            rigid.constraints = RigidbodyConstraints.None;
        }
    }
    private void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag=="Slider")
        {
            rigid.constraints = RigidbodyConstraints.None;
        }
        else if(col.gameObject.tag=="Spin")
        {
            RouletteManager.Instance.InputCoin(coinEnum);
            CoinSpawnManager.Instance.ReturnCoin(PoolData, this);
        }
        else if (col.gameObject.tag == "Outside")
        {
            BattleManager.Instance.CharacterAction((int)coinEnum);
            CoinSpawnManager.Instance.ReturnCoin(PoolData, this);
        }
    }
    public void OnSpawn()
    {
        rigid.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        rigid.velocity = new Vector3(0, -35f, 0);
    }

    public void OnDespawn()
    {
    }
    public void ResetRigidbody()
    {
        rigid.constraints = RigidbodyConstraints.None;
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
    }
    public void SetVelocity(Vector3 vec)
    {
        rigid.velocity = vec;
    }
}
