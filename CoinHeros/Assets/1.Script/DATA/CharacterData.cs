using NaughtyAttributes;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.UI;




[RequireComponent(typeof(TinyCharacterController))]
public class CharacterData : CharacterBase
{
    public AnimatorOverrideController[] jobAnims;
    public TinyCharacterController _controller;
    protected override void Start()
    {
        base.Start();
    }


    

    public void LvUpState()
    {

    }

    public enum eJobAnim
    {
        SingleSword,
        DoubleSword,
        SwordAndShield,
        TwoHandSword,
        Spear,
        Archer,
        Magic,
        None
    }

    /// <summary>
    /// 애니메이션 이벤트 함수 -아쳐 Attack
    /// </summary>
    public void OnBowAttackStart(int index)
    {
        switch (index)
        {
            case 1:
                _controller.Bow.CrossFade("Attack01", 0.1f);
                _controller.Arrow.CrossFade("Attack01", 0.1f);
                break;
            case 2:
                _controller.Bow.CrossFade("Attack02", 0.1f);
                _controller.Arrow.CrossFade("Attack02", 0.1f);
                break;
            case 3:
                _controller.Bow.CrossFade("Attack03", 0.1f);
                _controller.Arrow.CrossFade("Attack03", 0.1f);
                break;
            case 4:
                _controller.Bow.CrossFade("Attack04", 0.1f);
                _controller.Arrow.CrossFade("Attack04", 0.1f);
                break;
        }
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