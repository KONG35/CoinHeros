using System.Collections.Generic;
using UnityEngine;

namespace CoinHeros
{
    [System.Serializable]
    public enum ItemType
{
    MainWeapon,     // 주무기
    SubWeapon,      // 보조무기
    Helmet,         // 헬멧
    Armor,          // 갑옷
    Pants,          // 바지
    Shoes,          // 신발
    Accessory,      // 악세서리
    COUNT
}

    [System.Serializable]
    public enum WeaponType
{
    OneHandSword,   // 한손검
    TwoHandSword,   // 양손검
    OneHandAxe,     // 한손도끼
    TwoHandAxe,     // 양손도끼
    Spear,          // 창
    Bow,            // 활
    Staff,          // 지팡이
    Shield,         // 방패
    Arrow,          // 화살
    COUNT
}

    [System.Serializable]
    public enum WeaponCategory
{
    OneHand,        // 한손무기
    TwoHand,        // 양손무기
    Ranged,         // 원거리무기
    Shield,         // 방패
    COUNT
}

    [CreateAssetMenu(menuName = "Items/ItemData")]
    public class ItemData : ScriptableObject
{
    [Header("기본 정보")]
    public string itemName;
    public ItemType itemType;
    public WeaponType weaponType;
    public WeaponCategory weaponCategory;
    public Sprite icon;
    public GameObject prefab;
    
    [Header("스탯 보너스")]
    public List<StatBonus> statBonuses = new List<StatBonus>();
    
    [Header("어빌리티 관련")]
    public AbilityDefSO defaultAbility;
    
    [Header("외형 관련")]
    public int appearanceIndex = -1;     // 해당 아이템 타입에 맞는 외형 리스트 인덱스
    
    [Header("제약 조건")]
    public bool canUseWithShield; // 방패와 함께 사용 가능한지
    public bool requiresArrow;     // 화살이 필요한지 (활일 때)
}

    [System.Serializable]
    public class StatBonus
{
        public AttributeDefSO attribute;
        public float bonusValue;
    }
} 