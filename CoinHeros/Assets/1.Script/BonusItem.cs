using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Bonus
{
    Coin3,  // ÄÚÀÎ 3°³
    Coin6,
    Coin9
}
public class BonusItem : MonoBehaviour
{
    [SerializeField]
    private Image iconImg;
    private int index;
    private Bonus bonus;
    public RectTransform recTr { get; private set; }
    private void Awake()
    {
        recTr = gameObject.GetComponent<RectTransform>();
    }
    public void SetIndex(int num)
    {
        index = num;
    }
    public void SetBonus(Bonus _bonus)
    {
        bonus = _bonus;
    }
    public int Selected()
    {
        return index;
    }
}
