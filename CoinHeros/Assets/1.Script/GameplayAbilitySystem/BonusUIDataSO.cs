using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "UI/BonusUI")]
public class BonusUIDataSO : ScriptableObject
{
    public BonusEnum bonus;
    public CoinEnum appearCoinEnum;
    public Sprite sprite;
    public float basicPercent;

}
