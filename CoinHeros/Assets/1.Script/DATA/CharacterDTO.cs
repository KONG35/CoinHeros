using System;
using System.Collections.Generic;
using UnityEngine;
using CoinHeros;
using System.Linq;
using System.Xml;

[Serializable]
public class CharacterDTO
{
    // 기본 정보
    public string instanceId;        // 캐릭터 인스턴스 고유 ID
    public string characterId;       // 캐릭터 정의 ID (프리팹/템플릿 참조)
    public string name;              // 캐릭터 이름

    // 레벨/경험치
    public int level;                // 현재 레벨
    public long exp;                 // 현재 경험치
    public int rank;                 // 현재 랭크

    // 기본 스탯
    public int str;                  // 힘
    public int mag;                  // 마법
    public int con;                  // 체력
    public int agi;                  // 민첩
    public int spr;                  // 정신
    public int luk;                  // 행운

    // 등급 (필요시)
    public int grade_str;            // 힘 등급
    public int grade_mag;            // 마법 등급
    public int grade_con;            // 체력 등급
    public int grade_agi;            // 민첩 등급
    public int grade_spr;            // 정신 등급
    public int grade_luk;            // 행운 등급

    // 장착 아이템 (ItemType enum을 int로 저장)
    public List<EquippedItemRecordDTO> equippedItems = new List<EquippedItemRecordDTO>();

    public bool isBattle = false;
    public int BattleIndex = 0;

    // 생성 시간
    public string createdAt;         // 생성 시간 (ISO 문자열)
    public string lastModified;      // 마지막 수정 시간

    // 생성자
    public CharacterDTO()
    {
        createdAt = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");
        lastModified = createdAt;
    }



    // CharacterData에서 DTO로 변환
    public static CharacterDTO FromCharacterData(CharacterData characterData, string existingCreatedAt = null)
    {
        var dto = new CharacterDTO();

        // 기존 createdAt이 있으면 유지, 없으면 새로 생성 (생성자에서 이미 설정됨)
        if (!string.IsNullOrEmpty(existingCreatedAt))
        {
            dto.createdAt = existingCreatedAt;
        }

        // 기본 정보 - CharacterData의 고유 ID 사용
        dto.instanceId = characterData.UniqueId;

        dto.name = characterData._name;

        // 레벨/경험치
        dto.level = (int)characterData.GetState(GASAttributeData.Instance.LV);
        dto.exp = (long)characterData.GetState(GASAttributeData.Instance.EXP);
        dto.rank = (int)characterData.GetState(GASAttributeData.Instance.Rank);

        // 기본 스탯
        dto.str = (int)characterData.GetState(GASAttributeData.Instance.STR);
        dto.mag = (int)characterData.GetState(GASAttributeData.Instance.MAG);
        dto.con = (int)characterData.GetState(GASAttributeData.Instance.CON);
        dto.agi = (int)characterData.GetState(GASAttributeData.Instance.AGI);
        dto.spr = (int)characterData.GetState(GASAttributeData.Instance.SPR);
        dto.luk = (int)characterData.GetState(GASAttributeData.Instance.LUK);

        // 등급
        dto.grade_str = (int)characterData.GetState(GASAttributeData.Instance.Grade_STR);
        dto.grade_mag = (int)characterData.GetState(GASAttributeData.Instance.Grade_MAG);
        dto.grade_con = (int)characterData.GetState(GASAttributeData.Instance.Grade_CON);
        dto.grade_agi = (int)characterData.GetState(GASAttributeData.Instance.Grade_AGI);
        dto.grade_spr = (int)characterData.GetState(GASAttributeData.Instance.Grade_SPR);
        dto.grade_luk = (int)characterData.GetState(GASAttributeData.Instance.Grade_LUK);

        dto.isBattle = UserData.Instance.BattleUnit.Contains(characterData);
        if (dto.isBattle)
            dto.BattleIndex = Array.IndexOf(UserData.Instance.BattleUnit, characterData);
        else
            dto.BattleIndex = -1;

        // 장착 아이템
        foreach (var kvp in characterData.EquippedItems)
        {
            dto.equippedItems.Add(new EquippedItemRecordDTO
            {
                slot = (int)kvp.Key,
                itemId = kvp.Value.name // ItemData의 name을 ID로 사용 (나중에 고유 ID로 변경 가능)
            });
        }


        dto.lastModified = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ");

        return dto;
    }

    // DTO를 Dictionary로 변환 (Firebase 저장용)
    public Dictionary<string, object> ToDictionary()
    {
        var dict = new Dictionary<string, object>
        {
            {"instanceId", instanceId},
            {"characterId", characterId},
            {"name", name},
            {"level", level},
            {"exp", exp},
            {"rank", rank},
            {"str", str},
            {"mag", mag},
            {"con", con},
            {"agi", agi},
            {"spr", spr},
            {"luk", luk},
            {"grade_str", grade_str},
            {"grade_mag", grade_mag},
            {"grade_con", grade_con},
            {"grade_agi", grade_agi},
            {"grade_spr", grade_spr},
            {"grade_luk", grade_luk},
            {"isBattle", isBattle},
            {"battleIndex", BattleIndex},
            {"createdAt", createdAt},
            {"lastModified", lastModified}
        };

        // 장착 아이템을 Dictionary로 변환
        var equippedDict = new Dictionary<string, object>();
        foreach (var item in equippedItems)
        {
            equippedDict[item.slot.ToString()] = new Dictionary<string, object>
            {
                {"slot", item.slot},
                {"itemId", item.itemId}
            };
        }
        dict["equippedItems"] = equippedDict;

        return dict;
    }

    // Dictionary에서 DTO로 변환 (Firebase 로드용)
    public static CharacterDTO FromDictionary(Dictionary<string, object> dict)
    {
        var dto = new CharacterDTO();

        dto.instanceId = dict.ContainsKey("instanceId") ? dict["instanceId"].ToString() : "";
        dto.characterId = dict.ContainsKey("characterId") ? dict["characterId"].ToString() : "";
        dto.name = dict.ContainsKey("name") ? dict["name"].ToString() : "";
        dto.level = dict.ContainsKey("level") ? Convert.ToInt32(dict["level"]) : 1;
        dto.exp = dict.ContainsKey("exp") ? Convert.ToInt64(dict["exp"]) : 0;
        dto.rank = dict.ContainsKey("rank") ? Convert.ToInt32(dict["rank"]) : 0;

        dto.str = dict.ContainsKey("str") ? Convert.ToInt32(dict["str"]) : 0;
        dto.mag = dict.ContainsKey("mag") ? Convert.ToInt32(dict["mag"]) : 0;
        dto.con = dict.ContainsKey("con") ? Convert.ToInt32(dict["con"]) : 0;
        dto.agi = dict.ContainsKey("agi") ? Convert.ToInt32(dict["agi"]) : 0;
        dto.spr = dict.ContainsKey("spr") ? Convert.ToInt32(dict["spr"]) : 0;
        dto.luk = dict.ContainsKey("luk") ? Convert.ToInt32(dict["luk"]) : 0;

        dto.grade_str = dict.ContainsKey("grade_str") ? Convert.ToInt32(dict["grade_str"]) : 0;
        dto.grade_mag = dict.ContainsKey("grade_mag") ? Convert.ToInt32(dict["grade_mag"]) : 0;
        dto.grade_con = dict.ContainsKey("grade_con") ? Convert.ToInt32(dict["grade_con"]) : 0;
        dto.grade_agi = dict.ContainsKey("grade_agi") ? Convert.ToInt32(dict["grade_agi"]) : 0;
        dto.grade_spr = dict.ContainsKey("grade_spr") ? Convert.ToInt32(dict["grade_spr"]) : 0;
        dto.grade_luk = dict.ContainsKey("grade_luk") ? Convert.ToInt32(dict["grade_luk"]) : 0;

        dto.createdAt = dict.ContainsKey("createdAt") ? dict["createdAt"].ToString() : "";
        dto.lastModified = dict.ContainsKey("lastModified") ? dict["lastModified"].ToString() : "";


        dto.isBattle = dict.ContainsKey("isBattle") ? Convert.ToBoolean(dict["isBattle"]) : false;
        dto.BattleIndex = dict.ContainsKey("battleIndex") ? Convert.ToInt32(dict["battleIndex"]) : -1;

        // 장착 아이템 로드
        if (dict.ContainsKey("equippedItems") && dict["equippedItems"] is Dictionary<string, object> equippedDict)
        {
            foreach (var kvp in equippedDict)
            {
                if (kvp.Value is Dictionary<string, object> itemDict)
                {
                    dto.equippedItems.Add(new EquippedItemRecordDTO
                    {
                        slot = Convert.ToInt32(itemDict["slot"]),
                        itemId = itemDict["itemId"].ToString()
                    });
                }
            }
        }

        return dto;
    }

    // DTO에서 CharacterData로 변환 (Firebase 로드 후 게임 오브젝트 생성)
    public CharacterData ToCharacterData(GameObject basePrefab, Transform parent = null)
    {
        // 프리팹 인스턴스 생성
        var go = UnityEngine.Object.Instantiate(basePrefab, parent);

        var characterData = go.GetComponent<CharacterData>();

        if (characterData == null)
        {
            Debug.LogError("[CharacterDTO] 프리팹에 CharacterData 컴포넌트가 없습니다!");
            UnityEngine.Object.Destroy(go);
            return null;
        }

        // 기본 정보 설정
        characterData._name = this.name;
        characterData.UniqueId = this.instanceId; // Firebase에서 로드된 고유 ID 설정

        // 레벨/경험치 설정
        characterData.SetBaseState(GASAttributeData.Instance.LV, this.level);
        characterData.SetBaseState(GASAttributeData.Instance.EXP, this.exp);
        characterData.SetBaseState(GASAttributeData.Instance.Rank, this.rank);

        // 기본 스탯 설정
        characterData.SetBaseState(GASAttributeData.Instance.STR, this.str);
        characterData.SetBaseState(GASAttributeData.Instance.MAG, this.mag);
        characterData.SetBaseState(GASAttributeData.Instance.CON, this.con);
        characterData.SetBaseState(GASAttributeData.Instance.AGI, this.agi);
        characterData.SetBaseState(GASAttributeData.Instance.SPR, this.spr);
        characterData.SetBaseState(GASAttributeData.Instance.LUK, this.luk);

        // 등급 설정
        characterData.SetBaseState(GASAttributeData.Instance.Grade_STR, this.grade_str);
        characterData.SetBaseState(GASAttributeData.Instance.Grade_MAG, this.grade_mag);
        characterData.SetBaseState(GASAttributeData.Instance.Grade_CON, this.grade_con);
        characterData.SetBaseState(GASAttributeData.Instance.Grade_AGI, this.grade_agi);
        characterData.SetBaseState(GASAttributeData.Instance.Grade_SPR, this.grade_spr);
        characterData.SetBaseState(GASAttributeData.Instance.Grade_LUK, this.grade_luk);


        if (this.isBattle)
            UserData.Instance.BattleUnit[this.BattleIndex] = characterData;

        // 장착 아이템 복원
        foreach (var equippedItem in this.equippedItems)
        {
            // ItemData를 찾아서 장착 (임시로 name으로 찾기)
            var itemData = FindItemDataByName(equippedItem.itemId);
            if (itemData != null)
            {
                characterData.EquipItem(itemData);
            }
            else
            {
                Debug.LogWarning($"[CharacterDTO] 아이템을 찾을 수 없습니다: {equippedItem.itemId}");
            }
        }

        // 파생 스탯 계산
        characterData.SetCalcBaseStateToDetailState();

        // 전투 초기화
        //characterData.battleInit();


        Debug.Log($"[CharacterDTO] 캐릭터 생성 완료: {this.name} (레벨 {this.level})");

        return characterData;
    }


    private ItemData FindItemDataByName(string itemName)
    {
        return null;
    }
}

[Serializable]
public class EquippedItemRecordDTO
{
    public int slot;        // ItemType enum을 int로 저장
    public string itemId;   // 아이템 ID (ItemData.name 또는 고유 ID)
}
