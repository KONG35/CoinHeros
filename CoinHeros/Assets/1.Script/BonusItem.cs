using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BonusEnum
{
    Coin3=0,  // gold coin 3개
    Coin6,  // gold coin 6개
    Coin9,   // gold coin 9개
    EarthQuake, // 지진
    WaterSpout, // 물기둥
    Tornado, // 토네이도
    CoinTower, // 코인탑
    BonusCapsule, // 보너스 캡슐
    Artifact, // 유물
    Conut
}
[Serializable]
public class BonusInfo
{
    public BonusEnum bonus;
    public Sprite sprite;
}
public class BonusItem : MonoBehaviour
{
    [SerializeField]
    private Image iconImg;
    private int index;
    public BonusEnum bonus { get; private set; }
    public RectTransform recTr { get; private set; }
    private void Awake()
    {
        recTr = gameObject.GetComponent<RectTransform>();
    }
    public void SetIndex(int num)
    {
        index = num;
    }
    public void SetBonus(BonusUIDataSO _info)
    {
        bonus = _info.bonus;
        iconImg.sprite = _info.sprite;
    }
}
