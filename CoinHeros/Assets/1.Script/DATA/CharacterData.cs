using System.Collections;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.VisualScripting;
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


    public void Start()
    {
        isInit = false;
        _ability = GetComponent<GASAbilityComponent>();
        _state = GetComponent<GASAttributeSetComponent>();
        _cue = GetComponent<GASCueComponent>();
        _tag = GetComponent<GASTagComponent>();

        if(UserData.Instance)
        {
            UserData.Instance.CopyQueue.Enqueue(this);
        }
        StartCoroutine(TextureInit());
    }

    public IEnumerator TextureInit()
    {
        yield return new WaitForEndOfFrame();
        while (UserData.Instance==null)
            yield return new WaitForEndOfFrame();
        while (UserData.Instance.CopyQueue.First() != this)
            yield return new WaitForEndOfFrame();
        transform.localPosition = UserData.Instance.RenderTextureCamera.transform.localPosition - new Vector3(0.97f, 1.06f, 5.5f);
        yield return new WaitForEndOfFrame();
        UserData.Instance.CopyQueue.Dequeue();
        SetTextureCopyToImage();
        transform.localPosition = Vector3.zero;
        isInit = true;
    }

    public float GetState(AttributeDefSO SO)
    {
        return _state.GetValue(SO);
    }
    public void SetModifyState(AttributeDefSO SO, string Key, float value, StackPolicy policy)
    {
        _state.ModifyValue(SO, Key, value, policy);
    }
    public void SetBaseState(AttributeDefSO SO, float value)
    {
        _state.SetBaseValue(SO, value);
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
    public void SetTextureCopyToImage()
    {
        var texture = UserData.Instance.texture;
        var RenderTextureCamera = UserData.Instance.RenderTextureCamera;
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = texture;
        Image = new Texture2D(texture.width, texture.height, TextureFormat.RGBAFloat, false);
        Image.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        Image.Apply();
        RenderTexture.active = currentRT;
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