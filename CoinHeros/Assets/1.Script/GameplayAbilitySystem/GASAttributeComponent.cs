using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using NaughtyAttributes;
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

    #region 디버그 인스펙터
    [Header("=== 디버그 인스펙터 ===")]
    [SerializeField] private bool showDebugInfo = true;
    
    [Header("기본 능력치")]
    [ShowIf("showDebugInfo")]
    [ReadOnly]
    public float debug_STR;
    [ShowIf("showDebugInfo")]
    [ReadOnly]
    public float debug_MAG;
    [ShowIf("showDebugInfo")]
    [ReadOnly]
    public float debug_CON;
    [ShowIf("showDebugInfo")]
    [ReadOnly]
    public float debug_AGI;
    [ShowIf("showDebugInfo")]
    [ReadOnly]
    public float debug_SPR;
    [ShowIf("showDebugInfo")]
    [ReadOnly]
    public float debug_LUK;
    
    [Header("전투 능력치")]
    [ShowIf("showDebugInfo")]
    [ReadOnly]
    public float debug_HP;
    [ShowIf("showDebugInfo")]
    [ReadOnly]
    public float debug_MaxHP;
    [ShowIf("showDebugInfo")]
    [ReadOnly]
    public float debug_MP;
    [ShowIf("showDebugInfo")]
    [ReadOnly]
    public float debug_MaxMP;
    [ShowIf("showDebugInfo")]
    [ReadOnly]
    public float debug_AttackDamage;
    [ShowIf("showDebugInfo")]
    [ReadOnly]
    public float debug_MagicDamage;
    [ShowIf("showDebugInfo")]
    [ReadOnly]
    public float debug_AttackDefence;
    [ShowIf("showDebugInfo")]
    [ReadOnly]
    public float debug_MagicDefence;
    
    [Header("액션 코인")]
    [ShowIf("showDebugInfo")]
    [ReadOnly]
    public float debug_ActionCoin;
    [ShowIf("showDebugInfo")]
    [ReadOnly]
    public float debug_MaxActionCoin;
    
    [Header("기타")]
    [ShowIf("showDebugInfo")]
    [ReadOnly]
    public float debug_LV;
    [ShowIf("showDebugInfo")]
    [ReadOnly]
    public float debug_EXP;
    [ShowIf("showDebugInfo")]
    [ReadOnly]
    public float debug_Rank;
    
    [ShowIf("showDebugInfo")]
    [Button("능력치 새로고침")]
    public void RefreshDebugValues()
    {
        UpdateDebugValues();
    }
    
    [ShowIf("showDebugInfo")]
    [Button("모든 능력치 출력")]
    public void PrintAllAttributes()
    {
        Debug.Log("=== 모든 능력치 정보 ===");
        foreach (var def in definitions)
        {
            float value = GetValue(def);
            Debug.Log($"{def.name}: {value} (기본값: {def.DefaultValue}, 최소: {def.MinValue}, 최대: {def.MaxValue})");
        }
    }
    
    private void UpdateDebugValues()
    {
        if (!showDebugInfo) return;
        
        // 기본 능력치
        debug_STR = GetValue(GASAttributeData.Instance.STR);
        debug_MAG = GetValue(GASAttributeData.Instance.MAG);
        debug_CON = GetValue(GASAttributeData.Instance.CON);
        debug_AGI = GetValue(GASAttributeData.Instance.AGI);
        debug_SPR = GetValue(GASAttributeData.Instance.SPR);
        debug_LUK = GetValue(GASAttributeData.Instance.LUK);
        
        // 전투 능력치
        debug_HP = GetValue(GASAttributeData.Instance.HP);
        debug_MaxHP = GetValue(GASAttributeData.Instance.MaxHP);
        debug_MP = GetValue(GASAttributeData.Instance.MP);
        debug_MaxMP = GetValue(GASAttributeData.Instance.MaxMP);
        debug_AttackDamage = GetValue(GASAttributeData.Instance.AttackDamage);
        debug_MagicDamage = GetValue(GASAttributeData.Instance.MagicDamage);
        debug_AttackDefence = GetValue(GASAttributeData.Instance.AttackDefence);
        debug_MagicDefence = GetValue(GASAttributeData.Instance.MagicDefence);
        
        // 액션 코인
        debug_ActionCoin = GetValue(GASAttributeData.Instance.ActionCoin);
        debug_MaxActionCoin = GetValue(GASAttributeData.Instance.MaxActionCoin);
        
        // 기타
        debug_LV = GetValue(GASAttributeData.Instance.LV);
        debug_EXP = GetValue(GASAttributeData.Instance.EXP);
        debug_Rank = GetValue(GASAttributeData.Instance.Rank);
    }
    
    private void Update()
    {
        if (showDebugInfo)
        {
            UpdateDebugValues();
        }
    }
    #endregion
}
