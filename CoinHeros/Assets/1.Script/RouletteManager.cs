using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouletteManager : Singleton<RouletteManager>
{
    [SerializeField]
    private BonusRoulette bonusRoul;

    public int corCnt;
    private List<IEnumerator> corList;

    protected override void Awake()
    {
        base.Awake();
        corList = new List<IEnumerator>();
        corCnt = corList.Count;
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
                yield return StartCoroutine(corList[0]);
                corList.RemoveAt(0);
                corCnt = corList.Count;
            }
            yield return null;
        }
    }
    public void InputCoin(CoinEnum _cEnum = CoinEnum.Copper)
    {
        corList.Add(InputCor(_cEnum));
        corCnt = corList.Count;
    }
    IEnumerator InputCor(CoinEnum _cEnum)
    {
        switch (_cEnum)
        {
            default:
                {
                    yield return StartCoroutine(bonusRoul.SpinCor());
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
