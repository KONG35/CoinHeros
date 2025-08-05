using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GASAbilityComponent : DefinitionComponent<AbilityDefSO>
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
    }
    void Update()
    {
        specs.ForEach(s => s.Action());
    }

    public void SetCost(AttributeDefSO costSO, float value)
    {
        foreach(var a in specs)
        {
            for(int i=0;i<a.def.costs.Count;i++)
            {
                if(a.def.costs[i].attribute == costSO)
                {
                    var cost = a.def.costs[i];
                    cost.amount = value;
                    a.def.costs[i] = cost;
                }
            } 
        }
    }
}
class AbilitySpec
{
    public int Level = 0;
    public AbilityDefSO def;
    private GameObject owner;
    private IAbilityExecutor executor;

    GASTagComponent tags;
    GASAttributeSetComponent attriSet; 
        public AbilitySpec(AbilityDefSO def, GameObject owner)
    {
        this.def = def;
        this.owner = owner;
        
        // executor_FunctionName을 사용해서 실행자를 찾아서 저장
        if (!string.IsNullOrEmpty(def.executor_FunctionName))
        {
            this.executor = AbilityExecutor.GetExecutor(def.executor_FunctionName);
        }
        
        tags = owner.GetComponent<GASTagComponent>();
        attriSet = owner.GetComponent<GASAttributeSetComponent>();
    }
    public void Action()
    {
        if (!tags.HasAll(def.requiredTags) || tags.HasAny(def.blockedTags)) 
            return;
        if (!attriSet.HasEnough(def.costs)) return;
        if (executor == null) return;

        attriSet.Pay("Cost", def.costs);
        
        executor.Execute(new AbilityContext { Caster = owner, AbilityLevel = Level, Definition = def, Attributes = attriSet, Tags = tags });
    }

    public string abilityName()
    {
        return def.abilityName;
    }
}
