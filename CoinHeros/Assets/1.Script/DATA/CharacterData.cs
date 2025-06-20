using System.Collections;
using UnityEngine;
using UnityEngine.UI;




[RequireComponent(typeof(GASTagComponent))]
[RequireComponent(typeof(GASAttributeSetComponent))]
[RequireComponent(typeof(GASAbilityComponent))]
[RequireComponent(typeof(GASCueComponent))]
public class CharacterData : MonoBehaviour
{
    public bool isInit = false;
    public string _name;
    public Texture2D Image;
    GASAbilityComponent _ability;
    GASAttributeSetComponent _state;
    GASCueComponent _cue;
    GASTagComponent _tag;

    public CharacterBaseState BaseState;


    public void Awake()
    {
        _ability = GetComponent<GASAbilityComponent>();
        _state = GetComponent<GASAttributeSetComponent>();
        _cue = GetComponent<GASCueComponent>();
        _tag = GetComponent<GASTagComponent>();
        isInit = true;
    }


    public float GetState(AttributeDefSO SO)
    {
        return _state.GetValue(SO);
    }

    public int GetLv()
    {
        float exp = GetState(GASAttributeData.Instance.EXP);



        return 0;
    }
}


public struct CharacterBaseState
{
    public string Name;
    public int EXP;
    public BaseState STR;
    public BaseState MAG;
    public BaseState CON;
    public BaseState AGI;
    public BaseState SPR;
    public BaseState LCK;
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