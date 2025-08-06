using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "GAS/AbilityDef")]
public class AbilityDefSO : ScriptableObject
{
    public string abilityName;
    public List<AttributeCost> costs;
    public AttributeDefSO DamageAttribute;
    public AttributeDefSO DamagetargetAttribute;
    public List<string> requiredTags;
    public List<string> blockedTags;
    public Sprite Icon;
    public string executor_FunctionName;
}

