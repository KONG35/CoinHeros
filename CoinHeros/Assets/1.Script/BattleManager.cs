using NaughtyAttributes;
using System;
using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

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


    public int testStagenumber;
    [Button]
    public void CreateMonster()
    {
        var Unit = CreateMonster(testStagenumber);

        for(int i=0;i<UnitPositions.Right.Count;i++)
        {
            if(UnitPositions.RightSlot[i] == null)
            {
                UnitPositions.RightSlot[i]= Unit;
                Unit.transform.parent = UnitPositions.Right[i];
                Unit.transform.position = UnitPositions.RightSpawnPoint.position;
                Unit.transform.rotation = UnityEngine.Quaternion.identity;
                
                // 스폰 후 위치 확인
                Debug.Log($"Monster spawned and positioned at: {Unit.transform.position}");
                Debug.Log($"Monster parent: {Unit.transform.parent.name}");
                
                // 이동 명령을 큐에 추가하고 디버그 로그 출력
                Vector3 targetPos = Unit.transform.parent.position;
                Debug.Log($"Monster spawned at: {UnitPositions.RightSpawnPoint.position}");
                Debug.Log($"Monster target position: {targetPos}");
                Debug.Log($"Distance to target: {Vector3.Distance(Unit.transform.position, targetPos)}");
                
                // 더 긴 지연 후 이동 명령 실행 (스폰 애니메이션 등을 위한 시간)
                StartCoroutine(DelayedMove(Unit, targetPos, 2.0f));
                break;
            }
        }
    }
    public MonsterData CreateMonster(int Stage)
    {
        var data = DataTableManager.Instance;
        float value = nomalizeFloat(data.minMonsterState, data.maxMonsterState, 1, data.MaxStage, CurStage);

        float r1 = Random.Range(0, value + 1);
        float r2 = Random.Range(0, value + 1);
        float r3 = Random.Range(0, value + 1);
        float r4 = Random.Range(0, value + 1);
        float r5 = Random.Range(0, value + 1);


        float[] cuts = new float[] { r1, r2 ,r3,r4,r5};
        Array.Sort(cuts);

        float a = cuts[0];
        float b = cuts[1] - cuts[0];
        float c = cuts[2] - cuts[1];
        float d = cuts[3] - cuts[2];
        float e = cuts[4] - cuts[3];
        float f = value - cuts[4];

        int[] values = new int[] { (int)a, (int)b, (int)c,(int)d, (int)e, (int)f };
        Shuffle(values);

        int str = values[0];
        int agi = values[1];
        int con = values[2];
        int luk = values[3];
        int mag = values[4];
        int spr = values[5];

        var DTM = DataTableManager.Instance;
        var MonsterList = DTM.MonsterPrefabList;
        int index = Random.Range(0, MonsterList.Count);
        MonsterData Unit = Instantiate(MonsterList[index], UserData.Instance.transform);


        int Grade = 0;

        Unit.SetBaseState(GASAttributeData.Instance.STR, value);
        Unit.SetBaseState(GASAttributeData.Instance.Grade_STR, Grade);
        Unit.SetBaseState(GASAttributeData.Instance.MAG, value);
        Unit.SetBaseState(GASAttributeData.Instance.Grade_MAG, Grade);
        Unit.SetBaseState(GASAttributeData.Instance.CON, value);
        Unit.SetBaseState(GASAttributeData.Instance.Grade_CON, Grade);
        Unit.SetBaseState(GASAttributeData.Instance.AGI, value);
        Unit.SetBaseState(GASAttributeData.Instance.Grade_AGI, Grade);
        Unit.SetBaseState(GASAttributeData.Instance.SPR, value);
        Unit.SetBaseState(GASAttributeData.Instance.Grade_SPR, Grade);
        Unit.SetBaseState(GASAttributeData.Instance.LUK, value);
        Unit.SetBaseState(GASAttributeData.Instance.Grade_LUK, Grade);
        Unit.SetCalcBaseStateToDetailState();


        return Unit;
    }


    public void MonsterAction()
    {

    }

    public void CharacterAction(int CoinIndex)
    {

    }

    public float nomalizeFloat(float ValueMin,float ValueMax,float RangeMin, float RangeMax ,float Range)
    {
        return (RangeMin + (Range - ValueMin) * (RangeMax - RangeMin) / (ValueMax - ValueMin));
    }



    void Shuffle(int[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
    }

    private IEnumerator DelayedMove(CharacterBase unit, Vector3 targetPos, float speed)
    {
        // 스폰 애니메이션 등을 위한 충분한 시간 대기
        yield return new WaitForSeconds(11.0f);
        
        // 스폰 포인트와 목표가 다른 경우에만 이동
        if (Vector3.Distance(unit.transform.position, targetPos) > 0.1f)
        {
            unit.toMove(targetPos, speed);
            Debug.Log($"Monster move command executed: {unit.name} -> {targetPos}");
        }
        else
        {
            Debug.Log($"Monster already at target position: {unit.name}");
        }
    }

}
