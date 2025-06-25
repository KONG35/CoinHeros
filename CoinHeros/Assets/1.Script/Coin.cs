using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CoinEnum
{
    Copper,
    Silver,
    Gold,
    Diamond
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
        rigid.velocity = new Vector3(0, -35f, 0);
    }
    public void Init()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {


    }

    void LateUpdate()
    {
        //if(isFixed)
        //{
        //    Vector3 rot = transform.rotation.eulerAngles;
        //    rot.x = fixRotX;
        //    transform.rotation = Quaternion.Euler(rot);
        //}
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
        if(col.gameObject.tag=="Spin")
        {
            RouletteManager.Instance.InputCoin(coinEnum);
            ObjectManager.Instance.Return<Coin>(PoolData, this);
        }
    }

    public void OnSpawn()
    {
    }

    public void OnDespawn()
    {
    }
}
