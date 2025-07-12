using System.Collections;
using System.Collections.Generic;
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
