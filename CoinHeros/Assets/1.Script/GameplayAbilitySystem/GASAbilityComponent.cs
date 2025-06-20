using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class GASAbilityComponent : DefinitionComponent<AbilityDefSO>
{
    List<AbilitySpec> specs;
    protected override void OnInitialized(List<AbilityDefSO> defs)
    { 
        specs = defs.ConvertAll(d => new AbilitySpec(d, gameObject));
    }
    public void AddAbility(AbilityDefSO defSo)
    {
        var abil = specs.Find(x => x.abilityName() == defSo.abilityName);
        if (abil==null)
        {
            specs.Add(new AbilitySpec(defSo, gameObject));
        }
        else
        {
            abil.SetLevel(abil.Level+1);
        }
    }
    public int GetAbilityLv(AbilityDefSO defSo)
    {
        var abil = specs.Find(x => x.abilityName() == defSo.abilityName);
        if (abil == null)
        {
            return 0;
        }
        return abil.Level+1;
    }
    void Update()
    {
        specs.ForEach(s => s.Action());
    }
}
class AbilitySpec
{
    public int Level = 0;
    private AbilityDefSO def;
    private GameObject owner;
    AbilityExecutorSO executor;
    GASTagComponent tags;
    GASAttributeSetComponent attriSet; 
    float lastUsedTime = -999f;
    public AbilitySpec(AbilityDefSO def, GameObject owner)
    {
        this.def = def;
        this.owner = owner;
        this.executor = def.executor;
        tags = owner.GetComponent<GASTagComponent>();
        attriSet = owner.GetComponent<GASAttributeSetComponent>();
    }
    public void SetLevel(int newLevel)
    {
        Level = Mathf.Clamp(newLevel, 1, def.LevelState.Count-1);
    }
    public void Action()
    {
        var State = def.LevelState[Level];
        if (Time.time - lastUsedTime < State.Cooldown) return;
        if (!tags.HasAll(def.requiredTags) || tags.HasAny(def.blockedTags)) 
            return;
        if (!attriSet.HasEnough(State.costs)) return;
        attriSet.Pay(State.costs);

        executor.Execute(new AbilityContext { Caster = owner, AbilityLevel = Level, Definition = def, Attributes = attriSet, Tags = tags });
    }

    public string abilityName()
    {
        return def.abilityName;
    }
}
