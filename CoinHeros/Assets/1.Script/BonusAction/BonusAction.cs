using System.Collections;
using System.Collections.Generic;
using UnityEngine;
interface IBonusAction
{
    void Show();
}
public abstract class BonusAction : MonoBehaviour, IBonusAction
{
    public abstract void Show();
}
