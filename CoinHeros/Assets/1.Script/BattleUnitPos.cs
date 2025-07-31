using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BattleUnitPos : MonoBehaviour
{
    public List<Transform> Left;
    public CharacterBase[] LeftSlot = new CharacterBase[6];
    public List<Transform> Right;
    public CharacterBase[] RightSlot = new CharacterBase[6];
    public Transform RightSpawnPoint;
    public void DisposUnit(eBattleUnitPos Slot,CharacterBase unit)
    {
        int iSlot = (int)Slot;
        LeftSlot[iSlot] = unit;
        unit.transform.parent = Left[iSlot];
        unit.transform.position = Vector3.zero;
        unit.transform.localRotation = Quaternion.identity;
        unit.transform.localScale = Vector3.one;
    }

    public void DisposMonster(eBattleUnitPos Slot, CharacterBase monster)
    {
        int iSlot = (int)Slot;
        RightSlot[iSlot] = monster;
        monster.transform.parent = Right[iSlot];
        monster.transform.position = Vector3.zero;
        monster.transform.localRotation = Quaternion.identity;
        monster.transform.localScale = Vector3.one;
    }



    public enum eBattleUnitPos
    {
        FRONT_TOP,
        FRONT_MID,
        FRONT_BOT,
        BACK_TOP,
        BACK_MID,
        BACK_BOT,
        COUNT
    }
}
