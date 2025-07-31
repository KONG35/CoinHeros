using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CoinSpawnManager :Singleton<CoinSpawnManager>
{
    [SerializeField]
    private Coin copperCoin;
    [SerializeField]
    private Coin silverCoin;
    [SerializeField]
    private Coin goldCoin;
    [SerializeField]
    private Coin diamondCoin;

    [Space(5)]
    [Header("StartCoin 셋팅")]
    [SerializeField]
    private Transform[] startCoinGroupTr;

    private void Start()
    {
        // 미리 생성되어 있는 코인 만들기
        // 사전에 셋팅된 코인 종류, 갯수에 따른 셋팅
        // 250724 총 16*6 + 16*5 + 17*3 = 227
        // copper 50, silver 150, gold 27
        int length = startCoinGroupTr.Length;
        for (int i = 0; i < length; i++)
        {
            int j = Random.Range(0, length);
            Vector3 temp = startCoinGroupTr[i].position;
            startCoinGroupTr[i].position = startCoinGroupTr[j].position;
            startCoinGroupTr[j].position = temp;
        }

        for (int i=0;i< length; i++)
        {
            CoinEnum cEnum = CoinEnum.Copper;
            if (i<27)
            {
                cEnum = CoinEnum.Gold;
            }
            else if(i<177)
            {
                cEnum = CoinEnum.Silver;
            }

            var c = GetCoin(cEnum);
            if (c == null) return;
            c.ResetRigidbody();
            
            c.gameObject.transform.rotation = Quaternion.identity;
            // 땅에 붙이기
            //LayerMask mask = LayerMask.GetMask("Slider", "Coin");
            c.gameObject.transform.position = startCoinGroupTr[i].position;
            //CoinMaker.PlacedOn(c.gameObject, c.transform.position, mask);
        }
        
    }
    public Coin GetCoin(CoinEnum _cEnum)
    {
        PoolDataSO coinData = copperCoin.PoolData;

        switch (_cEnum)
        {
            case CoinEnum.Copper:
                coinData = copperCoin.PoolData;
                break;
            case CoinEnum.Silver:
                coinData = silverCoin.PoolData;
                break;
            case CoinEnum.Gold:
                coinData = goldCoin.PoolData;
                break;
            case CoinEnum.Diamond:
                coinData = diamondCoin.PoolData;
                break;
        }
        Coin coin = ObjectManager.Instance.Get<Coin>(coinData);
        if (coin == null)
            Debug.Log($"{_cEnum}의 {coinData}가 존재하지 않음.");
            
        return coin;
    }
    public void ReturnCoin(PoolDataSO poolData, Coin item)
    {
        ObjectManager.Instance.Return<Coin>(poolData, item);
    }
}
