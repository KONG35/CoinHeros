using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : Singleton<BattleManager>
{

    public int CurStage = 1;

    public BattleUnitPos UnitPositions;





    public CharacterBase test_SetUnit;
    public BattleUnitPos.eBattleUnitPos Pos;
    [Button]
    public void SetUnit()
    {

        var unit = Instantiate(test_SetUnit);

        var character = unit as CharacterData;
        var monster = unit as MonsterData;
        if (character)
        {
            UnitPositions.DisposUnit(Pos, unit);
        }

        if(monster)
        {
            UnitPositions.DisposMonster(Pos, unit);
        }
        TargetCharacter = unit;

    }

    public CharacterBase TargetCharacter;
    public float Speed =1f;
    public CharacterBase.eAnimState anim;
    [Button]
    public void MoveUnit()
    {
        var character = TargetCharacter as CharacterData;
        var monster = TargetCharacter as MonsterData;
        if (character)
        {
            TargetCharacter.toMove(UnitPositions.Left[(int)Pos].transform.position, Speed);
        }

        if (monster)
        {
            TargetCharacter.toMove(UnitPositions.Right[(int)Pos].transform.position, Speed);
        }
    }
    [Button]
    public void PlayAnim()
    {
        TargetCharacter.PlayAnim(anim);
    }
}
