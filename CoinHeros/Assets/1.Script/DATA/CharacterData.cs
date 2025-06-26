using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;




public class CharacterData : CharacterBase
{


    protected override void Start()
    {
        base.Start();
    }

    

    public void SetCalcBaseStateToDetailState()
    {
        var table = DataTableManager.Instance;
        var gasSOdata = GASAttributeData.Instance;
        float value = 0.0f;

        //AttackDamage
        value = GetState(gasSOdata.STR) * table.CONGradeEfficiency[(int)GetState(gasSOdata.Grade_STR)] * 0.3f;
        SetModifyState(gasSOdata.AttackDamage, "CalcBase", value, StackPolicy.Override);
        //MagicDamage
        value = GetState(gasSOdata.MAG) * table.CONGradeEfficiency[(int)GetState(gasSOdata.Grade_MAG)] * 0.3f;
        SetModifyState(gasSOdata.MagicDamage, "CalcBase", value, StackPolicy.Override);
        //AttackDefence
        value = (GetState(gasSOdata.STR) * table.CONGradeEfficiency[(int)GetState(gasSOdata.Grade_STR)] *0.1f)
            + (GetState(gasSOdata.CON) * table.CONGradeEfficiency[(int)GetState(gasSOdata.Grade_CON)] *0.2f);
        SetModifyState(gasSOdata.AttackDefence, "CalcBase", value, StackPolicy.Override);
        //MagicDefence
        value = (GetState(gasSOdata.MAG) * table.CONGradeEfficiency[(int)GetState(gasSOdata.Grade_MAG)] * 0.1f)
            + (GetState(gasSOdata.AGI) * table.CONGradeEfficiency[(int)GetState(gasSOdata.Grade_AGI)] * 0.2f);
        SetModifyState(gasSOdata.MagicDefence, "CalcBase", value, StackPolicy.Override);
        //HP
        value = (GetState(gasSOdata.STR) * table.CONGradeEfficiency[(int)GetState(gasSOdata.Grade_STR)] * 0.25f)
            + (GetState(gasSOdata.CON) * table.CONGradeEfficiency[(int)GetState(gasSOdata.Grade_CON)] * 1.0f);
        SetModifyState(gasSOdata.HP, "CalcBase", value, StackPolicy.Override);
        //ActionCoin
        value = 4f;
        SetModifyState(gasSOdata.ActionCoin, "CalcBase", value, StackPolicy.Override);
    }
}

public struct BaseState
{
    public int value;
    public eGrade grade;

    public BaseState(int v,int g)
    {
        value = v;
        grade = (eGrade)g;
    }
}

public enum eGrade
{
    F,
    E,
    D,
    C,
    B,
    A,
    S,
    COUNT
}