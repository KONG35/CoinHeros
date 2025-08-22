using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using CoinHeros;
using System.Linq;

public class UserData : Singleton<UserData>
{
    public bool isInit = false;
    public string _Name;
    public List<CharacterData> UnitList;

    public CharacterData[] BattleUnit = new CharacterData[6];

    public Camera RenderTextureCamera;
    public RenderTexture texture;
    public Queue<CharacterBase> CopyQueue;

    // 전역 인벤토리 시스템 (아이템 보관만)
    [Header("전역 인벤토리")]
    public List<InventorySlot> globalInventory = new List<InventorySlot>();

    private int _gold;
    public int Gold
    {
        get
        {
            return _gold;
        }
        set
        {
            _gold = value;
            var lobby = FindObjectOfType<LobbyUI>();
            if (lobby)
                lobby.TextMoney.text = _gold.ToString();

        }
    }
    public int BattleUnitMaxCount = 1;

    public int MaxStage = 1;

    public void Start()
    {
        Init();
    }
    public void Init()
    {
        if (isInit)
            return;
        UnitList = new List<CharacterData>();
        CopyQueue = new Queue<CharacterBase>();

        Gold = 10000;

        if(UnitList.Count==0)
        {
            AddCharacter();
            BattleUnit[0] = UnitList.First();
        }
        isInit = true;

    }

    // 캐릭터 고유 ID 중복 검사 및 수정
    public void ValidateCharacterUniqueIds()
    {
        var usedIds = new HashSet<string>();
        var duplicates = new List<CharacterData>();

        foreach (var character in UnitList)
        {
            if (character == null) continue;

            if (usedIds.Contains(character.UniqueId))
            {
                // 중복된 ID 발견
                duplicates.Add(character);
                Debug.LogWarning($"[UserData] 중복된 캐릭터 ID 발견: {character._name} - {character.UniqueId}");
            }
            else
            {
                usedIds.Add(character.UniqueId);
            }
        }

        // 중복된 캐릭터들의 ID 재생성
        foreach (var duplicate in duplicates)
        {
            var oldId = duplicate.UniqueId;
            duplicate.UniqueId = GenerateNewUniqueId(usedIds);
            usedIds.Add(duplicate.UniqueId);
            Debug.Log($"[UserData] 캐릭터 ID 재생성: {duplicate._name} - {oldId} → {duplicate.UniqueId}");
        }
    }

    // 새로운 고유 ID 생성 (중복 방지)
    private string GenerateNewUniqueId(HashSet<string> existingIds)
    {
        string newId;
        int attempts = 0;
        const int maxAttempts = 100;

        do
        {
            var timestamp = System.DateTime.Now.Ticks + attempts;
            var random = UnityEngine.Random.Range(1000, 9999);
            newId = $"char_{timestamp}_{random}";
            attempts++;
        } while (existingIds.Contains(newId) && attempts < maxAttempts);

        if (attempts >= maxAttempts)
        {
            Debug.LogError("[UserData] 고유 ID 생성 실패 - 최대 시도 횟수 초과");
            newId = $"char_emergency_{System.DateTime.Now.Ticks}_{UnityEngine.Random.Range(10000, 99999)}";
        }

        return newId;
    }

    // 디버깅용: 모든 캐릭터의 고유 ID 출력
    [Button]
    public void DebugAllCharacterIds()
    {
        Debug.Log("=== 모든 캐릭터의 고유 ID ===");
        for (int i = 0; i < UnitList.Count; i++)
        {
            var character = UnitList[i];
            if (character != null)
            {
                Debug.Log($"[{i}] {character._name}: {character.UniqueId}");
            }
            else
            {
                Debug.Log($"[{i}] null");
            }
        }
        Debug.Log("==========================");
    }

    // 전역 인벤토리 관리 함수들 (단일 아이템 인스턴스)
    public bool AddItemToInventory(ItemData item)
    {
        // 이미 있는 아이템인지 확인
        var existingSlot = globalInventory.Find(slot => slot.item == item);
        
        if (existingSlot != null)
        {
            return false;
        }
        
        var newSlot = new InventorySlot
        {
            item = item,
            isEquipped = false,
            equippedBy = null
        };
        globalInventory.Add(newSlot);
        
        return true;
    }
    
    public bool RemoveItemFromInventory(ItemData item)
    {
        var slot = globalInventory.Find(s => s.item == item);
        if (slot == null)
        {
            return false;
        }
        
        // 장착 중인 아이템은 제거 불가
        if (slot.isEquipped)
        {
            return false;
        }
        
        globalInventory.Remove(slot);
        return true;
    }
    
    public bool HasItem(ItemData item)
    {
        var slot = globalInventory.Find(s => s.item == item);
        return slot != null;
    }
    
    public InventorySlot GetInventorySlot(ItemData item)
    {
        return globalInventory.Find(s => s.item == item);
    }

    [Button]
    public async void AddCharacter()
    {
        var CharacterList = DataTableManager.Instance.characterPrefabList;
        int index = UnityEngine.Random.Range(0,CharacterList.Count);
        //var Unit = Instantiate(CharacterList[index], this.transform);
        var Unit = ObjectManager.Instance.Get<CharacterData>(CharacterList[index].PoolSO);
        // 고유 ID 중복 검사 및 수정
        ValidateCharacterUniqueIds();
        
        UnitList.Add(Unit);

        await WaitUntilAsync(() => Unit.isInit);
        int grade = 0;
        var DTM = DataTableManager.Instance;
        Unit._name = DTM.CharNameList[UnityEngine.Random.Range(0, DTM.CharNameList.Count)];

        //STR
        float value = 100f * UnityEngine.Random.Range(0.5f, 1.2f);
        float Grade = UnityEngine.Random.Range((int)grade, (int)grade + 3);
        Unit.SetBaseState(GASAttributeData.Instance.STR, value);
        Unit.SetBaseState(GASAttributeData.Instance.Grade_STR, Grade);
        //MAG
        value = 100f * UnityEngine.Random.Range(0.5f, 1.2f);
        Grade = UnityEngine.Random.Range((int)grade, (int)grade + 3);
        Unit.SetBaseState(GASAttributeData.Instance.MAG, value);
        Unit.SetBaseState(GASAttributeData.Instance.Grade_MAG, Grade);
        //CON
        value = 100f * UnityEngine.Random.Range(0.5f, 1.2f);
        Grade = UnityEngine.Random.Range((int)grade, (int)grade + 3);
        Unit.SetBaseState(GASAttributeData.Instance.CON, value);
        Unit.SetBaseState(GASAttributeData.Instance.Grade_CON, Grade);
        //AGI
        value = 100f * UnityEngine.Random.Range(0.5f, 1.2f);
        Grade = UnityEngine.Random.Range((int)grade, (int)grade + 3);
        Unit.SetBaseState(GASAttributeData.Instance.AGI, value);
        Unit.SetBaseState(GASAttributeData.Instance.Grade_AGI, Grade);
        //SPR
        value = 100f * UnityEngine.Random.Range(0.5f, 1.2f);
        Grade = UnityEngine.Random.Range((int)grade, (int)grade + 3);
        Unit.SetBaseState(GASAttributeData.Instance.SPR, value);
        Unit.SetBaseState(GASAttributeData.Instance.Grade_SPR, Grade);
        //LCK
        value = 100f * UnityEngine.Random.Range(0.5f, 1.2f);
        Grade = UnityEngine.Random.Range((int)grade, (int)grade + 3);
        Unit.SetBaseState(GASAttributeData.Instance.LUK, value);
        Unit.SetBaseState(GASAttributeData.Instance.Grade_LUK, Grade);

        Unit.SetCalcBaseStateToDetailState();

        LobbyUI Lobby = FindObjectOfType<LobbyUI>();

        if(Lobby)
            Lobby.UnitListUI.SetListItem();
        Unit.gameObject.SetActive(false);
    }
    public void AddCharacter(CharacterData data)
    {
        LobbyUI Lobby = FindObjectOfType<LobbyUI>();
        UnitList.Add(data);
        Lobby.UnitListUI.SetListItem();
    }

    public Texture2D RenderTextureCopy()
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = texture;
        Texture2D tex = new Texture2D(texture.width, texture.height, TextureFormat.RGBAFloat, false);
        tex.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
        tex.Apply();
        RenderTexture.active = currentRT;

        return tex;
    }
    public async Task WaitUntilAsync(Func<bool> condition, int checkIntervalMs = 100)
    {
        while (!condition())
        {
            await Task.Delay(checkIntervalMs);
        }
    }
}