using System.Collections;
using System.Collections.Generic;
using SharpUI.Source.Common.UI.Elements;
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
    [Header("StartCoin ����")]
    [SerializeField]
    private Transform[] startCoinGroupTr;

    [SerializeField]
    private CoinLaunchMachine coinLaunchMachine;
    public int maxCoinCount{get; private set;}
    public int remainCoinCount{get {return remainCoinList.Count;}}
    private List<CoinEnum> remainCoinList;
    private bool isPaused;  //!! 추후에 다같이 관리할거임
    override protected void Awake()
    {
        base.Awake();
        isPaused = false;
        remainCoinList = new List<CoinEnum>();
        maxCoinCount = 8;
        
        ResetCoin();
    }
    
    private void Start()
    {
        // 250724 �� 16*6 + 16*5 + 17*3 = 227
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
            // ���� ���̱�
            //LayerMask mask = LayerMask.GetMask("Slider", "Coin");
            c.gameObject.transform.position = startCoinGroupTr[i].position;
            //CoinMaker.PlacedOn(c.gameObject, c.transform.position, mask);
        }
        
    }
    private void Update()
    {
        
        if(Input.GetKeyDown("1"))
        {
            if(remainCoinList.Count== 0 || isPaused)
                return;

            coinLaunchMachine.InsertCoin(remainCoinList[0]);
            remainCoinList.RemoveAt(0);
            
            if(remainCoinList.Count==0)
            {
                isPaused = true;
                
                BattleManager.Instance.MonsterAction();
                ResetCoin();
            }
        }
        
    }
    /// <summary>
    /// 좌상단 coin pool 디시 set
    /// </summary>
    private void ResetCoin()
    {
        for(int i=0;i<maxCoinCount;i++)
        {
            int n = Random.Range(0,100);
            CoinEnum cEnum = CoinEnum.Copper;
            if(n<40)
            {
                cEnum = CoinEnum.Copper;
            }
            else if(n<80)
            {
                cEnum = CoinEnum.Silver;
            }
            else if(n<95)
            {
                cEnum = CoinEnum.Gold;
            }
            else
            {
                cEnum = CoinEnum.Diamond;
            }   
            remainCoinList.Add(cEnum);
        }
        // 몬스터 공격 끝나고 호출할 것
        // 수정될 부분
        isPaused = false;
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
            Debug.Log($"{_cEnum}�� {coinData}�� �������� ����.");
            
        return coin;
    }
    public void ReturnCoin(PoolDataSO poolData, Coin item)
    {
        ObjectManager.Instance.Return<Coin>(poolData, item);
    }
}
