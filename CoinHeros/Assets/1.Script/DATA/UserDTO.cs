using System;
using System.Collections.Generic;
using UnityEngine;
using CoinHeros;

[Serializable]
public class UserDTO
{
    // 기본 정보
    public string uid;                    // Firebase Auth UID
    public string displayName;            // 사용자 표시 이름
    
    // 게임 데이터
    public int gold;                      // 골드
    public int maxStage;                  // 최대 진행 단계
    
    // 캐릭터 관련
    public List<string> characterIds = new List<string>();     // 보유 캐릭터 ID 목록
    public List<string> battleUnitIds = new List<string>();    // 전투 유닛 ID 목록
    
    // 인벤토리 관련
    public List<InventoryItemDTO> inventory = new List<InventoryItemDTO>();
    
    // 설정/상태
    public bool isFirstLogin;             // 첫 로그인 여부
    public string lastLoginTime;          // 마지막 로그인 시간
    public string createdAt;              // 계정 생성 시간
    public string lastModified;           // 마지막 수정 시간
    
    // 생성자
    public UserDTO()
    {
        createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
        lastModified = createdAt;
        lastLoginTime = createdAt;
        isFirstLogin = true;
    }
    
    // UserData에서 DTO로 변환
    public static UserDTO FromUserData(UserData userData, string uid, string existingCreatedAt = null)
    {
        var dto = new UserDTO();
        
        // 기존 createdAt이 있으면 유지, 없으면 새로 생성 (생성자에서 이미 설정됨)
        if (!string.IsNullOrEmpty(existingCreatedAt))
        {
            dto.createdAt = existingCreatedAt;
        }
        
        // 기본 정보
        dto.uid = uid;
        dto.displayName = userData._Name ?? "플레이어";
        
        // 게임 데이터
        dto.gold = userData.Gold;
        dto.maxStage = userData.MaxStage;
        
        // 캐릭터 ID 목록 (UniqueId 사용 - GetInstanceID는 게임 재시작 시 변경됨)
        dto.characterIds.Clear();
        foreach (var character in userData.UnitList)
        {
            if (character != null)
            {
                dto.characterIds.Add(character.UniqueId);
            }
        }
        
        // 전투 유닛 ID 목록 (UniqueId 사용 - GetInstanceID는 게임 재시작 시 변경됨)
        dto.battleUnitIds.Clear();
        for (int i = 0; i < userData.BattleUnit.Length; i++)
        {
            if (userData.BattleUnit[i] != null)
            {
                dto.battleUnitIds.Add(userData.BattleUnit[i].UniqueId);
            }
        }
        
        // 인벤토리
        dto.inventory.Clear();
        foreach (var slot in userData.globalInventory)
        {
            dto.inventory.Add(new InventoryItemDTO
            {
                itemId = slot.item.name,
                isEquipped = slot.isEquipped,
                equippedByInstanceId = slot.equippedBy?.GetInstanceID().ToString() ?? ""
            });
        }
        
        dto.lastModified = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
        dto.lastLoginTime = dto.lastModified;
        
        return dto;
    }
    
    // DTO를 Dictionary로 변환 (Firebase 저장용)
    public Dictionary<string, object> ToDictionary()
    {
        var dict = new Dictionary<string, object>
        {
            {"uid", uid},
            {"displayName", displayName},
            {"gold", gold},
            {"maxStage", maxStage},
            {"isFirstLogin", isFirstLogin},
            {"lastLoginTime", lastLoginTime},
            {"createdAt", createdAt},
            {"lastModified", lastModified}
        };
        
        // 캐릭터 ID 목록
        dict["characterIds"] = characterIds.ToArray();
        
        // 전투 유닛 ID 목록
        dict["battleUnitIds"] = battleUnitIds.ToArray();
        
        // 인벤토리를 Dictionary로 변환
        var inventoryDict = new Dictionary<string, object>();
        for (int i = 0; i < inventory.Count; i++)
        {
            inventoryDict[i.ToString()] = inventory[i].ToDictionary();
        }
        dict["inventory"] = inventoryDict;
        
        return dict;
    }
    
    // Dictionary에서 DTO로 변환 (Firebase 로드용)
    public static UserDTO FromDictionary(Dictionary<string, object> dict)
    {
        var dto = new UserDTO();
        
        dto.uid = dict.ContainsKey("uid") ? dict["uid"].ToString() : "";
        dto.displayName = dict.ContainsKey("displayName") ? dict["displayName"].ToString() : "플레이어";
        
        dto.gold = dict.ContainsKey("gold") ? Convert.ToInt32(dict["gold"]) : 10000;
        dto.maxStage = dict.ContainsKey("maxStage") ? Convert.ToInt32(dict["maxStage"]) : 1;
        
        dto.isFirstLogin = dict.ContainsKey("isFirstLogin") ? Convert.ToBoolean(dict["isFirstLogin"]) : true;
        dto.lastLoginTime = dict.ContainsKey("lastLoginTime") ? dict["lastLoginTime"].ToString() : "";
        dto.createdAt = dict.ContainsKey("createdAt") ? dict["createdAt"].ToString() : "";
        dto.lastModified = dict.ContainsKey("lastModified") ? dict["lastModified"].ToString() : "";
        
        // 캐릭터 ID 목록 로드
        if (dict.ContainsKey("characterIds") && dict["characterIds"] is object[] charIds)
        {
            dto.characterIds.Clear();
            foreach (var id in charIds)
            {
                dto.characterIds.Add(id.ToString());
            }
        }
        
        // 전투 유닛 ID 목록 로드
        if (dict.ContainsKey("battleUnitIds") && dict["battleUnitIds"] is object[] battleIds)
        {
            dto.battleUnitIds.Clear();
            foreach (var id in battleIds)
            {
                dto.battleUnitIds.Add(id.ToString());
            }
        }
        
        // 인벤토리 로드
        if (dict.ContainsKey("inventory") && dict["inventory"] is Dictionary<string, object> inventoryDict)
        {
            dto.inventory.Clear();
            foreach (var kvp in inventoryDict)
            {
                if (kvp.Value is Dictionary<string, object> itemDict)
                {
                    dto.inventory.Add(InventoryItemDTO.FromDictionary(itemDict));
                }
            }
        }
        
        return dto;
    }
    
    // DTO를 UserData에 적용 (로드 후)
    public void ApplyToUserData(UserData userData)
    {
        // 기본 정보
        userData._Name = this.displayName;
        
        // 게임 데이터
        userData.Gold = this.gold;
        userData.MaxStage = this.maxStage;
        
        // 인벤토리 복원
        userData.globalInventory.Clear();
        foreach (var itemDto in this.inventory)
        {
            var itemData = FindItemDataByName(itemDto.itemId);
            if (itemData != null)
            {
                var slot = new InventorySlot
                {
                    item = itemData,
                    isEquipped = itemDto.isEquipped,
                    equippedBy = null // 나중에 캐릭터 로드 후 연결
                };
                userData.globalInventory.Add(slot);
            }
        }
        
        Debug.Log($"[UserDTO] 사용자 데이터 적용 완료: {this.displayName} (골드: {this.gold})");
    }
    
    private ItemData FindItemDataByName(string itemName)
    {
        
        return null;
    }
}

[Serializable]
public class InventoryItemDTO
{
    public string itemId;                 // 아이템 ID (ItemData.name 또는 고유 ID)
    public bool isEquipped;               // 장착 여부
    public string equippedByInstanceId;   // 장착한 캐릭터 인스턴스 ID
    
    // Dictionary로 변환
    public Dictionary<string, object> ToDictionary()
    {
        return new Dictionary<string, object>
        {
            {"itemId", itemId},
            {"isEquipped", isEquipped},
            {"equippedByInstanceId", equippedByInstanceId}
        };
    }
    
    // Dictionary에서 변환
    public static InventoryItemDTO FromDictionary(Dictionary<string, object> dict)
    {
        return new InventoryItemDTO
        {
            itemId = dict.ContainsKey("itemId") ? dict["itemId"].ToString() : "",
            isEquipped = dict.ContainsKey("isEquipped") ? Convert.ToBoolean(dict["isEquipped"]) : false,
            equippedByInstanceId = dict.ContainsKey("equippedByInstanceId") ? dict["equippedByInstanceId"].ToString() : ""
        };
    }
}