using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopUnitListItem : MonoBehaviour
{
    public CharacterData Unit;
    public RawImage UnitImage;
    public TextMeshProUGUI Name;

    public int _price;
    public TextMeshProUGUI txt_Price;
    public Button me;

    private LobbyUI lobbyui;

    public void Awake()
    {
        me.onClick.AddListener(Click);
        lobbyui = FindObjectOfType<LobbyUI>();
    }

    public void SetUnit(CharacterData data,int Price)
    {
        Unit = data;
        UnitImage.texture = data.Image;
        Name.text = data._name;
        _price = Price;

        txt_Price.text = Price.ToString();

        SetEmploy();
    }

    public void SetEmploy()
    {
        bool isEmploy = UserData.Instance.UnitList.Exists(x => x == Unit);
        me.interactable = !isEmploy;
    }

    public void Click()
    {
        if(!lobbyui)
            lobbyui = FindObjectOfType<LobbyUI>();
        if (Unit)
            lobbyui.UnitUI.SetCharacterData(Unit, _price);
    }

}
