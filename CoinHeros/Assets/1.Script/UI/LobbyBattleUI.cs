using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyBattleUI : MonoBehaviour
{
    public LobbyBattleUnitItem[] Item = new LobbyBattleUnitItem[6];

    public Button StartButton;
    public void Awake()
    {
        StartButton.onClick.AddListener(OnClickStart); 
        SetItem();

    }

    public void OnClickStart()
    {

    }

    public void SetStartBtnInteract()
    {
        foreach (var i in Item)
        {
            if (i.unitData)
            {
                StartButton.interactable = true;
                break;
            }
        }
    }
    public void SetItem()
    {
        var BattleUnits = UserData.Instance.BattleUnit;
        for(int i=0;i< Item.Length;i++)
        {
            Item[i].SetData(BattleUnits[i]);
        }
        SetStartBtnInteract();
    }

}
