using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(GASTagComponent))]
[RequireComponent(typeof(GASAttributeSetComponent))]
[RequireComponent(typeof(GASAbilityComponent))]
[RequireComponent(typeof(GASCueComponent))]
[RequireComponent(typeof(Rigidbody))]
public class CharacterBase : MonoBehaviour
{
    public bool isInit = false;
    public string _name;
    public Texture2D Image;
    protected GASAbilityComponent _ability;
    protected GASAttributeSetComponent _state;
    protected GASCueComponent _cue;
    protected GASTagComponent _tag;
    protected Rigidbody _rig;
    public Animator _anim;
    
    // 코루틴 큐를 위한 필드들
    private Queue<IEnumerator> _coroutineQueue = new Queue<IEnumerator>();
    private bool _isExecutingCoroutine = false;

    protected virtual void Start()
    {
        _ability = GetComponent<GASAbilityComponent>();
        _state = GetComponent<GASAttributeSetComponent>();
        _cue = GetComponent<GASCueComponent>();
        _tag = GetComponent<GASTagComponent>();
        _anim = GetComponent<Animator>();
        _rig = GetComponent<Rigidbody>();

        _rig.useGravity = false;
        _anim.applyRootMotion = false;

        isInit = false;

        if (UserData.Instance)
        {
            UserData.Instance.CopyQueue.Enqueue(this);
        }
        StartCoroutine(TextureInit());
    }

    public void Update()
    {
        _anim.SetFloat("Speed", _rig.velocity.magnitude);
        
        ExecuteCoroutineQueueInUpdate();
    }
    public void toMove(Vector3 Pos,float Speed)
    {
        StartCoroutine(eMove(Pos, Speed));
    }

    public void toMoveWithQueue(Vector3 Pos, float Speed)
    {
        AddCoroutineToQueue(eMove(Pos, Speed));
    }
    IEnumerator eMove(Vector3 Pos,float Speed)
    {
        if(_rig ==null)
            _rig = GetComponent<Rigidbody>();
        while (true)
        {
            Vector3 toTarget = Pos - _rig.position;
            if (toTarget.sqrMagnitude < 0.01f)
            {
                _rig.velocity = Vector3.zero;
                _rig.position = Pos;
                break;
            }
            Vector3 direction = toTarget.normalized;
            _rig.velocity = direction * Speed;
            yield return new WaitForFixedUpdate();
        }
    }

    private void AddCoroutineToQueue(IEnumerator coroutine)
    {
        _coroutineQueue.Enqueue(coroutine);
    }

    private IEnumerator ExecuteCoroutineQueue()
    {
        _isExecutingCoroutine = true;
        
        while (_coroutineQueue.Count > 0)
        {
            IEnumerator currentCoroutine = _coroutineQueue.Dequeue();
            yield return StartCoroutine(currentCoroutine);
        }
        
        _isExecutingCoroutine = false;
    }

    public void ClearCoroutineQueue()
    {
        _coroutineQueue.Clear();
        _isExecutingCoroutine = false;
    }

    private void ExecuteCoroutineQueueInUpdate()
    {
        if (_coroutineQueue.Count > 0 && !_isExecutingCoroutine)
        {
            _isExecutingCoroutine = true;
            StartCoroutine(ExecuteCoroutineQueue());
        }
    }
    public void PlayAnim(eAnimState State)
    {
        switch (State)
        {
            case eAnimState.Attack:
                _anim.CrossFade("Attack",0.1f);
                break;
            case eAnimState.Ability:
                _anim.CrossFade("Ability", 0.1f);
                break;
            case eAnimState.Defence:
                _anim.CrossFade("Defence", 0.1f);
                break;
            case eAnimState.Hit:
                _anim.CrossFade("Hit", 0.1f);
                break;
            case eAnimState.Dizzy:  
                _anim.CrossFade("Dizzy", 0.1f);
                break;
            case eAnimState.Die:
                _anim.CrossFade("Die", 0.1f);
                break;
            case eAnimState.COUNT:
                _anim.CrossFade("Idle", 0.1f);
                break;
        }
    }

    public IEnumerator TextureInit()
    {
        yield return new WaitForEndOfFrame();
        while (UserData.Instance == null)
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
        if(_state==null)
            _state = GetComponent<GASAttributeSetComponent>();
        return _state.GetValue(SO);
    }
    public void SetModifyState(AttributeDefSO SO, string Key, float value, StackPolicy policy)
    {
        if(_state==null)
            _state = GetComponent<GASAttributeSetComponent>();
        _state.ModifyValue(SO, Key, value, policy);
    }
    public void SetBaseState(AttributeDefSO SO, float value)
    {
        if(_state==null)
            _state = GetComponent<GASAttributeSetComponent>();
        _state.SetBaseValue(SO, value);
    }
    public void SetBaseState(
        AttributeDefSO strSO, float strvalue, 
        AttributeDefSO magSO, float magvalue, 
        AttributeDefSO conSO, float convalue, 
        AttributeDefSO agiSO, float agivalue, 
        AttributeDefSO sprSO, float sprvalue, 
        AttributeDefSO lukSO, float lukvalue)
    {
        if(_state==null)
            _state = GetComponent<GASAttributeSetComponent>();
        _state.SetBaseValue(strSO, strvalue);
        _state.SetBaseValue(magSO, magvalue);
        _state.SetBaseValue(conSO, convalue);
        _state.SetBaseValue(agiSO, agivalue);
        _state.SetBaseValue(sprSO, sprvalue);
        _state.SetBaseValue(lukSO, lukvalue);
        SetCalcBaseStateToDetailState();
    }

    public void SetCoinCost(float value)
    {
        if(_state==null)
            _state = GetComponent<GASAttributeSetComponent>();
        _state.SetBaseValue(GASAttributeData.Instance.MaxActionCoin, value);

        //_ability.
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
        value = (GetState(gasSOdata.STR) * table.CONGradeEfficiency[(int)GetState(gasSOdata.Grade_STR)] * 0.1f)
            + (GetState(gasSOdata.CON) * table.CONGradeEfficiency[(int)GetState(gasSOdata.Grade_CON)] * 0.2f);
        SetModifyState(gasSOdata.AttackDefence, "CalcBase", value, StackPolicy.Override);
        //MagicDefence
        value = (GetState(gasSOdata.MAG) * table.CONGradeEfficiency[(int)GetState(gasSOdata.Grade_MAG)] * 0.1f)
            + (GetState(gasSOdata.AGI) * table.CONGradeEfficiency[(int)GetState(gasSOdata.Grade_AGI)] * 0.2f);
        SetModifyState(gasSOdata.MagicDefence, "CalcBase", value, StackPolicy.Override);
        //HP
        value = (GetState(gasSOdata.STR) * table.CONGradeEfficiency[(int)GetState(gasSOdata.Grade_STR)] * 0.25f)
            + (GetState(gasSOdata.CON) * table.CONGradeEfficiency[(int)GetState(gasSOdata.Grade_CON)] * 1.0f);
        SetModifyState(gasSOdata.MaxHP, "CalcBase", value, StackPolicy.Override);
        //ActionCoin
        value = 4f;
        SetModifyState(gasSOdata.ActionCoin, "CalcBase", value, StackPolicy.Override);
    }
    public void battleInit()
    {
        var gasSOdata = GASAttributeData.Instance;
        SetModifyState(gasSOdata.HP, "CalcBase", GetState(gasSOdata.MaxHP), StackPolicy.Override);
        SetModifyState(gasSOdata.MP, "CalcBase", 0, StackPolicy.Override);
        SetModifyState(gasSOdata.ActionCoin, "CalcBase", 0, StackPolicy.Override);
    }

    public enum eAnimState
    {
        Attack,
        Ability,
        Defence,
        Hit,
        Dizzy,
        Die,
        COUNT
    }
}
