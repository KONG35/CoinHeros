using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using System.Linq;
using UniRx.Triggers;
public class CoinTowerBA : BonusAction
{
    [SerializeField] private Transform[] coinTowerTr;
    private const int TotalCnt = 126;
    private const int TotalLayer = 18;
    private const int PerLayer = 7;
    private const int CopperLayer = 8;
    private const int SilverLayer = 14;
    private const int GoldLayer = 17;
    private const int DiamondLayer = 18;
    private Coin[] coinArr;
    // Start is called before the first frame update
    void Start()
    {
        // 18층*7 = 126

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [Button]
    public override void Show()
    {
        // copper, silver, gold, dia 순으로 쌓임.
        // 8층 copper, 6층 silver, 3층 gold, 1층 diamond
        // 떨어져 있는 Coin 만 띄움
        // 아래층 갯수가 부족하다면 다음 단계의 코인을 사용
        // 코인탑을 형성하는 갯수가 부족하다면 그대로 타워 완료.

        // 0. 룰렛 멈추기
        // 1. 현재 떨어져 있는 코인 띄우기
        // 2. 코인탑 형성하는 포지션에 넣기
        // 3. 코인탑 완성되면 떨어뜨리기
        
        StartCoroutine(ShowCor());
    }
    IEnumerator ShowCor()
    {
        RouletteManager.Instance.SetIsStop(true);

        coinArr = ObjectManager.Instance.GetComponentsInChildren<Coin>().Where(x => x.IsResetRigidbody()).ToArray();

        float delay = 2.5f;
        foreach (var c in coinArr)
        {
            c.StartFly(delay);
        }
        yield return new WaitForSeconds(delay);
        
        yield return StartCoroutine(PlacedCor());

        foreach (var c in coinArr)
        {
            c.FinishFly(false);
        }
        RouletteManager.Instance.SetIsStop(false);

        yield return null;
    }
    IEnumerator PlacedCor()
    {
        var copperArr = coinArr.Where(x => x.CoinEnum == CoinEnum.Copper).ToArray();
        var silverArr = coinArr.Where(x => x.CoinEnum == CoinEnum.Silver).ToArray();
        var goldArr = coinArr.Where(x => x.CoinEnum == CoinEnum.Gold).ToArray();
        var diaArr = coinArr.Where(x => x.CoinEnum == CoinEnum.Diamond).ToArray();

        int token = 0;
        int count = 0;
        Coin moveCoin = null;
        for (int i = 0; i < TotalCnt; i++)
        {
            moveCoin = null;
            Vector3 movePos = Vector3.zero;
            switch (token)
            {
                case 0:
                    PlaceCoins(copperArr, PerLayer * CopperLayer, ref i, ref count, ref token, ref moveCoin);
                    break;
                case 1:
                    PlaceCoins(silverArr, PerLayer * SilverLayer, ref i, ref count, ref token, ref moveCoin);
                    break;
                case 2:
                    PlaceCoins(goldArr, PerLayer * GoldLayer, ref i, ref count, ref token, ref moveCoin);
                    break;
                case 3:
                    PlaceCoins(diaArr, PerLayer * DiamondLayer, ref i, ref count, ref token, ref moveCoin);
                    break;
                default:
                    break;
            }
            if(moveCoin != null)
            {
                moveCoin.FinishFly(true);
                StartCoroutine(moveCoin.MoveCor(0.5f, coinTowerTr[i].position));
            }
            yield return new WaitForFixedUpdate();
        }
    }
    void PlaceCoins(Coin[] arr, int limit, ref int i, ref int counter, ref int token, ref Coin moveCoin)
    {
        if (i < limit && counter < arr.Length)
        {
            moveCoin = arr[counter];
            counter++;
        }
        else
        {
            counter = 0;
            i--;
            token++;
        }
    }
}
