using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;

public class BonusManager : Singleton<BonusManager>
{
    [SerializeField] private BonusAction earthquakeAction;
    [SerializeField] private BonusAction waterSpoutAction;
    [SerializeField] private BonusAction coinTowerAction;
    protected override void Awake()
    {
        isDone = false;
        base.Awake();
    }
    public void Show(BonusEnum bEnum)
    {
        switch(bEnum)
        {
            case BonusEnum.EarthQuake:
            {
                earthquakeAction.Show();
            }
            break;
            case BonusEnum.WaterSpout:
            {
                waterSpoutAction.Show();
            }
            break;
        }
    }
    
}
