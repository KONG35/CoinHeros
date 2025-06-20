using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public class GASAttributeSetComponent : DefinitionComponent<AttributeDefSO>
{
    public class Modifier
    {
        public Modifier(float value)
        {
            magnitude = value;
        }
        public float magnitude;
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
        foreach (var m in mods[def])
        {
            switch (def.stackingPolicy)
            {
                case StackPolicy.Add: val += m.magnitude; break;
                case StackPolicy.Multiply: val *= m.magnitude; break;
                case StackPolicy.Override: val = m.magnitude; break;
            }
        }
        return Mathf.Clamp(val, def.MinValue, def.MaxValue);
    }

    public void ModifyValue(AttributeDefSO def, float delta)
    {
        mods[def].Add(new Modifier(delta));
    }
    public bool HasEnough(List<AttributeCost> costs)
    {
        foreach (var c in costs)
            if (GetFinalValue(c.attribute) < c.amount) return false;
        return true;
    }
    public void Pay(List<AttributeCost> costs)
    {
        foreach (var c in costs)
            mods[c.attribute].Add(new Modifier(-c.amount));
    }
}
