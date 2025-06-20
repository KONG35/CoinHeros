using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitListItem : MonoBehaviour
{
    public CharacterData UnitData;
    public RawImage Image;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI LV;
    public List<Image> ItemImageList;

    public Button Battle;
    public Button Rest;


    public Button me;


    private void Awake()
    {
        Battle.onClick.AddListener(UnitBattleGo);
        Rest.onClick.AddListener(UnitRestGo);
        me.onClick.AddListener(SetCharacterData);
    }

    public void SetCharacterData()
    {
        var lobby = FindObjectOfType<LobbyUI>();
        lobby.UnitUI.SetCharacterData(UnitData);
    }

    public void SetUnitData(CharacterData Data)
    {
        UnitData = Data;
        Name.text = Data._name;
        Image.texture = Data.Image;
        LV.text = ((int)Data.GetState(GASAttributeData.Instance.LV)).ToString();
        ButtonActiveSet();
    }


    public void UnitBattleGo()
    {
        var BattleUnits = UserData.Instance.BattleUnit;

        bool isFull = true;

        foreach (var unit in BattleUnits)
        {
            if (!unit)
            {
                isFull = false;
                break;
            }
        }
        if(isFull)
        {
            return;
        }

        for(int i=0;i<BattleUnits.Length;i++)
        {
            if(BattleUnits[i]==null)
            {
                BattleUnits[i] = UnitData;
                break;
            }
        }
        ButtonActiveSet();
    }
    public void UnitRestGo()
    {
        var BattleUnits = UserData.Instance.BattleUnit;

        for(int i=0;i<BattleUnits.Length;i++)
        {
            if (BattleUnits[i] == UnitData)
                BattleUnits[i] = null;
        }
        ButtonActiveSet();
    }
    public void ButtonActiveSet()
    {
        var BattleUnits = UserData.Instance.BattleUnit;

        bool isBattle = BattleUnits.Contains(UnitData);
        Battle.gameObject.SetActive(!isBattle);
        Rest.gameObject.SetActive(isBattle);

        var LobbyUI = FindObjectOfType<LobbyUI>();
        LobbyUI.BattleUI.SetItem();
    }


    enum ItemList
    {
        WEAPON,
        ARMOR,
        PANTS,
        GLOVES,
        SHOES,
        ACC,
        COUNT
    }

}
