using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GAS/AbilityDef")]
public class AbilityDefSO : ScriptableObject
{
    public string abilityName;

    public List<AbilityLevelStateData> LevelState;

    public AttributeDefSO DamageAttribute;
    public AttributeDefSO DamagetargetAttribute;
    public List<string> requiredTags;
    public List<string> blockedTags;
    public Sprite Icon;
    public AbilityExecutorSO executor;
}
[System.Serializable]
public struct AbilityLevelStateData
{
    [Min(1)] public int level;
    public float Damage;
    public float CharactorDamageMultiplier;
    public float Cooldown;
    public float MinRange;
    public float MaxRange;
    public float Speed;
    public float PierceCount;
    public float BounceCount;
    public float Scale;
    public List<AttributeCost> costs;
}