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
        var dmg = context.Definition.DamageAttribute;


    }
}
public class MagicAttackExecutor : IAbilityExecutor
{
    public void Execute(AbilityContext context)
    {

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