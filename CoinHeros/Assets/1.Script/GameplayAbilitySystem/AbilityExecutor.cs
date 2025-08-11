using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;

public static class AbilityExecutor
{
    private static readonly Dictionary<string, IAbilityExecutor> _map = new Dictionary<string, IAbilityExecutor>();

    static AbilityExecutor()
    {
        InitializeExecutors();
    }

    private static void InitializeExecutors()
    {
        // IAbilityExecutor를 구현하는 모든 클래스를 찾아서 등록
        var executorTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => typeof(IAbilityExecutor).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

        foreach (var type in executorTypes)
        {
            try
            {
                var instance = (IAbilityExecutor)System.Activator.CreateInstance(type);
                var executorName = GetExecutorName(type);
                _map[executorName] = instance;
                Debug.Log($"Registered executor: {executorName}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to create instance of {type.Name}: {e.Message}");
            }
        }
    }

    private static string GetExecutorName(System.Type type)
    {
        // 클래스 이름에서 "Executor" 접미사를 제거
        var name = type.Name;
        if (name.EndsWith("Executor"))
        {
            name = name.Substring(0, name.Length - "Executor".Length);
        }
        return name;
    }

    public static IAbilityExecutor GetExecutor(string abilityId)
    {
        if (_map.TryGetValue(abilityId, out var ex))
            return ex;
        Debug.LogWarning($"Executor not found for {abilityId}");
        return null;
    }

    // 등록된 모든 실행자 이름을 가져오는 메서드 (디버깅용)
    public static string[] GetRegisteredExecutorNames()
    {
        return _map.Keys.ToArray();
    }
}

public interface IAbilityExecutor
{
    void Execute(AbilityContext context);
}

// 공용 타겟 찾기 함수를 포함하는 정적 클래스
public static class TargetFinder
{
    /// <summary>
    /// 타겟 우선순위에 따라 적을 찾는 공용 함수
    /// </summary>
    /// <param name="caster">시전자</param>
    /// <param name="targetPriority">타겟 우선순위 배열 (인덱스 순서)</param>
    /// <returns>찾은 타겟, 없으면 null</returns>
    public static CharacterBase FindTarget(CharacterBase caster, int[] targetPriority)
    {
        var battleManager = BattleManager.Instance;
        if (battleManager == null || battleManager.UnitPositions == null)
        {
            Debug.LogError("BattleManager 또는 UnitPositions를 찾을 수 없습니다.");
            return null;
        }
        
        var unitPositions = battleManager.UnitPositions;
        var character = caster as CharacterData;
        
        // 캐릭터인지 몬스터인지에 따라 타겟 슬롯 결정
        var targetSlot = character != null ? unitPositions.RightSlot : unitPositions.LeftSlot;
        
        // 우선순위에 따라 타겟 찾기
        foreach (int index in targetPriority)
        {
            if (index < targetSlot.Length && targetSlot[index] != null)
            {
                var target = targetSlot[index] as CharacterBase;
                if (target != null && IsValidTarget(target))
                {
                    return target;
                }
            }
        }
        
        return null;
    }
    
    /// <summary>
    /// 범위 공격을 위한 타겟들을 찾는 함수 (양손검용)
    /// </summary>
    /// <param name="caster">시전자</param>
    /// <param name="mainTargetIndex">메인 타겟 인덱스</param>
    /// <returns>범위 내의 모든 타겟들</returns>
    public static List<CharacterBase> FindRangeTargets(CharacterBase caster, int mainTargetIndex)
    {
        var targets = new List<CharacterBase>();
        var battleManager = BattleManager.Instance;
        if (battleManager == null || battleManager.UnitPositions == null)
        {
            Debug.LogError("BattleManager 또는 UnitPositions를 찾을 수 없습니다.");
            return targets;
        }
        
        var unitPositions = battleManager.UnitPositions;
        var character = caster as CharacterData;
        var targetSlot = character != null ? unitPositions.RightSlot : unitPositions.LeftSlot;
        
        // 앞열(0,1,2)과 뒷열(3,4,5) 구분
        bool isFrontRow = mainTargetIndex <= 2;
        
        // 같은 열 내에서만 범위 공격
        int[] rangeIndices;
        if (isFrontRow)
        {
            // 앞열: 0, 1, 2 중에서 범위 계산
            rangeIndices = new int[] { mainTargetIndex - 1, mainTargetIndex, mainTargetIndex + 1 };
        }
        else
        {
            // 뒷열: 3, 4, 5 중에서 범위 계산
            rangeIndices = new int[] { mainTargetIndex - 1, mainTargetIndex, mainTargetIndex + 1 };
        }
        
        foreach (int index in rangeIndices)
        {
            // 같은 열 내에서만 유효한 인덱스인지 확인
            bool isValidIndex = false;
            if (isFrontRow)
            {
                isValidIndex = index >= 0 && index <= 2; // 앞열 범위
            }
            else
            {
                isValidIndex = index >= 3 && index <= 5; // 뒷열 범위
            }
            
            if (isValidIndex && index < targetSlot.Length && targetSlot[index] != null)
            {
                var target = targetSlot[index] as CharacterBase;
                if (target != null && IsValidTarget(target))
                {
                    targets.Add(target);
                }
            }
        }
        
        return targets;
    }
    
    /// <summary>
    /// 타겟이 유효한지 검증하는 함수 (죽지 않았고, 활성화되어 있는지 확인)
    /// </summary>
    /// <param name="target">검증할 타겟</param>
    /// <returns>유효한 타겟이면 true</returns>
    private static bool IsValidTarget(CharacterBase target)
    {
        if (target == null)
            return false;
            
        // 죽었는지 확인
        if (target.isDead)
            return false;
            
        // 게임오브젝트가 활성화되어 있는지 확인
        if (!target.gameObject.activeInHierarchy)
            return false;
            
        // HP가 0 이하인지 추가 확인
        var gasSOdata = GASAttributeData.Instance;
        if (gasSOdata != null)
        {
            float currentHP = target.GetState(gasSOdata.HP);
            if (currentHP <= 0)
                return false;
        }
        
        return true;
    }
    
    /// <summary>
    /// 기본 타겟 우선순위 (앞열 우선: 중앙 -> 위 -> 아래 -> 뒷열)
    /// </summary>
    public static readonly int[] DefaultTargetPriority = { 1, 0, 2, 4, 3, 5 };
    
    /// <summary>
    /// 뒷열 우선 타겟 순서 (마법사 등이 선호하는 순서)
    /// </summary>
    public static readonly int[] BackRowTargetPriority = { 4, 3, 5, 1, 0, 2 };
    
    /// <summary>
    /// 랜덤 타겟 순서
    /// </summary>
    public static int[] GetRandomTargetPriority()
    {
        var priority = new int[] { 0, 1, 2, 3, 4, 5 };
        for (int i = priority.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (priority[i], priority[j]) = (priority[j], priority[i]);
        }
        return priority;
    }
}

public class MeleeAttackExecutor : IAbilityExecutor
{
    public void Execute(AbilityContext context)
    {
        var caster = context.Caster;
        var casterCharacter = caster.GetComponent<CharacterBase>();
        var gasSOdata = GASAttributeData.Instance;
        
        if (casterCharacter == null)
        {
            Debug.LogError("캐릭터 컴포넌트를 찾을 수 없습니다.");
            return;
        }
        
        // 물리 공격력 가져오기
        float damage = casterCharacter.GetState(gasSOdata.AttackDamage);
        
        // 공격 애니메이션 재생
        casterCharacter.PlayAnim(CharacterBase.eAnimState.Attack);
        
        // 공용 타겟 찾기 함수 사용 (밀리 공격: 앞열 우선)
        var target = TargetFinder.FindTarget(casterCharacter, TargetFinder.DefaultTargetPriority);
        if (target != null)
        {
            Debug.Log($"{casterCharacter._name}이(가) {target._name}에게 근접 공격을 시도합니다. (데미지: {damage})");
            target.Hit(damage, gasSOdata.AttackDamage);
        }
        else
        {
            Debug.Log($"{casterCharacter._name}이(가) 근접 공격을 시도했지만 타겟이 없습니다. (데미지: {damage})");
        }
    }
}

public class MagicAttackExecutor : IAbilityExecutor
{
    public void Execute(AbilityContext context)
    {
        var caster = context.Caster;
        var casterCharacter = caster.GetComponent<CharacterBase>();
        var gasSOdata = GASAttributeData.Instance;
        
        if (casterCharacter == null)
        {
            Debug.LogError("캐릭터 컴포넌트를 찾을 수 없습니다.");
            return;
        }
        
        // 마법 공격력 가져오기
        float damage = casterCharacter.GetState(gasSOdata.MagicDamage);
        
        // 공격 애니메이션 재생
        casterCharacter.PlayAnim(CharacterBase.eAnimState.Ability);
        
        // 공용 타겟 찾기 함수 사용 (마법 공격: 앞열 우선)
        var target = TargetFinder.FindTarget(casterCharacter, TargetFinder.DefaultTargetPriority);
        if (target != null)
        {
            target.Hit(damage, gasSOdata.MagicDamage);
            Debug.Log($"{casterCharacter._name}이(가) {target._name}에게 마법 공격을 시도합니다. (데미지: {damage})");
        }
        else
        {
            Debug.Log($"{casterCharacter._name}이(가) 마법 공격을 시도했지만 타겟이 없습니다. (데미지: {damage})");
        }
    }
}

public class ArrowAttackExecutor : IAbilityExecutor
{
    public void Execute(AbilityContext context)
    {
        var caster = context.Caster;
        var casterCharacter = caster.GetComponent<CharacterBase>();
        var gasSOdata = GASAttributeData.Instance;
        
        if (casterCharacter == null)
        {
            Debug.LogError("캐릭터 컴포넌트를 찾을 수 없습니다.");
            return;
        }
        
        // 물리 공격력 가져오기 (화살 공격)
        float damage = casterCharacter.GetState(gasSOdata.AttackDamage);
        
        // 공격 애니메이션 재생
        casterCharacter.PlayAnim(CharacterBase.eAnimState.Attack);
        
        // 화살 공격: 뒷열 우선 (마법사나 힐러를 먼저 공격)
        var target = TargetFinder.FindTarget(casterCharacter, TargetFinder.BackRowTargetPriority);
        if (target != null)
        {
            Debug.Log($"{casterCharacter._name}이(가) {target._name}에게 화살 공격을 시도합니다. (데미지: {damage})");
            target.Hit(damage, gasSOdata.AttackDamage);
        }
        else
        {
            Debug.Log($"{casterCharacter._name}이(가) 화살 공격을 시도했지만 타겟이 없습니다. (데미지: {damage})");
        }
    }
}

public class OneHandSwordAttackExecutor : IAbilityExecutor
{
    public void Execute(AbilityContext context)
    {
        var caster = context.Caster;
        var casterCharacter = caster.GetComponent<CharacterBase>();
        var gasSOdata = GASAttributeData.Instance;
        
        if (casterCharacter == null)
        {
            Debug.LogError("캐릭터 컴포넌트를 찾을 수 없습니다.");
            return;
        }
        
        // 물리 공격력 가져오기
        float damage = casterCharacter.GetState(gasSOdata.AttackDamage);
        
        // 공격 애니메이션 재생
        casterCharacter.PlayAnim(CharacterBase.eAnimState.Attack);
        
        // 한손검 공격: 기본 우선순위
        var target = TargetFinder.FindTarget(casterCharacter, TargetFinder.DefaultTargetPriority);
        if (target != null)
        {
            Debug.Log($"{casterCharacter._name}이(가) {target._name}에게 한손검 공격을 시도합니다. (데미지: {damage})");
            target.Hit(damage, gasSOdata.AttackDamage);
        }
        else
        {
            Debug.Log($"{casterCharacter._name}이(가) 한손검 공격을 시도했지만 타겟이 없습니다. (데미지: {damage})");
        }
    }
}

public class TwoHandSwordAttackExecutor : IAbilityExecutor
{
    public void Execute(AbilityContext context)
    {
        var caster = context.Caster;
        var casterCharacter = caster.GetComponent<CharacterBase>();
        var gasSOdata = GASAttributeData.Instance;
        
        if (casterCharacter == null)
        {
            Debug.LogError("캐릭터 컴포넌트를 찾을 수 없습니다.");
            return;
        }
        
        // 물리 공격력 가져오기 (양손검은 범위 공격으로 0.5배 데미지)
        float damage = casterCharacter.GetState(gasSOdata.AttackDamage) * 0.5f;
        
        // 공격 애니메이션 재생
        casterCharacter.PlayAnim(CharacterBase.eAnimState.Attack);
        
        // 양손검 공격: 기본 우선순위로 메인 타겟 찾기
        var mainTarget = TargetFinder.FindTarget(casterCharacter, TargetFinder.DefaultTargetPriority);
        if (mainTarget != null)
        {
            // 메인 타겟의 인덱스 찾기
            var battleManager = BattleManager.Instance;
            var unitPositions = battleManager.UnitPositions;
            var character = casterCharacter as CharacterData;
            var targetSlot = character != null ? unitPositions.RightSlot : unitPositions.LeftSlot;
            
            int mainTargetIndex = -1;
            for (int i = 0; i < targetSlot.Length; i++)
            {
                if (targetSlot[i] == mainTarget)
                {
                    mainTargetIndex = i;
                    break;
                }
            }
            
            if (mainTargetIndex != -1)
            {
                // 범위 공격으로 메인 타겟과 양옆 타겟들 공격
                var rangeTargets = TargetFinder.FindRangeTargets(casterCharacter, mainTargetIndex);
                
                Debug.Log($"{casterCharacter._name}이(가) 양손검 범위 공격을 시도합니다. (데미지: {damage}, 타겟 수: {rangeTargets.Count})");
                
                foreach (var target in rangeTargets)
                {
                    target.Hit(damage, gasSOdata.AttackDamage);
                }
            }
        }
        else
        {
            Debug.Log($"{casterCharacter._name}이(가) 양손검 공격을 시도했지만 타겟이 없습니다. (데미지: {damage})");
        }
    }
}

public class DoubleSwordAttackExecutor : IAbilityExecutor
{
    public void Execute(AbilityContext context)
    {
        var caster = context.Caster;
        var casterCharacter = caster.GetComponent<CharacterBase>();
        var gasSOdata = GASAttributeData.Instance;
        
        if (casterCharacter == null)
        {
            Debug.LogError("캐릭터 컴포넌트를 찾을 수 없습니다.");
            return;
        }
        
        // 물리 공격력 가져오기 (쌍검은 1배 데미지로 두 번 공격)
        float damage = casterCharacter.GetState(gasSOdata.AttackDamage);
        
        // 공격 애니메이션 재생
        casterCharacter.PlayAnim(CharacterBase.eAnimState.Attack);
        
        // 쌍검 공격: 랜덤 타겟으로 두 번 공격
        var target = TargetFinder.FindTarget(casterCharacter, TargetFinder.GetRandomTargetPriority());
        if (target != null)
        {
            Debug.Log($"{casterCharacter._name}이(가) {target._name}에게 쌍검 이중 공격을 시도합니다. (데미지: {damage} x 2)");
            
            // 첫 번째 공격
            target.Hit(damage, gasSOdata.AttackDamage);
            
            // 두 번째 공격 (약간의 딜레이 없이)
            target.Hit(damage, gasSOdata.AttackDamage);
        }
        else
        {
            Debug.Log($"{casterCharacter._name}이(가) 쌍검 공격을 시도했지만 타겟이 없습니다. (데미지: {damage})");
        }
    }
}

public class SwordAndShieldAttackExecutor : IAbilityExecutor
{
    public void Execute(AbilityContext context)
    {
        var caster = context.Caster;
        var casterCharacter = caster.GetComponent<CharacterBase>();
        var gasSOdata = GASAttributeData.Instance;
        
        if (casterCharacter == null)
        {
            Debug.LogError("캐릭터 컴포넌트를 찾을 수 없습니다.");
            return;
        }
        
        // 물리 공격력 가져오기 (검+방패는 1배 데미지로 한 번 공격)
        float damage = casterCharacter.GetState(gasSOdata.AttackDamage);
        
        // 공격 애니메이션 재생
        casterCharacter.PlayAnim(CharacterBase.eAnimState.Attack);
        
        // 검+방패 공격: 기본 우선순위로 한 번 공격
        var target = TargetFinder.FindTarget(casterCharacter, TargetFinder.DefaultTargetPriority);
        if (target != null)
        {
            Debug.Log($"{casterCharacter._name}이(가) {target._name}에게 검+방패 공격을 시도합니다. (데미지: {damage})");
            target.Hit(damage, gasSOdata.AttackDamage);
        }
        else
        {
            Debug.Log($"{casterCharacter._name}이(가) 검+방패 공격을 시도했지만 타겟이 없습니다. (데미지: {damage})");
        }
    }
}

public class SpearAttackExecutor : IAbilityExecutor
{
    public void Execute(AbilityContext context)
    {
        var caster = context.Caster;
        var casterCharacter = caster.GetComponent<CharacterBase>();
        var gasSOdata = GASAttributeData.Instance;
        
        if (casterCharacter == null)
        {
            Debug.LogError("캐릭터 컴포넌트를 찾을 수 없습니다.");
            return;
        }
        
        // 물리 공격력 가져오기 (창은 관통 공격)
        float damage = casterCharacter.GetState(gasSOdata.AttackDamage) * 1.3f;
        float penetration = 30f; // 고정 관통력 30
        float penetrationPercent = 25f; // 방어력의 25% 관통
        
        // 공격 애니메이션 재생
        casterCharacter.PlayAnim(CharacterBase.eAnimState.Attack);
        
        // 창 공격: 기본 우선순위 (관통 특성)
        var target = TargetFinder.FindTarget(casterCharacter, TargetFinder.DefaultTargetPriority);
        if (target != null)
        {
            Debug.Log($"{casterCharacter._name}이(가) {target._name}에게 창 관통 공격을 시도합니다. (데미지: {damage}, 관통력: {penetration} + {penetrationPercent}%)");
            target.Hit(damage, gasSOdata.AttackDamage, penetration, penetrationPercent);
        }
        else
        {
            Debug.Log($"{casterCharacter._name}이(가) 창 공격을 시도했지만 타겟이 없습니다. (데미지: {damage})");
        }
    }
}




