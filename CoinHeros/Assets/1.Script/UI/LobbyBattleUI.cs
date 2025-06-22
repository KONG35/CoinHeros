using System;
using System.Threading.Tasks;
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
    public async void SetItem()
    {
        await WaitUntilAsync(() => UserData.Instance);

        var BattleUnits = UserData.Instance.BattleUnit;
        for(int i=0;i< Item.Length;i++)
        {
            Item[i].SetData(BattleUnits[i]);
        }
        SetStartBtnInteract();
    }

    public async Task WaitUntilAsync(Func<bool> condition, int checkIntervalMs = 100)
    {
        while (!condition())
        {
            await Task.Delay(checkIntervalMs);
        }
    }
}
