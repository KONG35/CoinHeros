using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class BattleManager : Singleton<BattleManager>
{

    public int CurStage = 1;
    public bool IsUpdate = false;

    public BattleUnitPos UnitPositions;

    public CharacterBase test_SetUnit;
    public BattleUnitPos.eBattleUnitPos Pos;

    public float StageStartDelayTime =3.0f;

    public void Start()
    {
        StartCoroutine(DelayStageStart(2.0f));
    }

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
        PlaceMonsterByAttackType(Unit);
    }

    private void PlaceMonsterByAttackType(MonsterData monster)
    {
        int targetIndex = -1;
        
        // 어택 타입에 따른 우선 배치 인덱스 결정
        int startIndex, endIndex;
        
        switch (monster.AttackType)
        {
            case MonsterData.MonsterAttackType.Melee:
                startIndex = 0;
                endIndex = 2;
                break;
            case MonsterData.MonsterAttackType.Magic:
                startIndex = 3;
                endIndex = 5;
                break;
            default:
                startIndex = 0;
                endIndex = 5;
                break;
        }
        
        // 우선 배치 구간에서 랜덤하게 빈 자리 찾기
        List<int> availablePositions = new List<int>();
        for (int i = startIndex; i <= endIndex; i++)
        {
            if (UnitPositions.RightSlot[i] == null)
            {
                availablePositions.Add(i);
            }
        }
        
        if (availablePositions.Count > 0)
        {
            targetIndex = availablePositions[Random.Range(0, availablePositions.Count)];
        }
        else
        {
            // 우선 구간에 자리가 없으면 전체에서 랜덤하게 빈 자리 찾기
            availablePositions.Clear();
            for (int i = 0; i < UnitPositions.RightSlot.Length; i++)
            {
                if (UnitPositions.RightSlot[i] == null)
                {
                    availablePositions.Add(i);
                }
            }
            
            if (availablePositions.Count > 0)
            {
                targetIndex = availablePositions[Random.Range(0, availablePositions.Count)];
            }
        }
        
        if (targetIndex != -1)
        {
            UnitPositions.RightSlot[targetIndex] = monster;
            monster.transform.parent = UnitPositions.RightSpawnPoint.transform;
            monster.transform.position = UnitPositions.RightSpawnPoint.position;
            monster.transform.rotation = UnityEngine.Quaternion.Euler(0, 90, 0);
            
            var targetPos = UnitPositions.Right[targetIndex].position;
            StartCoroutine(DelayedMove(monster, targetPos, 4.0f));
            
            Debug.Log($"몬스터 {monster.name} ({monster.AttackType})가 위치 {targetIndex}에 배치되었습니다.");
        }
        else
        {
            Debug.LogWarning("몬스터 배치를 위한 빈 자리가 없습니다!");
            Destroy(monster.gameObject);
        }
    }
    public MonsterData CreateMonster(int Stage,int Grade = 0)
    {
        var data = DataTableManager.Instance;
        float value = nomalizeFloat(1, data.MaxStage, data.minMonsterState, data.maxMonsterState, CurStage);

        float r1 = Random.Range(10, value + 1);
        float r2 = Random.Range(10, value + 1);
        float r3 = Random.Range(10, value + 1);
        float r4 = Random.Range(10, value + 1);
        float r5 = Random.Range(10, value + 1);


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

        switch(Unit.AttackType)
        {
            case MonsterData.MonsterAttackType.Melee:
                str += (int)(mag *0.5f);
                con += (int)(mag * 0.5f);
                mag = 0;
                break;
            case MonsterData.MonsterAttackType.Magic:
                mag += str;
                str = 0;
                break;

            default:
                break;
        }



        Unit.SetBaseState(GASAttributeData.Instance.STR, str);
        Unit.SetBaseState(GASAttributeData.Instance.Grade_STR, Grade);
        Unit.SetBaseState(GASAttributeData.Instance.MAG, mag);
        Unit.SetBaseState(GASAttributeData.Instance.Grade_MAG, Grade);
        Unit.SetBaseState(GASAttributeData.Instance.CON, con);
        Unit.SetBaseState(GASAttributeData.Instance.Grade_CON, Grade);
        Unit.SetBaseState(GASAttributeData.Instance.AGI, agi);
        Unit.SetBaseState(GASAttributeData.Instance.Grade_AGI, Grade);
        Unit.SetBaseState(GASAttributeData.Instance.SPR, spr);
        Unit.SetBaseState(GASAttributeData.Instance.Grade_SPR, Grade);
        Unit.SetBaseState(GASAttributeData.Instance.LUK, luk);
        Unit.SetBaseState(GASAttributeData.Instance.Grade_LUK, Grade);
        Unit.SetCalcBaseStateToDetailState();

        return Unit;
    }


    public async Task MonsterAction()
    {
        IsUpdate =false;
        foreach(var monster in UnitPositions.RightSlot)
        {
            if(monster!=null)
            {
                await TaskDelay(1500); 
                monster.Tick();
            }
        }
        IsUpdate=true;
    }

    private int currentCharacterIndex = 0; // 현재 처리할 캐릭터 인덱스
    
    [Header("디버그 - 현재 캐릭터 ActionCoin")]
    [SerializeField] private float[] currentActionCoins = new float[6]; // 6개 슬롯의 현재 ActionCoin
    
    [Button]
    public void UpdateActionCoinDisplay()
    {
        // 모든 캐릭터의 현재 ActionCoin을 배열에 업데이트
        for (int i = 0; i < UnitPositions.LeftSlot.Length; i++)
        {
            if (UnitPositions.LeftSlot[i] != null)
            {
                var character = UnitPositions.LeftSlot[i] as CharacterData;
                if (character != null)
                {
                    currentActionCoins[i] = character.GetState(GASAttributeData.Instance.ActionCoin);
                }
                else
                {
                    currentActionCoins[i] = 0f;
                }
            }
            else
            {
                currentActionCoins[i] = 0f;
            }
        }
        
    }

    public void CharacterAction(int CoinIndex)
    {
        // 현재 인덱스부터 캐릭터를 찾을 때까지 반복
        while (currentCharacterIndex < UnitPositions.LeftSlot.Length)
        {
            // 해당 인덱스에 캐릭터가 있는지 확인
            if (UnitPositions.LeftSlot[currentCharacterIndex] != null)
            {
                var character = UnitPositions.LeftSlot[currentCharacterIndex] as CharacterData;
                if (character != null)
                {
                    // 현재 ActionCoin 값을 가져와서 index만큼 증가 (최대값 제한 없음)
                    float currentCoin = character.GetState(GASAttributeData.Instance.ActionCoin);
                    float newCoin = currentCoin + CoinIndex + 1;
                    
                    // ActionCoin 값 업데이트
                    character.SetModifyState(GASAttributeData.Instance.ActionCoin, "CalcBase", newCoin, StackPolicy.Override);
                    
                    Debug.Log($"캐릭터 {character._name}의 ActionCoin이 {currentCoin}에서 {newCoin}으로 증가했습니다. (인덱스: {currentCharacterIndex})");
                    
                    // 디스플레이 업데이트
                    UpdateActionCoinDisplay();
                    
                    // 캐릭터를 찾았으므로 다음 호출을 위해 인덱스 증가 후 종료
                    currentCharacterIndex++;
                    return;
                }
            }
            else
            {
            }
            
            // 다음 인덱스로 이동
            currentCharacterIndex++;
        }
        
        // 모든 슬롯을 확인했지만 캐릭터를 찾지 못한 경우
        Debug.Log("모든 캐릭터가 처리되었습니다. 인덱스를 초기화합니다.");
        currentCharacterIndex = 0; // 모든 캐릭터 처리 후 초기화
    }

    /// <summary>
    /// 값을 한 범위에서 다른 범위로 정규화합니다.
    /// </summary>
    /// <param name="ValueMin">원본 범위의 최소값</param>
    /// <param name="ValueMax">원본 범위의 최대값</param>
    /// <param name="RangeMin">목표 범위의 최소값</param>
    /// <param name="RangeMax">목표 범위의 최대값</param>
    /// <param name="Range">정규화할 값</param>
    /// <returns>정규화된 값</returns>
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
        yield return new WaitForSeconds(2.0f);
        
        if (Vector3.Distance(unit.transform.position, targetPos) > 0.1f)
        {
            unit.toMove(targetPos, speed);
        }
    }
    private IEnumerator DelayStart(float waitTime)
    {
        // 스폰 애니메이션 등을 위한 충분한 시간 대기
        yield return new WaitForSeconds(waitTime);
        
        IsUpdate =true;
    }
    private IEnumerator DelayStageStart(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        StageStart();
    }
    [Button]
    public void StageStart()
    {
        ClearAllUnits();
        ClearAllMonsters();
        
        // 플레이어 유닛 배치
        var battleUnits = UserData.Instance.BattleUnit;
        int count = 0;
        for (int i = 0; i < battleUnits.Length; i++)
        {
            if (battleUnits[i] != null)
            {
                PlaceCharacterUnit(battleUnits[i], i);
                battleUnits[i].battleInit();
                battleUnits[i].gameObject.SetActive(true);
                count++;
            }
        }
        if(count ==0)
        {
            if(UserData.Instance.UnitList.Count==0)
                UserData.Instance.AddCharacter();
            UserData.Instance.BattleUnit[0] = UserData.Instance.UnitList[0];
            StageStart();
            return;
        }
        // 몬스터 배치 (현재는 랜덤 3~6마리, 나중에 데이터테이블에서 읽어올 예정)
        int monsterCount = GetMonsterCountForStage();
        
        for (int i = 0; i < monsterCount; i++)
        {
            var monster = CreateMonster(CurStage);
            PlaceMonsterByAttackType(monster);
            monster.battleInit();
        }
        StartCoroutine(DelayStart(StageStartDelayTime));
        Debug.Log($"스테이지 {CurStage}가 {GetActiveUnitCount()}개의 유닛과 {GetActiveMonsterCount()}개의 몬스터와 함께 시작되었습니다.");
    }
    
    private int GetMonsterCountForStage()
    {
        // TODO: 나중에 데이터테이블에서 스테이지별 몬스터 수를 읽어올 예정
        // 현재는 랜덤으로 3~6마리 생성
        return Random.Range(3, 7);
    }
    
    private void ClearAllMonsters()
    {
        // 기존 배치된 몬스터들 제거
        for (int i = 0; i < UnitPositions.RightSlot.Length; i++)
        {
            if (UnitPositions.RightSlot[i] != null)
            {
                Destroy(UnitPositions.RightSlot[i].gameObject);
                UnitPositions.RightSlot[i] = null;
            }
        }
    }
    
    private int GetActiveMonsterCount()
    {
        int count = 0;
        for (int i = 0; i < UnitPositions.RightSlot.Length; i++)
        {
            if (UnitPositions.RightSlot[i] != null)
            {
                count++;
            }
        }
        return count;
    }
    
    private void ClearAllUnits()
    {
        for (int i = 0; i < UnitPositions.LeftSlot.Length; i++)
        {
            if (UnitPositions.LeftSlot[i] != null)
            {
                Destroy(UnitPositions.LeftSlot[i].gameObject);
                UnitPositions.LeftSlot[i] = null;
            }
        }
    }
    
    private void PlaceCharacterUnit(CharacterData characterData, int positionIndex)
    {
        if (positionIndex >= UnitPositions.LeftSlot.Length)
        {
            return;
        }
        //var unit = Instantiate(characterData, UnitPositions.Left[positionIndex]);
        var unit = characterData;

        UnitPositions.DisposUnit(positionIndex,unit);
        UnitPositions.LeftSlot[positionIndex] = unit;
        unit.transform.localPosition = Vector3.zero;
        unit.transform.localRotation = Quaternion.identity;
        unit.transform.localScale = Vector3.one * 2.5f;
        
    }
    
    private int GetActiveUnitCount()
    {
        int count = 0;
        for (int i = 0; i < UnitPositions.LeftSlot.Length; i++)
        {
            if (UnitPositions.LeftSlot[i] != null)
            {
                count++;
            }
        }
        return count;
    }

    public void NextStage()
    {
        CurStage++;
        
        // 기존 몬스터들 제거
        ClearAllMonsters();
        
        // 새로운 몬스터들 배치
        int monsterCount = GetMonsterCountForStage();
        
        for (int i = 0; i < monsterCount; i++)
        {
            var monster = CreateMonster(CurStage);
            PlaceMonsterByAttackType(monster);
            monster.battleInit();
        }
        StartCoroutine(DelayStart(StageStartDelayTime));
        Debug.Log($"스테이지 {CurStage}로 진행되었습니다. {GetActiveMonsterCount()}개의 몬스터가 새로 배치되었습니다.");
    }
    
    private void Update()
    {
        if (!IsUpdate) return;
        
        foreach(var unit in UnitPositions.LeftSlot)
            if(unit!=null)
                unit.Tick();

        CheckMonsterStatus();
    }
    
    private void CheckMonsterStatus()
    {
        bool allMonstersDead = true;
        int aliveMonsterCount = 0;
        
        for (int i = 0; i < UnitPositions.RightSlot.Length; i++)
        {
            if (UnitPositions.RightSlot[i] != null)
            {
                var monster = UnitPositions.RightSlot[i] as MonsterData;
                if (monster != null&&!monster.isDead)
                {
                    float currentHP = monster.GetState(GASAttributeData.Instance.HP);
                    if (currentHP > 0)
                    {
                        allMonstersDead = false;
                        aliveMonsterCount++;
                    }
                }
            }
        }
        
        // 모든 몬스터가 죽었으면 다음 스테이지로 진행
        if (allMonstersDead && aliveMonsterCount == 0)
        {
            Debug.Log("모든 몬스터가 사망했습니다. 다음 스테이지로 진행합니다.");
            IsUpdate =false;

            NextStage();

            
        }
    }

    
    public async Task WaitUntilAsync(Func<bool> condition, int checkIntervalMs = 100)
    {
        while (!condition())
        {
            await Task.Delay(checkIntervalMs);
        }
    }
    public async Task TaskDelay(int ms = 1000)
    {
        await Task.Delay(ms);
    }
}

