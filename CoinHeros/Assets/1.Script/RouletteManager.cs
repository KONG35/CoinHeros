using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RouletteManager : Singleton<RouletteManager>
{
    [SerializeField]
    private BonusRoulette bonusRoul;

    public void InputCoin(CoinEnum _cEnum = CoinEnum.Copper)
    {
        switch(_cEnum)
        {
            default:
                {
                    bonusRoul.Spin();
                }
                break;
        }
    }
}
