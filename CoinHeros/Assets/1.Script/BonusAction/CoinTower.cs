using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using System.Linq;
public class CoinTower : BonusAction
{
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
        RouletteManager.Instance.SetIsStop(true);

        var coinArr = ObjectManager.Instance.GetComponentsInChildren<Coin>().OrderBy(x=> x.IsResetRigidbody());
        foreach(var c in coinArr)
        {
            c.StartFly();
        }
    }
}
