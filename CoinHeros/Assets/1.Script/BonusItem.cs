using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BonusEnum
{
    Coin3=0,  // ���� 3��
    Coin6,  // ���� 6��
    Coin9,   // ���� 9��
    EarthQuake, // ����
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
