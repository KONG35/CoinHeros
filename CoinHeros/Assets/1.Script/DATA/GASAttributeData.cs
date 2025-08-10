using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GASAttributeData : Singleton<GASAttributeData>
{
    public AttributeDefSO Rank;
    public AttributeDefSO LV;
    public AttributeDefSO EXP;
    public AttributeDefSO STR;
    public AttributeDefSO Grade_STR;
    public AttributeDefSO MAG;
    public AttributeDefSO Grade_MAG;
    public AttributeDefSO CON;
    public AttributeDefSO Grade_CON;
    public AttributeDefSO AGI;
    public AttributeDefSO Grade_AGI;
    public AttributeDefSO SPR;
    public AttributeDefSO Grade_SPR;
    public AttributeDefSO LUK;
    public AttributeDefSO Grade_LUK;
    public AttributeDefSO MaxHP;
    public AttributeDefSO HP;
    public AttributeDefSO MaxMP;
    public AttributeDefSO MP;
    public AttributeDefSO ActionCoin;
    public AttributeDefSO MaxActionCoin;
    public AttributeDefSO AttackDamage;
    public AttributeDefSO AttackDefence;
    public AttributeDefSO MagicDamage;
    public AttributeDefSO MagicDefence;



    /// Abillity
    public AbilityDefSO ArrowAttack;
    public AbilityDefSO DoubleSwordAttack;
    public AbilityDefSO MagicAttack;
    public AbilityDefSO MeleeAttack;
    public AbilityDefSO OneHandSwordAttack;
    public AbilityDefSO SpearAttack;
    public AbilityDefSO SwordAndShieldAttack;
    public AbilityDefSO TwoHandSwordAttack;



    ///Monstyer Abillity
    /// 
    public AbilityDefSO MonsterMeleeAttack;
    public AbilityDefSO MonsterMagicAttack;
}
