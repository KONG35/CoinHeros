using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Purchasing;
using UnityEngine.UI;

public class LobbyUnitShopUI : MonoBehaviour
{
    public ShopUnitListItem prefab;
    public List<ShopUnitListItem> ShopUnitList;
    public Transform parent;
    public LobbyUI lobby;
    public Camera RenderTextureCamera;
    public RenderTexture texture;

    public Button btnReroll;

    public void Init()
    {
        lobby = FindObjectOfType<LobbyUI>();
        ShopUnitList = new List<ShopUnitListItem>();
        btnReroll.onClick.AddListener(ReRoll);
        ReRoll();

    }

    public void OnEnable()
    {
        if (ShopUnitList.Count != 0)
        {
            ShopUnitList[0].Click();
        }
    }
    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            ReRoll();
        }
    }


    public void SetItems()
    {
        foreach(var item in ShopUnitList)
        {
            item.gameObject.SetActive(false);
        }
        //for(int i=0;i<5;i++)
        {
            CreateCharacter(eGrade.F,0);
            CreateCharacter(eGrade.E,1);
            CreateCharacter(eGrade.C,2);
            CreateCharacter(eGrade.E,3);
            CreateCharacter(eGrade.D,4);
        }
    }

    public void ReRoll()
    {
        SetItems();
    }
    public void RefreshList()
    {
        foreach (var item in ShopUnitList)
        {
            item.SetEmploy();
        }
    }
    public async void CreateCharacter(eGrade grade,int ShopItemIndex)
    {
        await WaitUntilAsync(() => DataTableManager.Instance);
        var DTM = DataTableManager.Instance;
        var CharacterList = DTM.characterPrefabList;
        int index = UnityEngine.Random.Range(0, CharacterList.Count);
        var Unit = Instantiate(CharacterList[index], UserData.Instance.transform);
        await WaitUntilAsync(() => Unit.isInit);
        float GradeSum = 0;

        Unit._name = DTM.CharNameList[UnityEngine.Random.Range(0, DTM.CharNameList.Count)];

        //STR
        float value = 100f * UnityEngine.Random.Range(0.5f,1.2f);
        float Grade = UnityEngine.Random.Range((int)grade,(int)grade+3);
        Unit.SetBaseState(GASAttributeData.Instance.STR,value);
        Unit.SetBaseState(GASAttributeData.Instance.Grade_STR,Grade);
        GradeSum += Grade;
        //MAG
        value = 100f * UnityEngine.Random.Range(0.5f, 1.2f);
        Grade = UnityEngine.Random.Range((int)grade, (int)grade + 3);
        Unit.SetBaseState(GASAttributeData.Instance.MAG, value);
        Unit.SetBaseState(GASAttributeData.Instance.Grade_MAG, Grade);
        GradeSum += Grade;
        //CON
        value = 100f * UnityEngine.Random.Range(0.5f, 1.2f);
        Grade = UnityEngine.Random.Range((int)grade, (int)grade + 3);
        Unit.SetBaseState(GASAttributeData.Instance.CON, value);
        Unit.SetBaseState(GASAttributeData.Instance.Grade_CON, Grade);
        GradeSum += Grade;
        //AGI
        value = 100f * UnityEngine.Random.Range(0.5f, 1.2f);
        Grade = UnityEngine.Random.Range((int)grade, (int)grade + 3);
        Unit.SetBaseState(GASAttributeData.Instance.AGI, value);
        Unit.SetBaseState(GASAttributeData.Instance.Grade_AGI, Grade);
        GradeSum += Grade;
        //SPR
        value = 100f * UnityEngine.Random.Range(0.5f, 1.2f);
        Grade = UnityEngine.Random.Range((int)grade, (int)grade + 3);
        Unit.SetBaseState(GASAttributeData.Instance.SPR, value);
        Unit.SetBaseState(GASAttributeData.Instance.Grade_SPR, Grade);
        GradeSum += Grade;
        //LCK
        value = 100f * UnityEngine.Random.Range(0.5f, 1.2f);
        Grade = UnityEngine.Random.Range((int)grade, (int)grade + 3);
        Unit.SetBaseState(GASAttributeData.Instance.LUK, value);
        Unit.SetBaseState(GASAttributeData.Instance.Grade_LUK, Grade);
        GradeSum += Grade;

        Unit.SetCalcBaseStateToDetailState();
        int price = Mathf.FloorToInt(200 * Mathf.Pow(GradeSum, 1.8f));

        if (ShopUnitList.Count <= ShopItemIndex)
        {
            var Item = Instantiate(prefab, parent);
            Item.SetUnit(Unit, price);
            ShopUnitList.Add(Item);
            Item.gameObject.SetActive(true);
        }
        else
        {
            ShopUnitList[ShopItemIndex].SetUnit(Unit, price);
            ShopUnitList[ShopItemIndex].gameObject.SetActive(true);
        }
    }
    public Texture2D RenderTextureCopy()
    {
        texture = UserData.Instance.texture;
        RenderTextureCamera = UserData.Instance.RenderTextureCamera;
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
