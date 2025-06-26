using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class GASAttributeSetComponent : DefinitionComponent<AttributeDefSO>
{
    public class Modifier
    {
        public Modifier(string key, float value, StackPolicy policy)
        {
            this.key = key;
            magnitude = value;
            stackingPolicy = policy;
        }
        public string key;
        public float magnitude;
        public StackPolicy stackingPolicy;
    }
    Dictionary<AttributeDefSO, float> baseValues;
    Dictionary<AttributeDefSO, List<Modifier>> mods;
    protected override void OnInitialized(List<AttributeDefSO> defs)
    {
        baseValues = defs.ToDictionary(def => def, def => def.DefaultValue);
        mods = defs.ToDictionary(def => def, def => new List<Modifier>());
    }
    public float GetValue(AttributeDefSO def)
    {
        return GetFinalValue(def);
    }
    private float GetFinalValue(AttributeDefSO def)
    {
        float val = baseValues[def];
        for (int i = 0; i<mods[def].Count;i++)
        {
            switch (mods[def][i].stackingPolicy)
            {
                case StackPolicy.Add: val += mods[def][i].magnitude; break;
                case StackPolicy.Multiply: val *= mods[def][i].magnitude; break;
                case StackPolicy.Override: val = mods[def][i].magnitude; break;
            }
        }
        return Mathf.Clamp(val, def.MinValue, def.MaxValue);
    }

    public void SetBaseValue(AttributeDefSO def, float value)
    {
        baseValues[def] = value;
    }

    public void ModifyValue(AttributeDefSO def,string key, float delta, StackPolicy policy)
    {
        if (policy == StackPolicy.Override)
            mods[def].Clear();
        mods[def].Add(new Modifier(key,delta, policy));
    }
    public void RemoveValue(AttributeDefSO def, string key)
    {
        mods[def].Remove(mods[def].Find(x => x.key == key));
    }
    public bool HasEnough(List<AttributeCost> costs)
    {
        foreach (var c in costs)
            if (GetFinalValue(c.attribute) < c.amount) return false;
        return true;
    }
    public void Pay(string key,List<AttributeCost> costs)
    {
        foreach (var c in costs)
            mods[c.attribute].Add(new Modifier(key,-c.amount,StackPolicy.Add));
    }

    public void AddDefinitionAttribute(AttributeDefSO so)
    {
        add(so);
    }
}
