using System;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyBattleUI : MonoBehaviour
{
    public LobbyBattleUnitItem[] Item = new LobbyBattleUnitItem[6];

    public Button StartButton;

    public TextMeshProUGUI BattleUnitText;
    public void Awake()
    {
        StartButton.onClick.AddListener(OnClickStart); 
        SetItem();

    }

    public void OnEnable()
    {
        var ud = UserData.Instance;
        if (ud)
        {
            int battleUnitCount = 0;
            for(int i=0;i<ud.BattleUnit.Length;i++)
            {
                if (ud.BattleUnit[i] == null)
                    continue;
                battleUnitCount++;
            }
            BattleUnitText.text = battleUnitCount.ToString() + "/" + ud.BattleUnitMaxCount.ToString();
        } 
    }

    public void OnClickStart()
    {
        LoadBattleScene();
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

    
    // 씬 전환 메서드들
    public void LoadBattleScene()
    {
        Debug.Log("전투씬으로 이동합니다.");
        SceneManager.Instance.LoadBattleScene();
    }
}
