using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using CoinHeros;




[RequireComponent(typeof(TinyCharacterController))]
public class CharacterData : CharacterBase
{
    public AnimatorOverrideController[] jobAnims;
    public TinyCharacterController _controller;
    
    [Header("장착 아이템")]
    [SerializeField] private Dictionary<ItemType, ItemData> equippedItems = new Dictionary<ItemType, ItemData>();
    
    public Dictionary<ItemType, ItemData> EquippedItems => equippedItems;
    
    [Header("현재 장착 상태")]
    [SerializeField] private ItemData[] equippedItemsArray = new ItemData[7]; // ItemType.COUNT
    
    [Header("디버그 - 현재 어빌리티")]
    [SerializeField] private string currentAbilityName = "없음";
    
    [Button]
    public void UpdateAbilityDisplay()
    {
        var currentAbility = GetCurrentAbility();
        if (currentAbility != null)
        {
            currentAbilityName = currentAbility.abilityName;
            Debug.Log($"현재 어빌리티: {currentAbilityName}");
        }
        else
        {
            currentAbilityName = "없음";
            Debug.Log("현재 어빌리티가 없습니다.");
        }
    }
    
    private void UpdateInspectorArray()
    {
        equippedItemsArray = new ItemData[7];
        foreach (var kvp in equippedItems)
        {
            equippedItemsArray[(int)kvp.Key] = kvp.Value;
        }
    }
    
    protected override void Start()
    {
        base.Start();
    }

    public void OnEnable()
    {
        this.transform.localScale = Vector3.one*2.5f;
    }
    

    public void LvUpState()
    {

    }
    public void battleInit()
    {
        UpdateInspectorArray();
        UpdateCharacterStats();
        UpdateCharacterAppearance();
        UpdateAbilityDisplay();
        base.battleInit();
    }

    // 개별 캐릭터 장착 시스템
    public bool EquipItem(ItemData item)
    {
        // 전역 인벤토리에 아이템이 있는지 확인
        if (!UserData.Instance.HasItem(item))
        {
            return false;
        }
        
        // 이미 다른 캐릭터가 장착하고 있는지 확인
        var inventorySlot = UserData.Instance.GetInventorySlot(item);
        if (inventorySlot.isEquipped && inventorySlot.equippedBy != this)
        {
            return false;
        }
        
        // 제약 조건 체크
        if (!CanEquipItem(item))
        {
            return false;
        }
        
        // 기존 아이템 해제
        UnequipItem(item.itemType);
        
        // 새 아이템 장착
        equippedItems[item.itemType] = item;
        
        // 인스펙터 배열 업데이트
        UpdateInspectorArray();
        
        // 인벤토리 슬롯 업데이트
        UpdateInventorySlot(item, true);
        
        // 캐릭터 업데이트
        UpdateCharacterStats();
        UpdateCharacterAppearance();
        
        // 어빌리티 디스플레이 업데이트
        UpdateAbilityDisplay();
        
        return true;
    }
    
    private bool CanEquipItem(ItemData item)
    {
        switch (item.itemType)
        {
            case ItemType.SubWeapon:
                var mainWeapon = GetEquippedItem(ItemType.MainWeapon);
                if (mainWeapon == null) return true;
                return CheckWeaponCompatibility(mainWeapon, item);
                
            case ItemType.MainWeapon:
                var subWeapon = GetEquippedItem(ItemType.SubWeapon);
                if (subWeapon != null)
                {
                    return CheckWeaponCompatibility(item, subWeapon);
                }
                return true;
                
            default:
                return true;
        }
    }
    
    private bool CheckWeaponCompatibility(ItemData mainWeapon, ItemData subWeapon)
    {
        if (mainWeapon == null || subWeapon == null) return false;
        
        if (mainWeapon.weaponCategory == WeaponCategory.TwoHand)
        {
            return false;
        }
        
        if (mainWeapon.weaponType == WeaponType.Bow)
        {
            return subWeapon.weaponType == WeaponType.Arrow;
        }
        
        if (mainWeapon.weaponCategory == WeaponCategory.OneHand)
        {
            return subWeapon.weaponCategory == WeaponCategory.Shield || 
                   subWeapon.weaponCategory == WeaponCategory.OneHand;
        }
        
        return false;
    }
    
    public ItemData GetEquippedItem(ItemType itemType)
    {
        return equippedItems.ContainsKey(itemType) ? equippedItems[itemType] : null;
    }
    
    public void UnequipItem(ItemType itemType)
    {
        if (equippedItems.ContainsKey(itemType))
        {
            var unequippedItem = equippedItems[itemType];
            equippedItems.Remove(itemType);
            
            // 인스펙터 배열 업데이트
            UpdateInspectorArray();
            
            // 인벤토리 슬롯 업데이트
            UpdateInventorySlot(unequippedItem, false);
            
            // 호환성 체크 (주무기 해제 시)
            if (itemType == ItemType.MainWeapon)
            {
                var subWeapon = GetEquippedItem(ItemType.SubWeapon);
                if (subWeapon != null && !CheckWeaponCompatibility(unequippedItem, subWeapon))
                {
                    UnequipItem(ItemType.SubWeapon);
                }
            }
            
            UpdateCharacterStats();
            UpdateCharacterAppearance();
            
            // 어빌리티 디스플레이 업데이트
            UpdateAbilityDisplay();
            
        }
    }
    
    private void UpdateCharacterStats()
    {
        // 장착된 아이템들의 스탯 보너스 적용
        foreach (var equippedItem in equippedItems.Values)
        {
            foreach (var statBonus in equippedItem.statBonuses)
            {
                if (statBonus.attribute != null)
                {
                    SetModifyState(statBonus.attribute, "Equipment", statBonus.bonusValue, StackPolicy.Add);
                }
            }
        }
        
        SetCalcBaseStateToDetailState();
    }
    
    private void UpdateCharacterAppearance()
    {
        var newAbility = GetCurrentAbility();
        if (newAbility != null)
        {
            AddAbility(newAbility);
        }
        
        UpdateCharacterModel();
    }
    
    private void UpdateCharacterModel()
    {
        if (_controller != null)
        {
            DisableAllAppearance(_controller);
            // 장착된 아이템들의 외형 적용
            foreach (var equippedItem in equippedItems.Values)
            {
                ApplyItemAppearance(_controller, equippedItem);
            }
            
            // 모든 아이템 할당 완료 후 마지막에 SetCharacterSelectObject 호출
            _controller.SetCharacterSelectObject();
        }
    }
    
    private void DisableAllAppearance(TinyCharacterController controller)
    {
        // 모든 외형 리스트 비활성화
        DisableList(controller.Body);
        DisableList(controller.Cloak);
        DisableList(controller.BackPack);
        DisableList(controller.Head_Glass);
        DisableList(controller.Head_Ears);
        DisableList(controller.Head_Crown);
        DisableList(controller.Head_Mask);
        DisableList(controller.Head_Mustache);
        DisableList(controller.Head_Eye);
        DisableList(controller.Head_Mouth);
        DisableList(controller.Head_Hair);
        DisableList(controller.Head_Head);
        DisableList(controller.Head_Armor);
        DisableList(controller.Head_Hat);
        DisableList(controller.Head_EyeBrow);
        DisableList(controller.Left_Bow);
        DisableList(controller.Left_Sword);
        DisableList(controller.Left_Shield);
        DisableList(controller.Right_Arrow);
        DisableList(controller.Right_Sword);
        DisableList(controller.Right_TwoHandSword);
        DisableList(controller.Right_Wand);
        DisableList(controller.Right_Spear);
    }
    
    private void DisableList(List<GameObject> list)
    {
        if (list != null)
        {
            foreach (var obj in list)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }
    }
    
    private void ApplyItemAppearance(TinyCharacterController controller, ItemData item)
    {
        if (item.appearanceIndex < 0) return;
        
        switch (item.itemType)
        {
            case ItemType.MainWeapon:
                ApplyWeaponAppearance(controller, item);
                break;
            case ItemType.SubWeapon:
                ApplyWeaponAppearance(controller, item);
                break;
            case ItemType.Helmet:
                if (item.appearanceIndex < controller.Head_Hat.Count)
                    controller.Select_Hat = controller.Head_Hat[item.appearanceIndex];
                break;
            case ItemType.Armor:
                if (item.appearanceIndex < controller.Body.Count)
                    controller.Select_Body = controller.Body[item.appearanceIndex];
                break;
            case ItemType.Pants:
                // Pants는 별도 리스트가 없으므로 Body에 포함
                if (item.appearanceIndex < controller.Body.Count)
                    controller.Select_Body = controller.Body[item.appearanceIndex];
                break;
            case ItemType.Shoes:
                // Shoes는 별도 리스트가 없으므로 Body에 포함
                if (item.appearanceIndex < controller.Body.Count)
                    controller.Select_Body = controller.Body[item.appearanceIndex];
                break;
            case ItemType.Accessory:
                // Accessory는 여러 종류가 있을 수 있으므로 가장 적합한 리스트 선택
                if (item.appearanceIndex < controller.Head_Glass.Count)
                    controller.Select_Glass = controller.Head_Glass[item.appearanceIndex];
                break;
        }
    }
    
    private void ApplyWeaponAppearance(TinyCharacterController controller, ItemData item)
    {
        Debug.Log($"ApplyWeaponAppearance 호출: {item.itemName}, WeaponType: {item.weaponType}, AppearanceIndex: {item.appearanceIndex}");
        
        switch (item.weaponType)
        {
            case WeaponType.Bow:
                Debug.Log($"활 외형 적용 시도: Left_Bow.Count = {controller.Left_Bow?.Count}, AppearanceIndex = {item.appearanceIndex}");
                if (controller.Left_Bow != null && item.appearanceIndex < controller.Left_Bow.Count)
                {
                    controller.SelectLw = controller.Left_Bow[item.appearanceIndex];
                    controller.eLw = TinyCharacterController.eLWeapon.bow;
                    Debug.Log($"활 외형 적용 성공: SelectLw에 Left_Bow[{item.appearanceIndex}] 할당");
                }
                else
                {
                    Debug.LogWarning($"활 외형 적용 실패: Left_Bow가 null이거나 인덱스 범위 초과");
                }
                break;
            case WeaponType.OneHandSword:
                if (item.appearanceIndex < controller.Right_Sword.Count)
                {
                    controller.SelectRw = controller.Right_Sword[item.appearanceIndex];
                    controller.eRw = TinyCharacterController.eRWeapon.sword;
                }
                break;
            case WeaponType.TwoHandSword:
                if (item.appearanceIndex < controller.Right_TwoHandSword.Count)
                {
                    controller.SelectRw = controller.Right_TwoHandSword[item.appearanceIndex];
                    controller.eRw = TinyCharacterController.eRWeapon.twohandsword;
                }
                break;
            case WeaponType.Staff:
                if (item.appearanceIndex < controller.Right_Wand.Count)
                {
                    controller.SelectRw = controller.Right_Wand[item.appearanceIndex];
                    controller.eRw = TinyCharacterController.eRWeapon.wand;
                }
                break;
            case WeaponType.Shield:
                if (item.appearanceIndex < controller.Left_Shield.Count)
                {
                    controller.SelectLw = controller.Left_Shield[item.appearanceIndex];
                    controller.eLw = TinyCharacterController.eLWeapon.shield;
                }
                break;
            case WeaponType.Arrow:
                Debug.Log($"화살 외형 적용 시도: Right_Arrow.Count = {controller.Right_Arrow?.Count}, AppearanceIndex = {item.appearanceIndex}");
                if (controller.Right_Arrow != null && item.appearanceIndex < controller.Right_Arrow.Count)
                {
                    controller.SelectRw = controller.Right_Arrow[item.appearanceIndex];
                    controller.eRw = TinyCharacterController.eRWeapon.arrow;
                    Debug.Log($"화살 외형 적용 성공: SelectRw에 Right_Arrow[{item.appearanceIndex}] 할당");
                }
                else
                {
                    Debug.LogWarning($"화살 외형 적용 실패: Right_Arrow가 null이거나 인덱스 범위 초과");
                }
                break;
            case WeaponType.Spear:
                if (item.appearanceIndex < controller.Right_Spear.Count)
                {
                    controller.SelectRw = controller.Right_Spear[item.appearanceIndex];
                    controller.eRw = TinyCharacterController.eRWeapon.spear;
                }
                break;
        }
    }
    
    // CharacterBase의 GetCurrentAbility 오버라이드
    public override AbilityDefSO GetCurrentAbility()
    {
        var mainWeapon = GetEquippedItem(ItemType.MainWeapon);
        var subWeapon = GetEquippedItem(ItemType.SubWeapon);
        var data =  GASAttributeData.Instance;
        // 주무기가 없으면 기본 어빌리티 반환
        if (mainWeapon == null) return data.MeleeAttack;
        
        // 활인 경우 (화살이 있어도 없어도 활 어빌리티 사용)
        if (mainWeapon.weaponType == WeaponType.Bow)
        {
            return data.ArrowAttack;
        }
        
        // 양손무기인 경우
        if (mainWeapon.weaponCategory == WeaponCategory.TwoHand)
        {
            return data.TwoHandSwordAttack;
        }
        
        // 한손무기 + 방패 조합
        if (mainWeapon.weaponCategory == WeaponCategory.OneHand && 
            subWeapon != null && subWeapon.weaponCategory == WeaponCategory.Shield)
        {
            return data.SwordAndShieldAttack;
        }
        
        // 한손무기 + 한손무기 조합 (쌍검)
        if (mainWeapon.weaponCategory == WeaponCategory.OneHand && 
            subWeapon != null && subWeapon.weaponCategory == WeaponCategory.OneHand)
        {
            return data.DoubleSwordAttack;
        }
        
        return data.MeleeAttack;
    }
    
    // CharacterBase의 GetAbilityByName 오버라이드
    protected override AbilityDefSO GetAbilityByName(string abilityName)
    {
        // GASAbilityComponent에서 해당 이름의 어빌리티를 찾는 로직
        // 실제 구현은 GASAbilityComponent의 구조에 따라 달라질 수 있음
        
        return null; // 임시로 null 반환
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

    private void UpdateInventorySlot(ItemData item, bool isEquipping)
    {
        var slot = UserData.Instance.globalInventory.Find(s => s.item == item);
        if (slot != null)
        {
            slot.isEquipped = isEquipping;
            slot.equippedBy = isEquipping ? this : null;
        }
    }

    [Button]
    public void TestItemEquip()
    {
        // 테스트용 아이템들 생성
        var sword1 = CreateTestItem("테스트 검 1", ItemType.MainWeapon, WeaponType.OneHandSword, WeaponCategory.OneHand);
        var sword2 = CreateTestItem("테스트 검 2", ItemType.MainWeapon, WeaponType.OneHandSword, WeaponCategory.OneHand);
        var shield1 = CreateTestItem("테스트 방패 1", ItemType.SubWeapon, WeaponType.Shield, WeaponCategory.Shield);
        var bow1 = CreateTestItem("테스트 활 1", ItemType.MainWeapon, WeaponType.Bow, WeaponCategory.Ranged);
        var arrow1 = CreateTestItem("테스트 화살 1", ItemType.SubWeapon, WeaponType.Arrow, WeaponCategory.Ranged);
        var helmet1 = CreateTestItem("테스트 헬멧 1", ItemType.Helmet, WeaponType.OneHandSword, WeaponCategory.OneHand);
        var armor1 = CreateTestItem("테스트 갑옷 1", ItemType.Armor, WeaponType.OneHandSword, WeaponCategory.OneHand);
        
        // 인벤토리에 아이템들 추가
        UserData.Instance.AddItemToInventory(sword1);
        UserData.Instance.AddItemToInventory(sword2);
        UserData.Instance.AddItemToInventory(shield1);
        UserData.Instance.AddItemToInventory(bow1);
        UserData.Instance.AddItemToInventory(arrow1);
        UserData.Instance.AddItemToInventory(helmet1);
        UserData.Instance.AddItemToInventory(armor1);
        
        Debug.Log($"=== {_name} 테스트 아이템 장착 시작 ===");
        
        // 이 캐릭터에 아이템 장착 테스트
        // 검 장착
        if (EquipItem(sword1))
        {
            Debug.Log($"{_name}이(가) {sword1.itemName}을(를) 장착했습니다.");
        }
        
        // 방패 장착
        if (EquipItem(shield1))
        {
            Debug.Log($"{_name}이(가) {shield1.itemName}을(를) 장착했습니다.");
        }
        
        // 헬멧 장착
        if (EquipItem(helmet1))
        {
            Debug.Log($"{_name}이(가) {helmet1.itemName}을(를) 장착했습니다.");
        }
        
        // 갑옷 장착
        if (EquipItem(armor1))
        {
            Debug.Log($"{_name}이(가) {armor1.itemName}을(를) 장착했습니다.");
        }
        
        // 활과 화살 테스트 (기존 장비 해제 후)
        UnequipItem(ItemType.MainWeapon);
        UnequipItem(ItemType.SubWeapon);
        
        // 활 장착
        if (EquipItem(bow1))
        {
            Debug.Log($"{_name}이(가) {bow1.itemName}을(를) 장착했습니다.");
        }
        
        // 화살 장착
        if (EquipItem(arrow1))
        {
            Debug.Log($"{_name}이(가) {arrow1.itemName}을(를) 장착했습니다.");
        }
        
        Debug.Log($"=== {_name} 테스트 아이템 장착 완료 ===");
        
        // 현재 장착 상태 출력
        Debug.Log($"{_name}의 현재 장착 상태:");
        foreach (var equippedItem in equippedItems)
        {
            Debug.Log($"- {equippedItem.Key}: {equippedItem.Value.itemName}");
        }
        
        // 인벤토리 상태 출력
        Debug.Log($"전역 인벤토리 아이템 수: {UserData.Instance.globalInventory.Count}");
        foreach (var slot in UserData.Instance.globalInventory)
        {
            string equippedInfo = slot.isEquipped ? $" (장착: {slot.equippedBy._name})" : " (미장착)";
            Debug.Log($"- {slot.item.itemName}{equippedInfo}");
        }
    }
    
    [Button]
    public void ShowEquippedItems()
    {
        Debug.Log($"=== {_name}의 장착 아이템 목록 ===");
        
        if (equippedItems.Count == 0)
        {
            Debug.Log("장착된 아이템이 없습니다.");
            return;
        }
        
        foreach (var equippedItem in equippedItems)
        {
            var itemType = equippedItem.Key;
            var item = equippedItem.Value;
            
            Debug.Log($"[{itemType}] {item.itemName}");
            Debug.Log($"  - 무기 타입: {item.weaponType}");
            Debug.Log($"  - 무기 카테고리: {item.weaponCategory}");
            Debug.Log($"  - 외형 인덱스: {item.appearanceIndex}");
            
            // 스탯 보너스 출력
            if (item.statBonuses.Count > 0)
            {
                Debug.Log("  - 스탯 보너스:");
                foreach (var bonus in item.statBonuses)
                {
                    if (bonus.attribute != null)
                    {
                        Debug.Log($"    * {bonus.attribute.name}: +{bonus.bonusValue}");
                    }
                }
            }
            Debug.Log("");
        }
    }
    
    [Button]
    public void ShowAllInventoryItems()
    {
        Debug.Log($"=== 전역 인벤토리 아이템 목록 ===");
        
        if (UserData.Instance.globalInventory.Count == 0)
        {
            Debug.Log("인벤토리에 아이템이 없습니다.");
            return;
        }
        
        foreach (var slot in UserData.Instance.globalInventory)
        {
            string status = slot.isEquipped ? $"장착 중 ({slot.equippedBy._name})" : "미장착";
            Debug.Log($"[{slot.item.itemType}] {slot.item.itemName} - {status}");
        }
    }
    
    private ItemData CreateTestItem(string name, ItemType itemType, WeaponType weaponType, WeaponCategory weaponCategory)
    {
        var item = ScriptableObject.CreateInstance<ItemData>();
        item.itemName = name;
        item.itemType = itemType;
        item.weaponType = weaponType;
        item.weaponCategory = weaponCategory;
        item.appearanceIndex = 0; // 기본 외형 인덱스
        
        // 테스트용 스탯 보너스 추가
        var statBonus = new StatBonus();
        statBonus.attribute = GASAttributeData.Instance.STR; // STR 속성
        statBonus.bonusValue = 10f; // +10 보너스
        item.statBonuses.Add(statBonus);
        
        return item;
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