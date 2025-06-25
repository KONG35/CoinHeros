using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouletteManager : Singleton<RouletteManager>
{
    [SerializeField]
    private BonusRoulette bonusRoul;
    private List<IEnumerator> corList;
    public int remainBonusCnt { get; private set; }
    protected override void Awake()
    {
        base.Awake();
        corList = new List<IEnumerator>();
    }
    private void Start()
    {
        StartCoroutine(Loop());
    }
    private IEnumerator Loop()
    {
        while (true)
        {
            if (corList != null && corList.Count > 0)
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
    private void OnDestroy()
    {
        StopCoroutine("Loop");
    }
}
