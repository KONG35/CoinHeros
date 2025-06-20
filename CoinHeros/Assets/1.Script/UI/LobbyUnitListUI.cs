using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUnitListUI : MonoBehaviour
{

    public UnitListItem prefab;
    public List<UnitListItem> ListItem;
    public Transform ListPanel;

    public void Awake()
    {
        ListItem = new List<UnitListItem>();
    }
    public async void OnEnable()
    {
        if (UserData.Instance == null)
            await WaitUntilAsync(() => UserData.Instance != null);
        if (!UserData.Instance.isInit)
            await WaitUntilAsync(() => UserData.Instance.isInit);
        SetListItem();

    }

    public void AddItem(CharacterData Data)
    {
        var item = Instantiate(prefab, ListPanel);
        item.SetUnitData(Data);
        ListItem.Add(item);
        item.gameObject.SetActive(true);
    }
    public void AddItem(CharacterData Data,UnitListItem item)
    {
        item.SetUnitData(Data);
        item.gameObject.SetActive(true);
    }
    public void SetListItem()
    {
        var UnitList = UserData.Instance.UnitList;
        foreach (var item in ListItem)
            item.gameObject.SetActive(false);

        for(int i=0;i<UnitList.Count;i++)
        {
            if (i < ListItem.Count)
                AddItem(UnitList[i], ListItem[i]);
            else
                AddItem(UnitList[i]);
        }
    }
    public async Task WaitUntilAsync(Func<bool> condition, int checkIntervalMs = 100)
    {
        while (!condition())
        {
            await Task.Delay(checkIntervalMs);
        }
    }
}
