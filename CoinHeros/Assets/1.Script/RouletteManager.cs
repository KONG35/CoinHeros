using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouletteManager : Singleton<RouletteManager>
{
    public BonusRoulette bonusRoul;
    private List<IEnumerator> corList;
    public int remainBonusCnt { get; private set; }
    public bool isStop { get; private set; }
    private BattleManager battleManager;
    protected override void Awake()
    {
        isDone = false;
        base.Awake();
        isStop = false;
        corList = new List<IEnumerator>();
    }
    private void Start()
    {
        battleManager = BattleManager.Instance;
        StartCoroutine(Loop());
    }
    private IEnumerator Loop()
    {
        while (true)
        {
            if (battleManager == null)
                battleManager = BattleManager.Instance;

            if (corList != null && corList.Count > 0 && !isStop && battleManager.IsUpdate)
            {
                remainBonusCnt--;
                yield return StartCoroutine(corList[0]);
                corList.RemoveAt(0);
            }
            yield return null;
        }
    }
    public void InputCoin(CoinEnum _cEnum)
    {
        corList.Add(InputCor(_cEnum));
        remainBonusCnt++;
        bonusRoul.SetRemainCnt();
    }
    IEnumerator InputCor(CoinEnum _cEnum)
    {
        switch (_cEnum)
        {
            default:
                {
                    yield return StartCoroutine(bonusRoul.SpinCor(_cEnum));
                }
                break;
        }
        yield return null;
    }
    public void SetIsStop(bool _isstop)
    {
        isStop = _isstop;
    }
    private void OnDestroy()
    {
        StopCoroutine("Loop");
    }
}
