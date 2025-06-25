using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Bonus
{
    Coin3=0,  // 코인 3개
    Coin6,  // 코인 6개
    Coin9   // 코인 9개
}
[Serializable]
public class BonusInfo
{
    public Bonus bonus;
    public Sprite sprite;
}
public class BonusItem : MonoBehaviour
{
    [SerializeField]
    private Image iconImg;
    private int index;
    public Bonus bonus { get; private set; }
    public RectTransform recTr { get; private set; }
    private void Awake()
    {
        recTr = gameObject.GetComponent<RectTransform>();
    }
    public void SetIndex(int num)
    {
        index = num;
    }
    public void SetBonus(BonusInfo _info)
    {
        bonus = _info.bonus;
        iconImg.sprite = _info.sprite;
    }
    public int Selected()
    {
        return index;
    }
}
