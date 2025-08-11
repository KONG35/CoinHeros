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
    
    public bool isBattle=false;
    public bool isDead=false;

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
        _rig.velocity = Vector3.zero;
        _rig.angularVelocity = Vector3.zero;
        _rig.ResetInertiaTensor();

        isInit = false;

        if (UserData.Instance)
        {
            UserData.Instance.CopyQueue.Enqueue(this);
        }
        StartCoroutine(TextureInit());
        isDead=false;
    }
    public void Update()
    {
        if(isDead)
            return;
        if(_anim)
            _anim.SetFloat("Speed", _rig.velocity.magnitude);
    }
    public void Tick()
    {
        if(isDead)
            return;
        if(_ability)
            _ability.Action();
    }
    public void toMove(Vector3 Pos,float Speed)
    {
        StartCoroutine(eMove(Pos, Speed));
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
        isDead=false;
    }
    
    public void Hit(float damage, AttributeDefSO damageType, float penetration = 0f, float penetrationPercent = 0f)
    {
        if(isDead)
            return;
        var gasSOdata = GASAttributeData.Instance;
        float currentHP = GetState(gasSOdata.HP);
        
        // 데미지 타입에 따른 방어력 적용 (관통력 포함)
        float finalDamage = CalculateDamage(damage, damageType, penetration, penetrationPercent);
        float newHP = Mathf.Max(0, currentHP - finalDamage);
        
        SetModifyState(gasSOdata.HP, "CalcBase", newHP, StackPolicy.Override);
        
        // 히트 애니메이션 재생
        PlayAnim(eAnimState.Hit);
        
        // 데미지 타입에 따른 로그 메시지
        string damageTypeName = (damageType == gasSOdata.AttackDamage) ? "물리" : "마법";
        string penetrationText = "";
        if (penetration > 0 || penetrationPercent > 0)
        {
            penetrationText = " (관통력: ";
            if (penetration > 0) penetrationText += $"{penetration}";
            if (penetration > 0 && penetrationPercent > 0) penetrationText += " + ";
            if (penetrationPercent > 0) penetrationText += $"{penetrationPercent}%";
            penetrationText += ")";
        }
        Debug.Log($"{_name}이(가) {damageTypeName} 데미지 {damage}를 받았습니다.{penetrationText} (방어력 적용 후: {finalDamage}, HP: {currentHP} -> {newHP})");
        
        // HP가 0 이하가 되면 사망 처리
        if (newHP <= 0)
        {
            Die();
        }
    }
    

    
    // 특정 어빌리티를 추가하는 함수
    public void AddAbility(AbilityDefSO abilityDef)
    {
        if (_ability == null)
        {
            Debug.LogError($"{_name}: 어빌리티 컴포넌트가 없습니다.");
            return;
        }
        
        _ability.AddAbility(abilityDef);
        Debug.Log($"{_name}에게 {abilityDef.abilityName} 어빌리티가 추가되었습니다.");
    }
    
    // 어빌리티 비용을 설정하는 함수
    public void SetAbilityCost(AttributeDefSO costSO, float value)
    {
        if (_ability == null)
        {
            Debug.LogError($"{_name}: 어빌리티 컴포넌트가 없습니다.");
            return;
        }
        
        _ability.SetCost(costSO, value);
        Debug.Log($"{_name}의 어빌리티 비용이 설정되었습니다: {costSO.name} = {value}");
    }
    
    private float CalculateDamage(float baseDamage, AttributeDefSO damageType, float penetration = 0f, float penetrationPercent = 0f)
    {
        var gasSOdata = GASAttributeData.Instance;
        
        // 데미지 타입에 따른 방어력 결정
        float defence = 0f;
        if (damageType == gasSOdata.AttackDamage)
        {
            // 물리 데미지 -> 물리 방어력 적용
            defence = GetState(gasSOdata.AttackDefence);
        }
        else if (damageType == gasSOdata.MagicDamage)
        {
            // 마법 데미지 -> 마법 방어력 적용
            defence = GetState(gasSOdata.MagicDefence);
        }
        
        // 관통력에 따른 방어력 감소 (고정값 + 퍼센트)
        float flatPenetration = penetration; // 고정 관통력
        float percentPenetration = defence * (penetrationPercent / 100f); // 퍼센트 관통력
        float totalPenetration = flatPenetration + percentPenetration;
        
        float effectiveDefence = Mathf.Max(0f, defence - totalPenetration);
        
        // 방어력에 따른 데미지 감소 계산
        // 방어력이 높을수록 데미지가 감소하도록 계산
        float damageReduction = effectiveDefence / (effectiveDefence + 100f); // 방어력 공식 (필요에 따라 조정 가능)
        float finalDamage = baseDamage * (1f - damageReduction);
        
        return Mathf.Max(1f, finalDamage); // 최소 1 데미지는 보장
    }
    
    private void Die()
    {
        // 사망 애니메이션 재생
        PlayAnim(eAnimState.Die);
        
        Debug.Log($"{_name}이(가) 사망했습니다.");
        isDead=true;
        // 사망 시 추가 처리 (필요에 따라 확장)
        // 예: 경험치 지급, 아이템 드롭 등
    }
    
    // 현재 장착된 아이템에 따라 적절한 어빌리티를 반환하는 함수 (플레이어 캐릭터 전용)
    public virtual AbilityDefSO GetCurrentAbility()
    {
        // 기본 구현은 null 반환 (몬스터는 아이템을 사용하지 않음)
        return null;
    }
    
    // 어빌리티 이름으로 찾는 헬퍼 함수 (플레이어 캐릭터 전용)
    protected virtual AbilityDefSO GetAbilityByName(string abilityName)
    {
        // 기본 구현은 null 반환 (몬스터는 아이템을 사용하지 않음)
        return null;
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
