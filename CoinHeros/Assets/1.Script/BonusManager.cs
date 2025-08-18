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
    [SerializeField] private BonusAction bonusCapsuleAction;
    [SerializeField] private BonusAction artifactAction;
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
            case BonusEnum.CoinTower:
            {
                coinTowerAction.Show();
            }
            break;
            case BonusEnum.BonusCapsule:
                {
                    bonusCapsuleAction.Show();
                }
                break;
            case BonusEnum.Artifact:
                {
                    artifactAction.Show();
                }
                break;
        }
    }
    
}
