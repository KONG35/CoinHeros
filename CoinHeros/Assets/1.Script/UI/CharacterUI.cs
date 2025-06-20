using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterUI : MonoBehaviour
{
    public RawImage image;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI LV;
    public TextMeshProUGUI Exp;
    public Button RankUpBtn;


    public List<BaseStateUI> StateUI;
    public BaseStateUI etc_S_UI;



    public TextMeshProUGUI HP;
    public TextMeshProUGUI ActionCoin;
    public TextMeshProUGUI AttackDamage;
    public TextMeshProUGUI MagicDamage;
    public TextMeshProUGUI AttackDefence;
    public TextMeshProUGUI MagicDefence;

    public void Start()
    {
        etc_S_UI.GradeBack.transform.gameObject.SetActive(false);
    }



    public void SetCharacterData(CharacterData data)
    {
        image.texture = data.Image;
        Name.text = data._name;
        int curLv = (int)data.GetState(GASAttributeData.Instance.LV);
        LV.text = curLv.ToString()+ "/" + 
            DataTableManager.Instance.RankMaxLvTable[(int)data.GetState(GASAttributeData.Instance.Rank)];
        Exp.text = ((int)data.GetState(GASAttributeData.Instance.EXP) - DataTableManager.Instance.GetTotalPrevExp(curLv)).ToString() + "/" +
            DataTableManager.Instance.ExpTable[(int)data.GetState(GASAttributeData.Instance.Rank)];
        StateUI[(int)eStateUI.STR].SetState(new BaseState((int)data.GetState(GASAttributeData.Instance.STR), (int)data.GetState(GASAttributeData.Instance.Grade_STR)), this);
        StateUI[(int)eStateUI.MAG].SetState(new BaseState((int)data.GetState(GASAttributeData.Instance.MAG), (int)data.GetState(GASAttributeData.Instance.Grade_MAG)), this);
        StateUI[(int)eStateUI.CON].SetState(new BaseState((int)data.GetState(GASAttributeData.Instance.CON), (int)data.GetState(GASAttributeData.Instance.Grade_CON)), this);
        StateUI[(int)eStateUI.AGI].SetState(new BaseState((int)data.GetState(GASAttributeData.Instance.AGI), (int)data.GetState(GASAttributeData.Instance.Grade_AGI)), this);
        StateUI[(int)eStateUI.SPR].SetState(new BaseState((int)data.GetState(GASAttributeData.Instance.SPR), (int)data.GetState(GASAttributeData.Instance.Grade_SPR)), this);
        StateUI[(int)eStateUI.LCK].SetState(new BaseState((int)data.GetState(GASAttributeData.Instance.LCK), (int)data.GetState(GASAttributeData.Instance.Grade_LCK)), this);


        HP.text = ((int)data.GetState(GASAttributeData.Instance.HP)).ToString();
        ActionCoin.text = ((int)data.GetState(GASAttributeData.Instance.ActionCoin)).ToString();
        AttackDamage.text = ((int)data.GetState(GASAttributeData.Instance.AttackDamage)).ToString();
        MagicDamage.text = ((int)data.GetState(GASAttributeData.Instance.MagicDamage)).ToString();
        AttackDefence.text = ((int)data.GetState(GASAttributeData.Instance.AttackDefence)).ToString();
        MagicDefence.text = ((int)data.GetState(GASAttributeData.Instance.MagicDefence)).ToString();


    }






    [System.Serializable]
    public struct BaseStateUI
    {
        public TextMeshProUGUI value;
        public Image GradeBack;
        public TextMeshProUGUI GradeValue;

        public void SetState(BaseState state , CharacterUI UI)
        {
            value.text = state.value.ToString();
            GradeValue.text = state.grade.ToString();
            switch (state.grade)
            {
                case eGrade.F:
                    GradeBack.color = UI.F_Image;
                    GradeValue.color = UI.F_Text;
                    break;
                case eGrade.E:
                    GradeBack.color = UI.E_Image;
                    GradeValue.color = UI.E_Text;
                    break;
                case eGrade.D:
                    GradeBack.color = UI.D_Image;
                    GradeValue.color = UI.D_Text;
                    break;
                case eGrade.C:
                    GradeBack.color = UI.C_Image;
                    GradeValue.color = UI.C_Text;
                    break;
                case eGrade.B:
                    GradeBack.color = UI.B_Image;
                    GradeValue.color = UI.B_Text;
                    break;
                case eGrade.A:
                    GradeBack.color = UI.A_Image;
                    GradeValue.color = UI.A_Text;
                    break;
                case eGrade.S:
                    GradeBack.color = UI.S_Image;
                    GradeValue.color = UI.S_Text;
                    break;
                default:
                    GradeBack.color = UI.F_Image;
                    GradeValue.color = UI.F_Text;
                    GradeValue.text = "?";
                    Debug.LogError("·©Å© ÃÊ°ú");
                    break;
            }
        }
    }

    public Color S_Image;
    public Color S_Text;
    public Color A_Image;
    public Color A_Text;
    public Color B_Image;
    public Color B_Text;
    public Color C_Image;
    public Color C_Text;
    public Color D_Image;
    public Color D_Text;
    public Color E_Image;
    public Color E_Text;
    public Color F_Image;
    public Color F_Text;
    [Button]
    public void SetTextColorView()
    {
        StateUI[(int)eStateUI.STR].GradeBack.color = A_Image;
        StateUI[(int)eStateUI.STR].GradeValue.color = A_Text;
        StateUI[(int)eStateUI.STR].GradeValue.text = "A";
        StateUI[(int)eStateUI.MAG].GradeBack.color = B_Image;
        StateUI[(int)eStateUI.MAG].GradeValue.color = B_Text;
        StateUI[(int)eStateUI.MAG].GradeValue.text = "B";
        StateUI[(int)eStateUI.CON].GradeBack.color = C_Image;
        StateUI[(int)eStateUI.CON].GradeValue.color = C_Text;
        StateUI[(int)eStateUI.CON].GradeValue.text = "C";
        StateUI[(int)eStateUI.AGI].GradeBack.color = D_Image;
        StateUI[(int)eStateUI.AGI].GradeValue.color = D_Text;
        StateUI[(int)eStateUI.AGI].GradeValue.text = "D";
        StateUI[(int)eStateUI.SPR].GradeBack.color = E_Image;
        StateUI[(int)eStateUI.SPR].GradeValue.color = E_Text;
        StateUI[(int)eStateUI.SPR].GradeValue.text = "E";
        StateUI[(int)eStateUI.LCK].GradeBack.color = F_Image;
        StateUI[(int)eStateUI.LCK].GradeValue.color = F_Text;
        StateUI[(int)eStateUI.LCK].GradeValue.text = "F";

        etc_S_UI.GradeBack.color = S_Image;
        etc_S_UI.GradeValue.color = S_Text;
        etc_S_UI.GradeValue.text = "S";
    }


    public enum eStateUI
    {
        STR,
        MAG,
        CON,
        AGI,
        SPR,
        LCK,
        COUNT

    }
}

