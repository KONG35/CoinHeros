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
        
        // 타겟 찾기 (밀리 공격: 앞열 우선)
        var target = FindMeleeTarget();
        if (target != null)
        {
            target.Hit(damage, gasSOdata.AttackDamage);
            Debug.Log($"{casterCharacter._name}이(가) {target._name}에게 근접 공격을 시도합니다. (데미지: {damage})");
        }
        else
        {
            Debug.Log($"{casterCharacter._name}이(가) 근접 공격을 시도했지만 타겟이 없습니다. (데미지: {damage})");
        }
    }
    
    private CharacterBase FindMeleeTarget()
    {
        var battleManager = BattleManager.Instance;
        if (battleManager == null || battleManager.UnitPositions == null)
        {
            Debug.LogError("BattleManager 또는 UnitPositions를 찾을 수 없습니다.");
            return null;
        }
        
        var unitPositions = battleManager.UnitPositions;
        
        // 앞열 우선 타겟 순서: 중앙(1) -> 위(0) -> 아래(2) -> 뒷열 중앙(4) -> 뒷열 위(3) -> 뒷열 아래(5)
        int[] targetPriority = { 1, 0, 2, 4, 3, 5 };
        
        foreach (int index in targetPriority)
        {
            if (index < unitPositions.RightSlot.Length && unitPositions.RightSlot[index] != null &&!unitPositions.RightSlot[index].isDead)
            {
                var target = unitPositions.RightSlot[index] as CharacterBase;
                if (target != null)
                {
                    return target;
                }
            }
        }
        
        return null;
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
        
        // 타겟 찾기 (마법 공격: 앞열 우선)
        var target = FindMagicTarget();
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
    
    private CharacterBase FindMagicTarget()
    {
        var battleManager = BattleManager.Instance;
        if (battleManager == null || battleManager.UnitPositions == null)
        {
            Debug.LogError("BattleManager 또는 UnitPositions를 찾을 수 없습니다.");
            return null;
        }
        
        var unitPositions = battleManager.UnitPositions;
        
        // 앞열 우선 타겟 순서: 중앙(1) -> 위(0) -> 아래(2) -> 뒷열 중앙(4) -> 뒷열 위(3) -> 뒷열 아래(5)
        int[] targetPriority = { 1, 0, 2, 4, 3, 5 };
        
        foreach (int index in targetPriority)
        {
            if (index < unitPositions.RightSlot.Length && unitPositions.RightSlot[index] != null&&!unitPositions.RightSlot[index].isDead)
            {
                var target = unitPositions.RightSlot[index] as CharacterBase;
                if (target != null)
                {
                    return target;
                }
            }
        }
        
        return null;
    }
}
public class ArrowAttackExecutor : IAbilityExecutor
{
    public void Execute(AbilityContext context)
    {

    }
}
public class OneHandSwordAttackExecutor : IAbilityExecutor
{
    public void Execute(AbilityContext context)
    {

    }
}

public class TwoHandSwordAttackExecutor : IAbilityExecutor
{
    public void Execute(AbilityContext context)
    {

    }
}

public class DoubleSwordAttackExecutor : IAbilityExecutor
{
    public void Execute(AbilityContext context)
    {

    }
}

public class SwordAndShieldAttackExecutor : IAbilityExecutor
{
    public void Execute(AbilityContext context)
    {

    }
}

public class SpearAttackExecutor : IAbilityExecutor
{
    public void Execute(AbilityContext context)
    {

    }
}